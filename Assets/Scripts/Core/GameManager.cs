using UnityEngine;
using TMPro;

/// <summary>
/// Tracks the match score, updates the score displays, resets the ball after each
/// point, and ends the match (Game Over panel) when a side reaches the win score.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Ball ball;

    [Header("Score Display")]
    [SerializeField] private ScoreDisplay playerScoreDisplay;
    [SerializeField] private ScoreDisplay aiScoreDisplay;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Rules")]
    [SerializeField] private int winScore = 11;

    public int PlayerScore { get; private set; }
    public int AIScore { get; private set; }

    private void Awake() => Instance = this;

    private void Start()
    {
        Time.timeScale = 1f;                                  // un-freeze on (re)load
        if (ball == null) ball = FindAnyObjectByType<Ball>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        PlayerScore = 0;
        AIScore = 0;
        UpdateDisplays();
    }

    public void PlayerScored() { PlayerScore++; AfterScore(); }
    public void AIScored()     { AIScore++;     AfterScore(); }

    private void AfterScore()
    {
        UpdateDisplays();

        if (PlayerScore >= winScore || AIScore >= winScore)
        {
            EndMatch(PlayerScore >= winScore);
            return;                                           // no re-serve on match point
        }

        if (ball != null) ball.ResetAndServe();
    }

    private void EndMatch(bool playerWon)
    {
        if (resultText != null)     resultText.text = playerWon ? "YOU WIN" : "YOU LOSE";
        if (finalScoreText != null) finalScoreText.text = $"{PlayerScore} - {AIScore}";
        if (gameOverPanel != null)  gameOverPanel.SetActive(true);
        Time.timeScale = 0f;                                  // freeze the match
    }

    private void UpdateDisplays()
    {
        if (playerScoreDisplay != null) playerScoreDisplay.SetScore(PlayerScore);
        if (aiScoreDisplay != null)     aiScoreDisplay.SetScore(AIScore);
    }

    // --- Game Over buttons ---------------------------------------------------
    public void PlayAgain()    => SceneLoader.ReloadCurrent();
    public void GoToMainMenu() => SceneLoader.LoadMainMenu();
}
