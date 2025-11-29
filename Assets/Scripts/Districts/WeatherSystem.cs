using System;
using UnityEngine;

/// <summary>
/// System for managing weather effects (Rain, Sunny, Fog).
/// </summary>
public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem Instance { get; private set; }

    [Header("Current Weather")]
    [SerializeField] private WeatherType currentWeather = WeatherType.Sunny;

    [Header("Weather Effects")]
    [SerializeField] private ParticleSystem rainEffect;
    [SerializeField] private ParticleSystem heavyRainEffect;
    [SerializeField] private GameObject fogEffect;

    [Header("Weather Settings")]
    [SerializeField] private float weatherCheckIntervalHours = 1f;
    [SerializeField] private float baseRainChance = 0.3f;
    [SerializeField] private float baseFogChance = 0.1f;

    [Header("Lighting Adjustments")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float sunnyIntensity = 1.0f;
    [SerializeField] private float cloudyIntensity = 0.7f;
    [SerializeField] private float rainyIntensity = 0.5f;
    [SerializeField] private float foggyIntensity = 0.4f;

    public event Action<WeatherType> OnWeatherChanged;

    private float lastWeatherCheckHour = -1f;

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
        ApplyWeatherEffects();

        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick += OnTimeTick;
        }

        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged += OnDistrictChanged;
        }
    }

    private void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick -= OnTimeTick;
        }

        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged -= OnDistrictChanged;
        }
    }

    private void OnTimeTick(int hour, int minute)
    {
        float currentHour = hour + minute / 60f;
        if (lastWeatherCheckHour < 0 || currentHour - lastWeatherCheckHour >= weatherCheckIntervalHours)
        {
            lastWeatherCheckHour = currentHour;
            CheckWeatherChange();
        }
    }

    private void OnDistrictChanged(DistrictType district)
    {
        // District change may affect weather probabilities
        CheckWeatherChange();
    }

    private void CheckWeatherChange()
    {
        float rainChance = baseRainChance;
        float fogChance = baseFogChance;

        // Adjust based on current district
        if (DistrictManager.Instance != null)
        {
            var districtData = DistrictManager.Instance.GetCurrentDistrictData();
            if (districtData != null)
            {
                rainChance = districtData.rainChance;
                fogChance = districtData.fogChance;
            }
        }

        // Determine new weather
        float roll = UnityEngine.Random.value;

        if (roll < rainChance * 0.3f)
        {
            SetWeather(WeatherType.HeavyRain);
        }
        else if (roll < rainChance)
        {
            SetWeather(WeatherType.Rain);
        }
        else if (roll < rainChance + fogChance)
        {
            SetWeather(WeatherType.Fog);
        }
        else if (roll < rainChance + fogChance + 0.2f)
        {
            SetWeather(WeatherType.Cloudy);
        }
        else
        {
            SetWeather(WeatherType.Sunny);
        }
    }

    /// <summary>
    /// Gets the current weather type.
    /// </summary>
    public WeatherType GetCurrentWeather()
    {
        return currentWeather;
    }

    /// <summary>
    /// Sets the weather to a specific type.
    /// </summary>
    public void SetWeather(WeatherType weather)
    {
        if (currentWeather == weather) return;

        currentWeather = weather;
        ApplyWeatherEffects();
        OnWeatherChanged?.Invoke(currentWeather);
    }

    private void ApplyWeatherEffects()
    {
        // Disable all effects first
        if (rainEffect != null) rainEffect.Stop();
        if (heavyRainEffect != null) heavyRainEffect.Stop();
        if (fogEffect != null) fogEffect.SetActive(false);

        // Apply appropriate effects
        switch (currentWeather)
        {
            case WeatherType.Sunny:
            case WeatherType.Clear:
                SetLightIntensity(sunnyIntensity);
                break;

            case WeatherType.Cloudy:
                SetLightIntensity(cloudyIntensity);
                break;

            case WeatherType.Rain:
                if (rainEffect != null) rainEffect.Play();
                SetLightIntensity(rainyIntensity);
                break;

            case WeatherType.HeavyRain:
                if (heavyRainEffect != null) heavyRainEffect.Play();
                SetLightIntensity(rainyIntensity * 0.8f);
                break;

            case WeatherType.Fog:
                if (fogEffect != null) fogEffect.SetActive(true);
                SetLightIntensity(foggyIntensity);
                break;
        }
    }

    private void SetLightIntensity(float intensity)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = intensity;
        }
    }

    /// <summary>
    /// Checks if it's currently raining.
    /// </summary>
    public bool IsRaining()
    {
        return currentWeather == WeatherType.Rain || currentWeather == WeatherType.HeavyRain;
    }

    /// <summary>
    /// Checks if it's currently foggy.
    /// </summary>
    public bool IsFoggy()
    {
        return currentWeather == WeatherType.Fog;
    }

    /// <summary>
    /// Gets a weather description string.
    /// </summary>
    public string GetWeatherDescription()
    {
        switch (currentWeather)
        {
            case WeatherType.Sunny: return "Sunny";
            case WeatherType.Cloudy: return "Cloudy";
            case WeatherType.Rain: return "Rainy";
            case WeatherType.HeavyRain: return "Heavy Rain";
            case WeatherType.Fog: return "Foggy";
            case WeatherType.Clear: return "Clear";
            default: return "Unknown";
        }
    }
}
