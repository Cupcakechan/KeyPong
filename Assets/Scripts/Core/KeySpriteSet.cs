using UnityEngine;

/// <summary>
/// Central library of keyboard-key sprites used across Key Pong:
/// score digits (dark AND tan/rose sets), the two paddle (SPACE) sprites, and the
/// pool the ball randomly morphs through on every hit.
///
/// Create an asset instance via:  Assets > Create > Key Pong > Key Sprite Set.
/// </summary>
[CreateAssetMenu(fileName = "MainKeySet", menuName = "Key Pong/Key Sprite Set")]
public class KeySpriteSet : ScriptableObject
{
    [Header("Score Digits — DARK  (Element 0 = '0', ... Element 9 = '9')")]
    [SerializeField] private Sprite[] digits = new Sprite[10];

    [Header("Score Digits — TAN / ROSE  (Element 0 = '0', ... Element 9 = '9')")]
    [SerializeField] private Sprite[] tanDigits = new Sprite[10];

    [Header("Paddles")]
    [SerializeField] private Sprite spacePlayer;   // dark SPACE key
    [SerializeField] private Sprite spaceAI;       // tan/rose SPACE key

    [Header("Ball Morph Pool — drag a variety of standard keys here")]
    [SerializeField] private Sprite[] ballKeys;

    // --- Read-only accessors -------------------------------------------------
    public Sprite SpacePlayer => spacePlayer;
    public Sprite SpaceAI => spaceAI;
    public int BallKeyCount => ballKeys != null ? ballKeys.Length : 0;

    /// <summary>
    /// Returns the sprite for a single digit 0-9. Pass tan = true for the tan/rose set.
    /// Falls back to the dark digit if a tan slot is empty.
    /// </summary>
    public Sprite GetDigit(int digit, bool tan = false)
    {
        if (digit < 0 || digit > 9) return null;

        Sprite[] set = tan ? tanDigits : digits;
        Sprite result = (set != null && digit < set.Length) ? set[digit] : null;

        if (result == null && tan && digits != null && digit < digits.Length)
            result = digits[digit];   // graceful fallback to dark

        return result;
    }

    /// <summary>
    /// Returns a random ball key, guaranteed different from <paramref name="avoid"/>
    /// when the pool has 2+ entries — so the ball visibly changes on every hit.
    /// </summary>
    public Sprite GetRandomBallKey(Sprite avoid = null)
    {
        if (ballKeys == null || ballKeys.Length == 0) return null;
        if (ballKeys.Length == 1) return ballKeys[0];

        Sprite next;
        int safety = 0;
        do
        {
            next = ballKeys[Random.Range(0, ballKeys.Length)];
            safety++;
        }
        while (next == avoid && safety < 32);

        return next;
    }
}
