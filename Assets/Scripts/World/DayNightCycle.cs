using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Gradient lightColorGradient;
    [SerializeField] private AnimationCurve lightIntensityCurve;

    [Header("Skybox Settings")]
    [SerializeField] private float skyboxRotationSpeed = 1f;

    private void Update()
    {
        float timeOfDay = TimeManager.Instance.GetCurrentTime() / 24f; // Normalize to 0-1

        // Update light color and intensity
        directionalLight.color = lightColorGradient.Evaluate(timeOfDay);
        directionalLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);

        // Update light rotation for day/night
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((timeOfDay * 360f) - 90f, 170f, 0f));

        // Update skybox rotation
        if (skyboxMaterial != null)
        {
            float rotation = timeOfDay * 360f * skyboxRotationSpeed;
            skyboxMaterial.SetFloat("_Rotation", rotation);
        }
    }
}
