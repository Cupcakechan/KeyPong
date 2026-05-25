using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves the player's (left) paddle vertically using the New Input System.
/// W / Up Arrow = up, S / Down Arrow = down. Movement is clamped inside the court.
///
/// The Move action and its bindings are defined in code so this script is fully
/// self-contained — no .inputactions asset to wire up.
/// </summary>
public class PlayerPaddle : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Paddle speed in world units per second.")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Court Clamp (paddle CENTER y limits)")]
    [SerializeField] private float topLimit = 4.0f;
    [SerializeField] private float bottomLimit = -4.0f;

    private InputAction _moveAction;

    private void Awake()
    {
        // A 1D axis: -1 (down) .. 0 .. +1 (up). Two composites = W/S and arrow keys.
        _moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Axis");

        _moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/s")
            .With("Positive", "<Keyboard>/w");

        _moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/downArrow")
            .With("Positive", "<Keyboard>/upArrow");
    }

    private void OnEnable()  => _moveAction.Enable();
    private void OnDisable() => _moveAction.Disable();

    private void Update()
    {
        float input = _moveAction.ReadValue<float>();   // -1, 0, or +1
        if (Mathf.Approximately(input, 0f)) return;

        Vector3 pos = transform.position;
        pos.y += input * moveSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, bottomLimit, topLimit);
        transform.position = pos;
    }
}
