using UnityEngine;

/// <summary>
/// Persistent Time Attack high score, stored in PlayerPrefs so it survives between
/// sessions and (in a WebGL build) across page reloads in the same browser.
/// </summary>
public static class TimeAttackData
{
    private const string Key = "KeyPong_TimeAttack_Best";

    public static int Best => PlayerPrefs.GetInt(Key, 0);

    /// <summary>Saves the score if it beats the stored best. Returns true on a new best.</summary>
    public static bool TrySetBest(int score)
    {
        if (score > Best)
        {
            PlayerPrefs.SetInt(Key, score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
