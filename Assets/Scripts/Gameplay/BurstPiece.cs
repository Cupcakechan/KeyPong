using UnityEngine;

/// <summary>
/// One reusable end-game VFX piece (a key sprite). Driven by simple motion:
/// velocity + gravity + drag + spin, holding full alpha then fading out over the
/// tail of its life. Uses UNSCALED time because the game is frozen (timeScale 0)
/// when win/lose fires. Deactivates itself when done so the pool can reuse it.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BurstPiece : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Vector3 _velocity;
    private float _angularVel;
    private float _gravity;
    private float _drag;
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

    public void Launch(Sprite sprite, Vector3 pos, Vector3 velocity, float angularVel,
                       float gravity, float drag, Vector3 startScale, Vector3 endScale,
                       Color tint, float startAlpha, float lifetime)
    {
        _sr.sprite = sprite;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        _velocity = velocity;
        _angularVel = angularVel;
        _gravity = gravity;
        _drag = drag;
        _startScale = startScale;
        _endScale = endScale;
        transform.localScale = startScale;
        _startAlpha = startAlpha;
        _lifetime = Mathf.Max(0.01f, lifetime);
        _age = 0f;

        Color c = tint; c.a = startAlpha;
        _sr.color = c;
        _sr.enabled = true;
        _active = true;
    }

    private void Update()
    {
        if (!_active) return;

        float dt = Time.unscaledDeltaTime;
        _age += dt;
        float t = _age / _lifetime;
        if (t >= 1f) { _active = false; _sr.enabled = false; return; }

        _velocity.y -= _gravity * dt;
        _velocity *= Mathf.Clamp01(1f - _drag * dt);
        transform.position += _velocity * dt;
        transform.Rotate(0f, 0f, _angularVel * dt);

        // Hold full alpha for the first 60%, fade out over the last 40%.
        Color c = _sr.color;
        c.a = (t > 0.6f) ? Mathf.Lerp(_startAlpha, 0f, (t - 0.6f) / 0.4f) : _startAlpha;
        _sr.color = c;

        transform.localScale = Vector3.Lerp(_startScale, _endScale, t);
    }
}
