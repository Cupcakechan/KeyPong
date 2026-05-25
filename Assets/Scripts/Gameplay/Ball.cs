using UnityEngine;

/// <summary>
/// The Key Pong ball. Launches from center, bounces off walls (bouncy physics
/// material) and paddles (angle controlled here by hit position), speeds up on each
/// paddle hit, and morphs into a new random key sprite on EVERY bounce. Plays a
/// random keyboard "clack" on each bounce and a subtle camera shake on paddle hits.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private KeySpriteSet keySet;

    [Header("Speed (world units / sec)")]
    [SerializeField] private float launchSpeed = 6f;
    [SerializeField] private float speedIncrease = 0.25f;   // added on each paddle hit
    [SerializeField] private float maxSpeed = 14f;

    [Header("Serve & Bounce")]
    [SerializeField] private float launchDelay = 1.0f;
    [SerializeField] private float maxBounceAngle = 45f;    // degrees from horizontal

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Sprite _currentKey;
    private float _speed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start() => ResetAndServe();

    /// <summary>Center the ball, pick a fresh key, and serve after a short delay.</summary>
    public void ResetAndServe()
    {
        CancelInvoke();
        _rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
        Morph();
        _speed = launchSpeed;
        Invoke(nameof(Launch), launchDelay);
    }

    private void Launch()
    {
        float dirX = (Random.value < 0.5f) ? -1f : 1f;
        float angle = Random.Range(-maxBounceAngle, maxBounceAngle) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(dirX * Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        _rb.linearVelocity = dir * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Morph();   // every bounce changes the key
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClack();

        if (collision.gameObject.CompareTag("Paddle"))
            BounceOffPaddle(collision.transform);
        // "Wall" hits are reflected automatically by the bouncy physics material.
    }

    private void BounceOffPaddle(Transform paddle)
    {
        float halfHeight = 1f;
        Collider2D col = paddle.GetComponent<Collider2D>();
        if (col != null) halfHeight = Mathf.Max(0.01f, col.bounds.extents.y);

        float offset = Mathf.Clamp((transform.position.y - paddle.position.y) / halfHeight, -1f, 1f);

        float dirX = Mathf.Sign(transform.position.x - paddle.position.x);
        if (dirX == 0f) dirX = (transform.position.x <= 0f) ? 1f : -1f;

        float angle = offset * maxBounceAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(dirX * Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        _speed = Mathf.Min(_speed + speedIncrease, maxSpeed);
        _rb.linearVelocity = dir * _speed;

        if (CameraShake.Instance != null) CameraShake.Instance.ShakeHit();
    }

    private void Morph()
    {
        if (keySet == null) return;
        _currentKey = keySet.GetRandomBallKey(_currentKey);
        if (_currentKey != null) _sr.sprite = _currentKey;
    }
}
