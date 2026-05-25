using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pause menu controller. Toggled by Spacebar (or Esc). Freezes the match with
/// Time.timeScale = 0 and shows an overlay with Resume / Restart / Main Menu.
/// Pausing is ignored once the match is over (the Game Over panel owns the freeze).
///
/// No on-screen pause button on purpose: clicking mid-rally pulls focus off the
/// paddle and can cost the player a point. Spacebar is also WebGL-reliable.
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
        // Update still runs while timeScale = 0, so the keys work both ways.
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
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