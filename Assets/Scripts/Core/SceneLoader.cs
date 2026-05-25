using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralized scene transitions for Key Pong. Each load resets Time.timeScale to 1
/// so a paused (game-over) state never carries into the next scene.
/// Scene names must match the files AND the Build Profiles > Scene List exactly.
/// </summary>
public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";

    public static void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenu);
    }

    public static void LoadGameplay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Gameplay);
    }

    public static void ReloadCurrent()
    {
        Time.timeScale = 1f;
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
}
