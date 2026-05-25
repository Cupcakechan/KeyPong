using UnityEngine;

/// <summary>
/// The Key Pong ball. Launches from center, bounces off walls (handled by a bouncy
/// physics material) and off paddles (angle controlled here by the hit position),
/// speeds up on each paddle hit, and morphs into a new random key sprite on EVERY
/// bounce — the game's signature effect.
///
/// NOTE: the "recycle when off-screen" logic is TEMPORARY so we can test the rally
/// before real scoring exists. We'll replace it with scoring in the next step.
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
    [SerializeField] private float recycleX = 9.5f;         // TEMP: reset when past this x

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

    private void Update()
    {
        // TEMPORARY: recycle the ball if it leaves the court (real scoring comes next step).
        if (Mathf.Abs(transform.position.x) > recycleX)
            ResetAndServe();
    }

    private void ResetAndServe()
    {
        CancelInvoke();
        _rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
        Morph();                                  // fresh random key on reset
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

        if (collision.gameObject.CompareTag("Paddle"))
            BounceOffPaddle(collision.transform);
        // "Wall" hits are reflected automatically by the bouncy physics material.
    }

    private void BounceOffPaddle(Transform paddle)
    {
        // World half-height of the paddle (works regardless of rotation/scale).
        float halfHeight = 1f;
        Collider2D col = paddle.GetComponent<Collider2D>();
        if (col != null) halfHeight = Mathf.Max(0.01f, col.bounds.extents.y);

        // Where on the paddle did we hit? -1 (bottom) .. 0 (center) .. +1 (top)
        float offset = Mathf.Clamp((transform.position.y - paddle.position.y) / halfHeight, -1f, 1f);

        // Send the ball away from the paddle it just struck.
        float dirX = Mathf.Sign(transform.position.x - paddle.position.x);
        if (dirX == 0f) dirX = (transform.position.x <= 0f) ? 1f : -1f;

        float angle = offset * maxBounceAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(dirX * Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        _speed = Mathf.Min(_speed + speedIncrease, maxSpeed);
        _rb.linearVelocity = dir * _speed;
    }

    private void Morph()
    {
        if (keySet == null) return;
        _currentKey = keySet.GetRandomBallKey(_currentKey);
        if (_currentKey != null) _sr.sprite = _currentKey;
    }
}
