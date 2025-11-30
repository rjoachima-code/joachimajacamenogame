using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton AudioManager for managing sound effects and background music.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("SFX Library")]
    [SerializeField] private List<SFXClip> sfxLibrary = new List<SFXClip>();

    [System.Serializable]
    public class SFXClip
    {
        public string name;
        public AudioClip clip;
    }

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
        BuildSFXDictionary();
    }

    private void Start()
    {
        // Start background music if assigned
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    /// <summary>
    /// Initialize audio sources if not assigned in inspector.
    /// </summary>
    private void InitializeAudioSources()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Build dictionary from SFX library for quick lookup.
    /// </summary>
    private void BuildSFXDictionary()
    {
        sfxDictionary.Clear();
        foreach (var sfx in sfxLibrary)
        {
            if (!string.IsNullOrEmpty(sfx.name) && sfx.clip != null)
            {
                sfxDictionary[sfx.name] = sfx.clip;
            }
        }
    }

    /// <summary>
    /// Plays a sound effect by name from the SFX library.
    /// </summary>
    /// <param name="clipName">Name of the clip to play.</param>
    public void PlaySFX(string clipName)
    {
        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX not found: {clipName}");
        }
    }

    /// <summary>
    /// Plays a sound effect clip.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Plays the jump sound effect.
    /// </summary>
    public void PlayJumpSFX()
    {
        if (jumpSFX != null)
        {
            PlaySFX(jumpSFX);
        }
    }

    /// <summary>
    /// Plays background music.
    /// </summary>
    /// <param name="clip">The music clip to play.</param>
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Pauses the currently playing music.
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    /// <summary>
    /// Resumes paused music.
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// Sets the SFX volume.
    /// </summary>
    /// <param name="volume">Volume level (0-1).</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    /// <param name="volume">Volume level (0-1).</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    /// <summary>
    /// Gets the current SFX volume.
    /// </summary>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Gets the current music volume.
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }
}
