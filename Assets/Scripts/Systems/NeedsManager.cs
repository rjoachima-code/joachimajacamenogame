using UnityEngine;
using UnityEngine.Events;

public class NeedsManager : MonoBehaviour
{
    public static NeedsManager Instance { get; private set; }

    [Header("Need Settings")]
    [SerializeField] private float hungerDecayRate = 1f; // Per minute
    [SerializeField] private float energyDecayRate = 0.5f; // Per minute
    [SerializeField] private float stressIncreaseRate = 0.2f; // Per minute

    [Header("Events")]
    public UnityEvent<float> onHungerChanged;
    public UnityEvent<float> onEnergyChanged;
    public UnityEvent<float> onStressChanged;

    private float currentHunger = 100f;
    private float currentEnergy = 100f;
    private float currentStress = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Decay needs over time
        currentHunger = Mathf.Max(0f, currentHunger - hungerDecayRate * Time.deltaTime);
        currentEnergy = Mathf.Max(0f, currentEnergy - energyDecayRate * Time.deltaTime);
        currentStress = Mathf.Min(100f, currentStress + stressIncreaseRate * Time.deltaTime);

        onHungerChanged?.Invoke(currentHunger);
        onEnergyChanged?.Invoke(currentEnergy);
        onStressChanged?.Invoke(currentStress);
    }

    public void EatFood(float hungerRestore)
    {
        currentHunger = Mathf.Min(100f, currentHunger + hungerRestore);
        onHungerChanged?.Invoke(currentHunger);
    }

    public void Sleep(float energyRestore)
    {
        currentEnergy = Mathf.Min(100f, currentEnergy + energyRestore);
        onEnergyChanged?.Invoke(currentEnergy);
    }

    public void ReduceStress(float stressReduction)
    {
        currentStress = Mathf.Max(0f, currentStress - stressReduction);
        onStressChanged?.Invoke(currentStress);
    }

    public float GetHunger()
    {
        return currentHunger;
    }

    public float GetEnergy()
    {
        return currentEnergy;
    }

    public float GetStress()
    {
        return currentStress;
    }
}
