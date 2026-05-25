using UnityEngine;

/// <summary>
/// End-of-match celebration VFX (world space, pooled, WebGL-light). Built once at
/// Awake. Renders below the Overlay UI so the result text stays on top.
///   PlayWin()  -> a fountain burst of key sprites from center.
///   PlayLose() -> a dark dim fades in over the court + a few keys drift down.
/// All motion uses unscaled time (the game is frozen when these fire).
/// </summary>
public class EndGameVFX : MonoBehaviour
{
    public static EndGameVFX Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private KeySpriteSet keySet;

    [Header("Pool / Layering")]
    [SerializeField] private int poolSize = 32;
    [SerializeField] private int pieceSortingOrder = 50;   // above court, below Overlay UI
    [SerializeField] private int dimSortingOrder = 40;     // above court, below the pieces
    [SerializeField] private float pieceScale = 0.45f;

    [Header("Win Burst")]
    [SerializeField] private int burstCount = 24;
    [SerializeField] private float burstMinSpeed = 4f;
    [SerializeField] private float burstMaxSpeed = 9f;
    [SerializeField] private float burstUpwardBias = 2f;
    [SerializeField] private float burstGravity = 9f;
    [SerializeField] private float burstLifetime = 1.3f;

    [Header("Lose Effect")]
    [SerializeField] private int fallCount = 6;
    [SerializeField] private float fallSpeed = 1.5f;
    [SerializeField] private float fallLifetime = 2.5f;
    [SerializeField] private float dimTargetAlpha = 0.6f;
    [SerializeField] private float dimFadeSpeed = 2f;

    private BurstPiece[] _pool;
    private int _next;
    private SpriteRenderer _dim;
    private float _dimAlpha;
    private float _dimTarget;

    private static Sprite _whiteSprite;

    private void Awake()
    {
        Instance = this;
        BuildPool();
        BuildDim();
    }

    private void Update()
    {
        if (_dim == null) return;
        _dimAlpha = Mathf.MoveTowards(_dimAlpha, _dimTarget, dimFadeSpeed * Time.unscaledDeltaTime);
        Color c = _dim.color; c.a = _dimAlpha; _dim.color = c;
    }

    // --- Public triggers -----------------------------------------------------

    public void PlayWin()
    {
        if (keySet == null) return;
        for (int i = 0; i < burstCount; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            if (dir == Vector2.zero) dir = Vector2.up;
            float speed = Random.Range(burstMinSpeed, burstMaxSpeed);
            Vector3 vel = (Vector3)(dir * speed) + Vector3.up * burstUpwardBias;
            Next().Launch(keySet.GetRandomBallKey(null), Vector3.zero, vel,
                          Random.Range(-360f, 360f), burstGravity, 0.5f,
                          Vector3.one * pieceScale, Vector3.one * pieceScale * 0.7f,
                          Color.white, 1f, burstLifetime * Random.Range(0.85f, 1.15f));
        }
    }

    public void PlayLose()
    {
        _dimTarget = dimTargetAlpha;   // dim fades in (Update drives it)
        if (keySet == null) return;

        float halfH = ViewHalfHeight();
        float halfW = halfH * ViewAspect();
        for (int i = 0; i < fallCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-halfW, halfW), halfH + Random.Range(0f, 2f), 0f);
            Vector3 vel = new Vector3(Random.Range(-0.3f, 0.3f), -fallSpeed, 0f);
            Next().Launch(keySet.GetRandomBallKey(null), pos, vel,
                          Random.Range(-60f, 60f), 0f, 0f,
                          Vector3.one * pieceScale, Vector3.one * pieceScale,
                          Color.white, 0.85f, fallLifetime);
        }
    }

    // --- Setup helpers -------------------------------------------------------

    private BurstPiece Next()
    {
        BurstPiece p = _pool[_next];
        _next = (_next + 1) % _pool.Length;
        return p;
    }

    private void BuildPool()
    {
        var container = new GameObject("EndVFXPool");
        container.transform.SetParent(transform, false);
        _pool = new BurstPiece[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"VFXPiece_{i}");
            go.transform.SetParent(container.transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = pieceSortingOrder;
            _pool[i] = go.AddComponent<BurstPiece>();
        }
    }

    private void BuildDim()
    {
        var go = new GameObject("LoseDim");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
        _dim = go.AddComponent<SpriteRenderer>();
        _dim.sprite = WhiteSprite();
        _dim.color = new Color(0f, 0f, 0f, 0f);
        _dim.sortingOrder = dimSortingOrder;

        float fullH = ViewHalfHeight() * 2f;
        float fullW = fullH * ViewAspect();
        go.transform.localScale = new Vector3(fullW * 1.25f, fullH * 1.25f, 1f);
    }

    private static float ViewHalfHeight()
    {
        Camera cam = Camera.main;
        return (cam != null) ? cam.orthographicSize : 5f;
    }

    private static float ViewAspect()
    {
        Camera cam = Camera.main;
        return (cam != null && cam.aspect > 0f) ? cam.aspect : (16f / 9f);
    }

    private static Sprite WhiteSprite()
    {
        if (_whiteSprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
        return _whiteSprite;
    }
}
