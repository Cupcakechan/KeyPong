using UnityEngine;

/// <summary>
/// Subtle camera shake. Attach to the Main Camera. Other systems call
/// CameraShake.Instance.ShakeHit() (paddle bounce) or ShakeScore() (point scored).
/// The camera offsets from its rest position by a decaying random amount, then
/// snaps back exactly. Uses unscaled time so it works even when the game is frozen.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Paddle Hit (subtle)")]
    [SerializeField] private float hitDuration = 0.12f;
    [SerializeField] private float hitMagnitude = 0.06f;

    [Header("Score (a touch stronger)")]
    [SerializeField] private float scoreDuration = 0.25f;
    [SerializeField] private float scoreMagnitude = 0.15f;

    private Vector3 _basePos;
    private float _duration;
    private float _magnitude;
    private float _elapsed;
    private bool _shaking;

    private void Awake()
    {
        Instance = this;
        _basePos = transform.localPosition;
    }

    public void ShakeHit()   => Shake(hitDuration, hitMagnitude);
    public void ShakeScore() => Shake(scoreDuration, scoreMagnitude);

    public void Shake(float duration, float magnitude)
    {
        _duration = Mathf.Max(0.01f, duration);
        _magnitude = magnitude;
        _elapsed = 0f;
        _shaking = true;
    }

    private void LateUpdate()
    {
        if (!_shaking) return;

        _elapsed += Time.unscaledDeltaTime;
        if (_elapsed >= _duration)
        {
            _shaking = false;
            transform.localPosition = _basePos;   // snap back to rest
            return;
        }

        float damper = 1f - (_elapsed / _duration);          // decay to zero
        Vector2 offset = Random.insideUnitCircle * _magnitude * damper;
        transform.localPosition = _basePos + (Vector3)offset; // z stays put
    }
}
