using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider energyBar;
    [SerializeField] private TMP_Text moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to events
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onHungerChanged.AddListener(UpdateHunger);
            PlayerStats.Instance.onEnergyChanged.AddListener(UpdateEnergy);
            PlayerStats.Instance.onMoneyChanged.AddListener(UpdateMoney);
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourPassed.AddListener(UpdateClock);
        }

        // Initial updates
        UpdateHunger(PlayerStats.Instance?.GetHunger() ?? 100f);
        UpdateEnergy(PlayerStats.Instance?.GetEnergy() ?? 100f);
        UpdateMoney(PlayerStats.Instance?.GetMoney() ?? 100);
        UpdateClock(TimeManager.Instance?.GetCurrentHour() ?? 8);
    }

    private void UpdateHunger(float hunger)
    {
        if (hungerBar != null)
        {
            hungerBar.value = hunger / 100f;
        }
    }

    private void UpdateEnergy(float energy)
    {
        if (energyBar != null)
        {
            energyBar.value = energy / 100f;
        }
    }

    private void UpdateMoney(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"${money}";
        }
    }

    private void UpdateClock(int hour)
    {
        if (clockText != null)
        {
            string period = hour >= 12 ? "PM" : "AM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;
            clockText.text = $"{displayHour}:00 {period}";
        }
    }
}
