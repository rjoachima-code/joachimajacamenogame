using UnityEngine;
using System;
using System.Collections.Generic;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public float balance = 500f;
    public event Action OnBalanceChanged;

    public List<Transaction> transactions = new List<Transaction>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
    }

    public void AddMoney(float amount, string description = "Income")
    {
        balance += amount;
        transactions.Add(new Transaction{ amount = amount, description = description, date = System.DateTime.Now.ToString() });
        OnBalanceChanged?.Invoke();
    }

    public bool Withdraw(float amount, string description = "Expense")
    {
        if (balance - amount < -10000f) return false; // hard credit limit
        balance -= amount;
        transactions.Add(new Transaction{ amount = -amount, description = description, date = System.DateTime.Now.ToString() });
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
    public int lastPaidYear = -1;
    public int lastPaidMonth = -1;

    public void MarkPaidForCurrentCycle()
    {
        var now = System.DateTime.Now;
        lastPaidYear = now.Year;
        lastPaidMonth = now.Month;
    }

    public bool IsDueThisMonth()
    {
        // simplistic: if lastPaidMonth != current month and dueDay <= today
        var now = System.DateTime.Now;
        if (lastPaidYear == now.Year && lastPaidMonth == now.Month) return false;
        return now.Day >= dueDay;
    }
}

public enum BillingFrequency { Monthly, Weekly, Daily }
