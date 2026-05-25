using UnityEngine;

/// <summary>
/// Tiny "juice" helper: on Punch() the object snaps to a deformed scale, then
/// springs smoothly back to its base scale. Configure the punch per object:
///   Ball   -> uniform pop  (Punch X = Punch Y = ~1.25)
///   Paddle -> directional squash (Punch X length-axis ~1.15, Punch Y thickness ~0.8)
/// Uses unscaled time so it always settles back, even if the game freezes on a point.
/// </summary>
public class JuicePop : MonoBehaviour
{
    [Header("Punch (multipliers of base local scale)")]
    [SerializeField] private float punchX = 1.25f;
    [SerializeField] private float punchY = 1.25f;
    [SerializeField] private float recovery = 14f;   // higher = snappier spring-back

    private Vector3 _base;

    private void Awake() => _base = transform.localScale;

    public void Punch()
    {
        transform.localScale = new Vector3(_base.x * punchX, _base.y * punchY, _base.z);
    }

    private void Update()
    {
        Vector3 s = transform.localScale;
        if ((s - _base).sqrMagnitude <= 1e-6f) return;

        s = Vector3.Lerp(s, _base, recovery * Time.unscaledDeltaTime);
        if ((s - _base).sqrMagnitude <= 1e-6f) s = _base;
        transform.localScale = s;
    }
}
