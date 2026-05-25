using UnityEngine;

/// <summary>
/// Handles the Main Menu button actions. Wire each button's OnClick event
/// to these public methods in the Inspector.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("How To Play Panel (assigned in Step 4)")]
    [SerializeField] private GameObject howToPlayPanel;

    private void Start()
    {
        // Ensure the How To Play panel starts hidden (safe even before it's assigned).
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    // --- Button hooks --------------------------------------------------------

    public void PlayGame() => SceneLoader.LoadGameplay();

    public void QuitGame() => SceneLoader.Quit();

    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }
}
