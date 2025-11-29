using System;
using UnityEngine;

/// <summary>
/// Manager for district-specific ambient music.
/// </summary>
public class AmbientMusicManager : MonoBehaviour
{
    public static AmbientMusicManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Settings")]
    [SerializeField] private float crossfadeDuration = 2f;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float ambientVolume = 0.3f;

    [Header("Default Tracks")]
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip defaultAmbient;

    public event Action<AudioClip> OnMusicChanged;

    private AudioClip currentMusic;
    private bool isCrossfading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged += OnDistrictChanged;
            DistrictManager.Instance.OnDistrictDataLoaded += OnDistrictDataLoaded;
        }

        // Play default music
        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    private void OnDestroy()
    {
        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged -= OnDistrictChanged;
            DistrictManager.Instance.OnDistrictDataLoaded -= OnDistrictDataLoaded;
        }
    }

    private void OnDistrictChanged(DistrictType district)
    {
        // Music change will be handled by OnDistrictDataLoaded
    }

    private void OnDistrictDataLoaded(DistrictData districtData)
    {
        if (districtData != null && districtData.ambientMusic != null)
        {
            PlayMusic(districtData.ambientMusic);
        }

        if (districtData != null && districtData.ambientSounds != null && districtData.ambientSounds.Length > 0)
        {
            PlayAmbient(districtData.ambientSounds[0]);
        }
    }

    /// <summary>
    /// Plays district-specific music.
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || clip == currentMusic) return;
        if (isCrossfading) return;

        currentMusic = clip;

        if (musicSource != null)
        {
            StartCoroutine(CrossfadeMusic(clip));
        }

        OnMusicChanged?.Invoke(clip);
    }

    private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        isCrossfading = true;

        // Fade out current
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < crossfadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (crossfadeDuration / 2f));
            yield return null;
        }

        // Switch clip
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new
        elapsed = 0f;
        while (elapsed < crossfadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / (crossfadeDuration / 2f));
            yield return null;
        }

        musicSource.volume = musicVolume;
        isCrossfading = false;
    }

    /// <summary>
    /// Plays ambient sounds.
    /// </summary>
    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource == null) return;

        ambientSource.clip = clip;
        ambientSource.volume = ambientVolume;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    /// <summary>
    /// Stops all music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        currentMusic = null;
    }

    /// <summary>
    /// Stops ambient sounds.
    /// </summary>
    public void StopAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }
    }

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null && !isCrossfading)
        {
            musicSource.volume = musicVolume;
        }
    }

    /// <summary>
    /// Sets the ambient volume.
    /// </summary>
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume;
        }
    }

    /// <summary>
    /// Pauses all audio.
    /// </summary>
    public void PauseAll()
    {
        if (musicSource != null) musicSource.Pause();
        if (ambientSource != null) ambientSource.Pause();
    }

    /// <summary>
    /// Resumes all audio.
    /// </summary>
    public void ResumeAll()
    {
        if (musicSource != null) musicSource.UnPause();
        if (ambientSource != null) ambientSource.UnPause();
    }

    /// <summary>
    /// Gets the current music clip.
    /// </summary>
    public AudioClip GetCurrentMusic()
    {
        return currentMusic;
    }
}
