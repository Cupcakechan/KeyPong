using UnityEngine;

/// <summary>
/// Tracks the match score and resets the ball after each point.
/// Goal zones call PlayerScored() / AIScored() when the ball gets past a paddle.
/// (Win detection + Game Over panel are added in a later step.)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Ball ball;

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
        LogScore();
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
        LogScore();
        // TODO (next step): if PlayerScore >= winScore or AIScore >= winScore -> Game Over.
        if (ball != null) ball.ResetAndServe();
    }

    private void LogScore() => Debug.Log($"SCORE  >  Player {PlayerScore} : {AIScore} AI");
}
