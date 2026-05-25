using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a score (0-99; we only need 0-11) using the digit key sprites from a
/// KeySpriteSet. The tens Image is hidden for single-digit scores.
/// Turn on "Use Tan Digits" for the AI score so it reads in tan/rose.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private KeySpriteSet keySet;
    [SerializeField] private Image tensImage;
    [SerializeField] private Image onesImage;

    [Tooltip("Use the tan/rose digit set instead of the dark one (turn ON for the AI score).")]
    [SerializeField] private bool useTanDigits = false;

    public void SetScore(int value)
    {
        if (keySet == null || onesImage == null) return;

        value = Mathf.Clamp(value, 0, 99);
        int tens = value / 10;
        int ones = value % 10;

        if (tensImage != null)
        {
            bool showTens = tens > 0;
            tensImage.enabled = showTens;
            if (showTens) tensImage.sprite = keySet.GetDigit(tens, useTanDigits);
        }

        onesImage.enabled = true;
        onesImage.sprite = keySet.GetDigit(ones, useTanDigits);
    }
}
