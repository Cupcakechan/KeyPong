using UnityEngine;

/// <summary>
/// Central audio for Key Pong. Persists across scenes (DontDestroyOnLoad singleton).
/// Any clip left unassigned is simply skipped.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips")]
    [SerializeField] private AudioClip[] clackClips;     // the 6 typing sounds (random)
    [SerializeField] private AudioClip[] uiClickClips;   // button-press typing sounds
    [SerializeField] private AudioClip scoreClip;        // per-point chime
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.4f;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;      // one-shots (can overlap)
    [SerializeField] private AudioSource musicSource;    // looping

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayClack()   => PlayRandom(clackClips, Random.Range(0.94f, 1.06f));
    public void PlayUIClick() => PlayRandom(uiClickClips, 1f);
    public void PlayScore()   => PlayOne(scoreClip);
    public void PlayWin()     => PlayOne(winClip);
    public void PlayLose()    => PlayOne(loseClip);

    // --- Music control -------------------------------------------------------
    public void PauseMusic()  { if (musicSource != null) musicSource.Pause(); }
    public void ResumeMusic() { if (musicSource != null) musicSource.UnPause(); }

    private void PlayRandom(AudioClip[] clips, float pitch)
    {
        if (sfxSource == null || clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void PlayOne(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
