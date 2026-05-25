using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TEMPORARY test harness for KeySpriteSet.
/// Put it on a GameObject with a SpriteRenderer, assign a KeySpriteSet, press Play,
/// then tap SPACE to morph the sprite to a new random ball key.
/// Safe to delete once the real Ball script exists.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class KeySpriteSetTester : MonoBehaviour
{
    [SerializeField] private KeySpriteSet keySet;

    private SpriteRenderer _renderer;
    private Sprite _current;

    private void Awake() => _renderer = GetComponent<SpriteRenderer>();

    private void Start()
    {
        if (keySet == null)
        {
            Debug.LogWarning("KeySpriteSetTester: no KeySpriteSet assigned.", this);
            return;
        }

        Debug.Log($"[KeySpriteSet] Ball pool size = {keySet.BallKeyCount}. Press SPACE to morph.");
        Morph();
    }

    private void Update()
    {
        // New Input System: read the keyboard directly for this quick test.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Morph();
    }

    private void Morph()
    {
        if (keySet == null) return;
        _current = keySet.GetRandomBallKey(_current);
        if (_current != null) _renderer.sprite = _current;
    }
}
