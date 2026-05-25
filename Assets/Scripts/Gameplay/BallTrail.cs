using UnityEngine;

/// <summary>
/// Spawns a pooled trail of fading "ghost key" fragments behind the ball while it
/// moves. Built once at Start (no runtime instantiation), so it's WebGL-friendly.
/// Self-contained: reads the ball's own SpriteRenderer (current morphed key) and
/// Rigidbody2D (to only emit while moving). Just add this component to the Ball.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BallTrail : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private int sortingOrder = -1;   // behind the ball (which is 0)

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 0.04f;  // seconds between fragments
    [SerializeField] private float minSpeedToSpawn = 0.1f; // don't emit while serving

    [Header("Fragment Look")]
    [SerializeField, Range(0f, 1f)] private float startAlpha = 0.45f;
    [SerializeField] private float lifetime = 0.35f;
    [SerializeField] private float startScaleMul = 0.9f;   // x ball scale
    [SerializeField] private float endScaleMul = 0.4f;     // shrinks to this
    [SerializeField] private Color tint = Color.white;

    private SpriteRenderer _ballSr;
    private Rigidbody2D _rb;
    private TrailGhost[] _pool;
    private int _next;
    private float _timer;

    private void Start()
    {
        _ballSr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        var container = new GameObject("BallTrailPool");
        _pool = new TrailGhost[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"TrailGhost_{i}");
            go.transform.SetParent(container.transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = sortingOrder;
            _pool[i] = go.AddComponent<TrailGhost>();
        }
    }

    private void Update()
    {
        // Only emit while the ball is actually moving (skips the serve delay).
        if (_rb != null && _rb.linearVelocity.magnitude < minSpeedToSpawn) return;

        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;

        if (_ballSr.sprite == null) return;

        TrailGhost g = _pool[_next];
        _next = (_next + 1) % _pool.Length;
        g.Spawn(_ballSr.sprite,
                transform.position,
                transform.localScale * startScaleMul,
                transform.localScale * endScaleMul,
                tint, startAlpha, lifetime);
    }
}
