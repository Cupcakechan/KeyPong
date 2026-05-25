using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pause menu controller. Toggled by Esc or an on-screen button. Freezes the match
/// with Time.timeScale = 0 and shows an overlay with Resume / Restart / Main Menu.
/// Pausing is ignored once the match is over (the Game Over panel owns the freeze).
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool _isPaused;

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        _isPaused = false;
    }

    private void Update()
    {
        // Update still runs while timeScale = 0, so Esc works both ways.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (MatchIsOver()) return;
        if (_isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (MatchIsOver()) return;
        _isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void Restart()      => SceneLoader.ReloadCurrent();   // resets timeScale to 1
    public void GoToMainMenu() => SceneLoader.LoadMainMenu();    // resets timeScale to 1

    private bool MatchIsOver()
        => GameManager.Instance != null && GameManager.Instance.IsMatchOver;
}
