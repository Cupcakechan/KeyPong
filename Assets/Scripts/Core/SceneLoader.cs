using UnityEngine.SceneManagement;

/// <summary>
/// Centralized scene transitions for Key Pong. Call from any script.
/// The scene name constants below must match the scene file names AND the
/// entries in File > Build Profiles > Scene List exactly.
/// </summary>
public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";

    public static void LoadMainMenu() => SceneManager.LoadScene(MainMenu);

    public static void LoadGameplay() => SceneManager.LoadScene(Gameplay);

    public static void ReloadCurrent() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stop Play mode in the Editor
#else
        UnityEngine.Application.Quit();                  // quit the built app
#endif
    }
}
