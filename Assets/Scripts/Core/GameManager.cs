using UnityEngine;

/// <summary>
/// Tracks the match score, updates the on-screen score displays, and resets the ball
/// after each point. Goal zones call PlayerScored() / AIScored().
/// (Win detection + Game Over panel are added in the next step.)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Ball ball;

    [Header("Score Display")]
    [SerializeField] private ScoreDisplay playerScoreDisplay;
    [SerializeField] private ScoreDisplay aiScoreDisplay;

    [Header("Rules")]
    [SerializeField] private int winScore = 11;

    public int PlayerScore { get; private set; }
    public int AIScore { get; private set; }

    private void Awake() => Instance = this;

    private void Start()
    {
        if (ball == null) ball = FindAnyObjectByType<Ball>();
        PlayerScore = 0;
        AIScore = 0;
        UpdateDisplays();
    }

    public void PlayerScored()
    {
        PlayerScore++;
        AfterScore();
    }

    public void AIScored()
    {
        AIScore++;
        AfterScore();
    }

    private void AfterScore()
    {
        UpdateDisplays();
        // TODO (next step): if PlayerScore >= winScore or AIScore >= winScore -> Game Over.
        if (ball != null) ball.ResetAndServe();
    }

    private void UpdateDisplays()
    {
        if (playerScoreDisplay != null) playerScoreDisplay.SetScore(PlayerScore);
        if (aiScoreDisplay != null) aiScoreDisplay.SetScore(AIScore);
    }
}
