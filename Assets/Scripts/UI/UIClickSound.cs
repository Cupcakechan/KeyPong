using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add to any UI Button to play a random typing "click" through the AudioManager
/// when the button is pressed. Uses AudioManager.Instance at click time, so it works
/// regardless of which scene the persistent audio manager came from.
/// </summary>
[RequireComponent(typeof(Button))]
public class UIClickSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();
    }
}
