using UnityEngine;

/// <summary>
/// Simple AI for the right paddle: it tracks the ball's vertical position, moving
/// toward it at a capped speed (a touch slower than the player, for fairness) with a
/// small dead zone so it doesn't jitter when it's already lined up. MoveTowards keeps
/// it from overshooting.
/// </summary>
public class AIPaddle : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The ball to track. Leave empty to auto-find at start.")]
    [SerializeField] private Transform ball;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6.5f;   // slower than the player's 8 = beatable
    [SerializeField] private float deadZone = 0.3f;    // ignore tiny gaps to avoid jitter

    [Header("Court Clamp (paddle CENTER y limits)")]
    [SerializeField] private float topLimit = 4.0f;
    [SerializeField] private float bottomLimit = -4.0f;

    private void Start()
    {
        if (ball == null)
        {
            Ball b = FindAnyObjectByType<Ball>();
            if (b != null) ball = b.transform;
        }
    }

    private void Update()
    {
        if (ball == null) return;

        // Aim for the ball's height, but never past our own court limits.
        float targetY = Mathf.Clamp(ball.position.y, bottomLimit, topLimit);

        // Close enough? Hold still (prevents twitchy back-and-forth).
        if (Mathf.Abs(targetY - transform.position.y) <= deadZone) return;

        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = pos;
    }
}
