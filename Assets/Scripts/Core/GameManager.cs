using UnityEngine;
using TMPro;

/// <summary>
/// Drives both game modes (chosen via GameSession.Mode):
///   Classic     - first to winScore wins; shows YOU WIN / YOU LOSE.
///   Time Attack - a countdown; score as many points as possible; on time-up it
///                 saves the high score and shows TIME'S UP / NEW BEST!.
/// Both reuse the same Game Over panel (mode-aware text). Pauses music on end so the
/// stinger is audible (SceneLoader resumes it on transition).
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

    [Header("Classic Rules")]
    [SerializeField] private int winScore = 11;

    [Header("Time Attack")]
    [SerializeField] private float timeAttackSeconds = 300f;   // 5:00
    [SerializeField] private TextMeshProUGUI timerText;        // top-center; hidden in Classic

    public int PlayerScore { get; private set; }
    public int AIScore { get; private set; }
    public bool IsMatchOver { get; private set; }

    private GameMode _mode;
    private float _timeRemaining;

    private void Awake() => Instance = this;

    private void Start()
    {
        Time.timeScale = 1f;
        _mode = GameSession.Mode;

        if (ball == null) ball = FindAnyObjectByType<Ball>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        PlayerScore = 0;
        AIScore = 0;
        IsMatchOver = false;
        _timeRemaining = timeAttackSeconds;

        if (timerText != null) timerText.gameObject.SetActive(_mode == GameMode.TimeAttack);
        if (_mode == GameMode.TimeAttack) UpdateTimerText();

        UpdateDisplays();
    }

    private void Update()
    {
        if (_mode != GameMode.TimeAttack || IsMatchOver) return;

        _timeRemaining -= Time.deltaTime;   // freezes while paused (timeScale 0)
        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            UpdateTimerText();
            EndTimeAttack();
            return;
        }
        UpdateTimerText();
    }

    public void PlayerScored() { PlayerScore++; AfterScore(); }
    public void AIScored()     { AIScore++;     AfterScore(); }

    private void AfterScore()
    {
        UpdateDisplays();

        // Classic ends at the win score; Time Attack only ends on the timer.
        if (_mode == GameMode.Classic && (PlayerScore >= winScore || AIScore >= winScore))
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

        Time.timeScale = 0f;
    }

    private void EndTimeAttack()
    {
        IsMatchOver = true;

        bool newBest = TimeAttackData.TrySetBest(PlayerScore);
        if (resultText != null)     resultText.text = newBest ? "NEW BEST!" : "TIME'S UP";
        if (finalScoreText != null) finalScoreText.text = $"Score {PlayerScore}    Best {TimeAttackData.Best}";
        if (gameOverPanel != null)  gameOverPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseMusic();
            if (newBest) AudioManager.Instance.PlayWin();   // celebrate a new record; quiet otherwise
        }

        Time.timeScale = 0f;
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;
        int total = Mathf.CeilToInt(_timeRemaining);
        int m = total / 60;
        int s = total % 60;
        timerText.text = $"{m}:{s:00}";
    }

    private void UpdateDisplays()
    {
        if (playerScoreDisplay != null) playerScoreDisplay.SetScore(PlayerScore);
        if (aiScoreDisplay != null)     aiScoreDisplay.SetScore(AIScore);
    }

    public void PlayAgain()    => SceneLoader.ReloadCurrent();
    public void GoToMainMenu() => SceneLoader.LoadMainMenu();
}
