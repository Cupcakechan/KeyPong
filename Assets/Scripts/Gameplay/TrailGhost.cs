using UnityEngine;

/// <summary>
/// One reusable trail fragment. When Spawn() is called it snapshots the ball's
/// current key sprite/position, then fades its alpha to 0 and shrinks over its
/// lifetime before deactivating itself (ready to be reused by the pool).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TrailGhost : MonoBehaviour
{
    private SpriteRenderer _sr;
    private float _lifetime;
    private float _age;
    private float _startAlpha;
    private Vector3 _startScale;
    private Vector3 _endScale;
    private bool _active;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.enabled = false;
    }

    public void Spawn(Sprite sprite, Vector3 position, Vector3 startScale, Vector3 endScale,
                      Color tint, float startAlpha, float lifetime)
    {
        _sr.sprite = sprite;
        transform.position = position;
        _startScale = startScale;
        _endScale = endScale;
        transform.localScale = startScale;
        _startAlpha = startAlpha;
        _lifetime = Mathf.Max(0.01f, lifetime);
        _age = 0f;

        Color c = tint;
        c.a = startAlpha;
        _sr.color = c;

        _sr.enabled = true;
        _active = true;
    }

    private void Update()
    {
        if (!_active) return;

        _age += Time.deltaTime;
        float t = _age / _lifetime;
        if (t >= 1f) { _active = false; _sr.enabled = false; return; }

        Color c = _sr.color;
        c.a = Mathf.Lerp(_startAlpha, 0f, t);
        _sr.color = c;

        transform.localScale = Vector3.Lerp(_startScale, _endScale, t);
    }
}
