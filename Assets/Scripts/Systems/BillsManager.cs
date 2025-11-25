using UnityEngine;
using System.Collections.Generic;
using System;

public class BillsManager : MonoBehaviour, ISaveable
{
    public static BillsManager Instance { get; private set; }
    public List<Bill> bills = new List<Bill>();

    public event Action<Bill> OnBillDue;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
        TimeSystem.Instance.OnNewDay += CheckBills;
        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if (TimeSystem.Instance != null)
            TimeSystem.Instance.OnNewDay -= CheckBills;
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    void Start()
    {
        // Example pre-seed if empty
        if (bills.Count == 0)
        {
            bills.Add(new Bill{ id = "rent", name = "Rent", amount = 1200f, frequency = BillingFrequency.Monthly, dueDay = 1 });
            bills.Add(new Bill{ id = "elec", name = "Electricity", amount = 70f, frequency = BillingFrequency.Monthly, dueDay = 15 });
        }
    }

    void CheckBills()
    {
        foreach (var b in bills)
        {
            if (b.IsDueThisMonth())
            {
                OnBillDue?.Invoke(b);
                if (b.autoPay)
                {
                    MoneyManager.Instance.PayBill(b);
                }
            }
        }
    }

    public void AddBill(Bill b) { bills.Add(b); }
    public void RemoveBill(string id) { bills.RemoveAll(x => x.id == id); }

    [System.Serializable]
    private struct BillsData
    {
        public List<Bill> bills;
    }

    public string SaveData()
    {
        var data = new BillsData { bills = this.bills };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<BillsData>(state);
        this.bills = data.bills ?? new List<Bill>();
    }
}
