using UnityEngine;
using UnityEngine.Events;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Money Settings")]
    [SerializeField] private int startingMoney = 100;

    [Header("Events")]
    public UnityEvent<int> onMoneyChanged;

    private int currentMoney;

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

        currentMoney = startingMoney;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        onMoneyChanged?.Invoke(currentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            onMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }

    public int GetMoney()
    {
        return currentMoney;
    }
}
