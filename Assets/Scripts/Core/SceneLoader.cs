using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralized scene transitions for Key Pong. Each load resets Time.timeScale to 1
/// and resumes music (in case the match ended with music paused), so a paused/frozen
/// state never carries into the next scene.
/// </summary>
public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";

    public static void LoadMainMenu()
    {
        PrepareTransition();
        SceneManager.LoadScene(MainMenu);
    }

    public static void LoadGameplay()
    {
        PrepareTransition();
        SceneManager.LoadScene(Gameplay);
    }

    public static void ReloadCurrent()
    {
        PrepareTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    private static void PrepareTransition()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.ResumeMusic();
    }
}
