using UnityEngine;

/// <summary>
/// An invisible trigger zone placed just off-screen behind a paddle.
/// When the ball enters it, the chosen side earns a point.
///   - LEFT goal (behind the player paddle)  -> Awards Point To = AI
///   - RIGHT goal (behind the AI paddle)      -> Awards Point To = Player
/// </summary>
public class GoalZone : MonoBehaviour
{
    public enum Side { Player, AI }

    [Tooltip("Which side earns a point when the ball enters this zone.")]
    [SerializeField] private Side awardsPointTo = Side.Player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // React only to the ball.
        if (other.GetComponent<Ball>() == null) return;
        if (GameManager.Instance == null) return;

        if (awardsPointTo == Side.Player) GameManager.Instance.PlayerScored();
        else                              GameManager.Instance.AIScored();
    }
}
