using System;
using UnityEngine;

/// <summary>
/// Enhanced city atmosphere system with day/night cycle integration.
/// </summary>
public class CityAtmosphere : MonoBehaviour
{
    public static CityAtmosphere Instance { get; private set; }

    [Header("Lighting")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;

    [Header("Day/Night Colors")]
    [SerializeField] private Gradient ambientColorGradient;
    [SerializeField] private AnimationCurve lightIntensityCurve;
    [SerializeField] private Gradient fogColorGradient;

    [Header("Fog Settings")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private float baseFogDensity = 0.01f;
    [SerializeField] private float nightFogMultiplier = 1.5f;

    [Header("Time Settings")]
    [SerializeField] private float sunriseHour = 6f;
    [SerializeField] private float sunsetHour = 18f;

    [Header("District Modifiers")]
    [SerializeField] private float industrialPollutionFogMultiplier = 1.5f;

    [Header("Skybox Settings")]
    [SerializeField] private string skyboxRotationPropertyName = "_Rotation";

    public event Action<bool> OnDayNightChanged; // true = day, false = night
    public event Action<float> OnTimeOfDayChanged;

    private bool isDay = true;

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
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick += OnTimeTick;
        }

        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged += OnDistrictChanged;
        }

        if (WeatherSystem.Instance != null)
        {
            WeatherSystem.Instance.OnWeatherChanged += OnWeatherChanged;
        }

        UpdateAtmosphere();
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

        if (WeatherSystem.Instance != null)
        {
            WeatherSystem.Instance.OnWeatherChanged -= OnWeatherChanged;
        }
    }

    private void OnTimeTick(int hour, int minute)
    {
        UpdateAtmosphere();
        CheckDayNightTransition(hour);
    }

    private void OnDistrictChanged(DistrictType district)
    {
        UpdateAtmosphere();
    }

    private void OnWeatherChanged(WeatherType weather)
    {
        UpdateAtmosphere();
    }

    private void UpdateAtmosphere()
    {
        float timeOfDay = GetNormalizedTimeOfDay();
        OnTimeOfDayChanged?.Invoke(timeOfDay);

        UpdateLighting(timeOfDay);
        UpdateFog(timeOfDay);
        UpdateSkybox(timeOfDay);
    }

    private float GetNormalizedTimeOfDay()
    {
        if (TimeSystem.Instance == null) return 0.5f;

        float hour = TimeSystem.Instance.Hour + TimeSystem.Instance.Minute / 60f;
        return hour / 24f;
    }

    private void UpdateLighting(float timeOfDay)
    {
        if (directionalLight == null) return;

        // Update light color
        if (ambientColorGradient != null)
        {
            directionalLight.color = ambientColorGradient.Evaluate(timeOfDay);
        }

        // Update light intensity
        if (lightIntensityCurve != null)
        {
            directionalLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);
        }

        // Update light rotation for sun position
        float sunAngle = (timeOfDay * 360f) - 90f;
        directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
    }

    private void UpdateFog(float timeOfDay)
    {
        if (!useFog) return;

        RenderSettings.fog = true;

        // Update fog color
        if (fogColorGradient != null)
        {
            RenderSettings.fogColor = fogColorGradient.Evaluate(timeOfDay);
        }

        // Calculate fog density
        float density = baseFogDensity;

        // Night multiplier
        if (!isDay)
        {
            density *= nightFogMultiplier;
        }

        // District modifier (e.g., more fog in industrial areas)
        if (DistrictManager.Instance != null)
        {
            var currentDistrict = DistrictManager.Instance.GetCurrentDistrict();
            if (currentDistrict == DistrictType.Xero)
            {
                density *= industrialPollutionFogMultiplier;
            }
        }

        // Weather modifier
        if (WeatherSystem.Instance != null)
        {
            if (WeatherSystem.Instance.IsFoggy())
            {
                density *= 3f;
            }
            else if (WeatherSystem.Instance.IsRaining())
            {
                density *= 1.5f;
            }
        }

        RenderSettings.fogDensity = density;
    }

    private void UpdateSkybox(float timeOfDay)
    {
        if (skyboxMaterial == null) return;

        // Rotate skybox
        skyboxMaterial.SetFloat(skyboxRotationPropertyName, timeOfDay * 360f);
    }

    private void CheckDayNightTransition(int hour)
    {
        bool wasDay = isDay;
        isDay = hour >= sunriseHour && hour < sunsetHour;

        if (wasDay != isDay)
        {
            OnDayNightChanged?.Invoke(isDay);
        }
    }

    /// <summary>
    /// Checks if it's currently daytime.
    /// </summary>
    public bool IsDay()
    {
        return isDay;
    }

    /// <summary>
    /// Checks if it's currently nighttime.
    /// </summary>
    public bool IsNight()
    {
        return !isDay;
    }

    /// <summary>
    /// Gets the current time of day (0-1).
    /// </summary>
    public float GetTimeOfDay()
    {
        return GetNormalizedTimeOfDay();
    }

    /// <summary>
    /// Sets the sunrise hour.
    /// </summary>
    public void SetSunriseHour(float hour)
    {
        sunriseHour = Mathf.Clamp(hour, 0f, 12f);
    }

    /// <summary>
    /// Sets the sunset hour.
    /// </summary>
    public void SetSunsetHour(float hour)
    {
        sunsetHour = Mathf.Clamp(hour, 12f, 24f);
    }

    /// <summary>
    /// Enables or disables fog.
    /// </summary>
    public void SetFogEnabled(bool enabled)
    {
        useFog = enabled;
        RenderSettings.fog = enabled;
    }
}
