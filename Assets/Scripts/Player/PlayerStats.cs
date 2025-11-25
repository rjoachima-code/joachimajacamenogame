using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour, ISaveable
{
    public static PlayerStats Instance { get; private set; }

    [Header("Stats")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float maxStress = 100f;
    [SerializeField] private int maxExperience = 1000;

    [Header("Events")]
    public UnityEvent<float> onHungerChanged;
    public UnityEvent<float> onEnergyChanged;
    public UnityEvent<float> onStressChanged;
    public UnityEvent<int> onMoneyChanged;
    public UnityEvent<int> onExperienceChanged;

    private float currentHunger;
    private float currentEnergy;
    private float currentStress;
    private int currentMoney;
    private int currentExperience;

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

        currentHunger = maxHunger;
        currentEnergy = maxEnergy;
        currentStress = 0f;
        currentMoney = 100;
        currentExperience = 0;

        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    public void ModifyHunger(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
        onHungerChanged?.Invoke(currentHunger);
    }

    public void ModifyEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
        onEnergyChanged?.Invoke(currentEnergy);
    }

    public void ModifyStress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0f, maxStress);
        onStressChanged?.Invoke(currentStress);
    }

    public void ModifyMoney(int amount)
    {
        currentMoney += amount;
        onMoneyChanged?.Invoke(currentMoney);
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        onExperienceChanged?.Invoke(currentExperience);
    }

    public float GetHunger() => currentHunger;
    public float GetEnergy() => currentEnergy;
    public float GetStress() => currentStress;
    public int GetMoney() => currentMoney;
    public int GetExperience() => currentExperience;

    [System.Serializable]
    private struct PlayerStatsData
    {
        public float currentHunger;
        public float currentEnergy;
        public float currentStress;
        public int currentMoney;
        public int currentExperience;
    }

    public string SaveData()
    {
        var data = new PlayerStatsData
        {
            currentHunger = this.currentHunger,
            currentEnergy = this.currentEnergy,
            currentStress = this.currentStress,
            currentMoney = this.currentMoney,
            currentExperience = this.currentExperience
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<PlayerStatsData>(state);
        this.currentHunger = data.currentHunger;
        this.currentEnergy = data.currentEnergy;
        this.currentStress = data.currentStress;
        this.currentMoney = data.currentMoney;
        this.currentExperience = data.currentExperience;

        onHungerChanged?.Invoke(currentHunger);
        onEnergyChanged?.Invoke(currentEnergy);
        onStressChanged?.Invoke(currentStress);
        onMoneyChanged?.Invoke(currentMoney);
        onExperienceChanged?.Invoke(currentExperience);
    }
}
