using UnityEngine;
using System;
using System.Collections.Generic;

public class MoneyManager : MonoBehaviour, ISaveable
{
    public static MoneyManager Instance { get; private set; }

    public float balance = 500f;
    public event Action OnBalanceChanged;

    public List<Transaction> transactions = new List<Transaction>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    public void AddMoney(float amount, string description = "Income")
    {
        balance += amount;
        transactions.Add(new Transaction { amount = amount, description = description, date = TimeSystem.Instance.GetDateString() });
        OnBalanceChanged?.Invoke();
    }

    public bool Withdraw(float amount, string description = "Expense")
    {
        if (balance - amount < -10000f) return false; // hard credit limit
        balance -= amount;
        transactions.Add(new Transaction { amount = -amount, description = description, date = TimeSystem.Instance.GetDateString() });
        OnBalanceChanged?.Invoke();
        return true;
    }

    public void PayBill(Bill bill)
    {
        if (Withdraw(bill.amount, $"Bill: {bill.name}"))
        {
            bill.MarkPaidForCurrentCycle();
        }
    }

    [System.Serializable]
    private struct MoneyData
    {
        public float balance;
        public List<Transaction> transactions;
    }

    public string SaveData()
    {
        var data = new MoneyData
        {
            balance = balance,
            transactions = transactions
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<MoneyData>(state);
        balance = data.balance;
        transactions = data.transactions ?? new List<Transaction>();
        OnBalanceChanged?.Invoke();
    }
}

[System.Serializable]
public class Transaction
{
    public float amount;
    public string description;
    public string date;
}

[System.Serializable]
public class Bill
{
    public string id;
    public string name;
    public float amount;
    public BillingFrequency frequency = BillingFrequency.Monthly;
    public int dueDay = 1; // day of month for monthly
    public bool autoPay = true;

    // internal tracking
    public int lastPaidDay = -1;

    public void MarkPaidForCurrentCycle()
    {
        lastPaidDay = TimeSystem.Instance.Day;
    }

    public bool IsDueThisMonth()
    {
        if (lastPaidDay == TimeSystem.Instance.Day) return false; // Already paid today

        switch (frequency)
        {
            case BillingFrequency.Daily:
                return lastPaidDay < TimeSystem.Instance.Day;
            case BillingFrequency.Weekly:
                // Paid within the last 7 days?
                return (TimeSystem.Instance.Day - lastPaidDay) >= 7;
            case BillingFrequency.Monthly:
                // Assuming a month is 30 days for simplicity for now.
                // A more robust solution would be to use the TimeSystem to track months.
                return TimeSystem.Instance.Day >= dueDay && (TimeSystem.Instance.Day - lastPaidDay) >= 30;
            default:
                return false;
        }
    }
}

public enum BillingFrequency { Monthly, Weekly, Daily }
