using UnityEngine;
using TMPro;

/// <summary>
/// Handles the Main Menu button actions. Wire each button's OnClick event to these
/// public methods in the Inspector. Sets the game mode before loading Gameplay and
/// displays the Time Attack high score.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("How To Play Panel")]
    [SerializeField] private GameObject howToPlayPanel;

    [Header("Time Attack")]
    [SerializeField] private TextMeshProUGUI bestScoreText;   // "TIME ATTACK BEST: 14"

    private void Start()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        RefreshBestScore();
    }

    private void RefreshBestScore()
    {
        if (bestScoreText != null)
            bestScoreText.text = $"TIME ATTACK BEST: {TimeAttackData.Best}";
    }

    // --- Button hooks --------------------------------------------------------

    public void PlayGame()       { GameSession.Mode = GameMode.Classic;    SceneLoader.LoadGameplay(); }
    public void PlayTimeAttack() { GameSession.Mode = GameMode.TimeAttack; SceneLoader.LoadGameplay(); }
    public void QuitGame()       => SceneLoader.Quit();

    public void OpenHowToPlay()  { if (howToPlayPanel != null) howToPlayPanel.SetActive(true); }
    public void CloseHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(false); }
}
