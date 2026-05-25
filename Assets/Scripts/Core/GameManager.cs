using UnityEngine;
using TMPro;

/// <summary>
/// Tracks the match score, updates the score displays, resets the ball after each
/// point, ends the match (Game Over panel) at the win score, and triggers audio +
/// a camera shake on scoring. Pauses the background music on match end so the
/// stinger is clearly audible (SceneLoader resumes it on transition).
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
    public bool IsMatchOver { get; private set; }

    private void Awake() => Instance = this;

    private void Start()
    {
        Time.timeScale = 1f;
        if (ball == null) ball = FindAnyObjectByType<Ball>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        PlayerScore = 0;
        AIScore = 0;
        IsMatchOver = false;
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
            return;
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayScore();
        if (CameraShake.Instance != null) CameraShake.Instance.ShakeScore();
        if (ball != null) ball.ResetAndServe();
    }

    private void EndMatch(bool playerWon)
    {
        IsMatchOver = true;
        if (resultText != null)     resultText.text = playerWon ? "YOU WIN" : "YOU LOSE";
        if (finalScoreText != null) finalScoreText.text = $"{PlayerScore} - {AIScore}";
        if (gameOverPanel != null)  gameOverPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseMusic();
            if (playerWon) AudioManager.Instance.PlayWin();
            else           AudioManager.Instance.PlayLose();
        }

        Time.timeScale = 0f;   // stingers still play (audio ignores timeScale)
    }

    private void UpdateDisplays()
    {
        if (playerScoreDisplay != null) playerScoreDisplay.SetScore(PlayerScore);
        if (aiScoreDisplay != null)     aiScoreDisplay.SetScore(AIScore);
    }

    public void PlayAgain()    => SceneLoader.ReloadCurrent();
    public void GoToMainMenu() => SceneLoader.LoadMainMenu();
}
