using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

[Serializable]
public class GameSave
{
    public int hour;
    public int minute;
    public float moneyBalance;
    public List<Transaction> transactions;
    public List<Bill> bills;
    public NeedsSnapshot needs;
    public List<QuestSave> quests;
}

[Serializable]
public class QuestSave
{
    public string id;
    public bool completed;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    string saveFile => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
    }

    public void SaveGame()
    {
        var g = new GameSave();
        g.hour = TimeSystem.Instance.Hour;
        g.minute = TimeSystem.Instance.Minute;
        g.moneyBalance = MoneyManager.Instance.balance;
        g.transactions = MoneyManager.Instance.transactions;
        g.bills = BillsManager.Instance.bills;
        g.needs = NeedsManager.Instance.Snapshot();
        g.quests = QuestManager.Instance.GetQuestSaves();

        string json = JsonUtility.ToJson(g, true);
        File.WriteAllText(saveFile, json);
        Debug.Log($"Game saved to {saveFile}");
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFile)) { Debug.Log("No save file"); return; }
        string json = File.ReadAllText(saveFile);
        GameSave g = JsonUtility.FromJson<GameSave>(json);

        // restore
        TimeSystem.Instance.Hour = g.hour;
        TimeSystem.Instance.MinutesSet(g.minute); // helper method you'll add below
        MoneyManager.Instance.balance = g.moneyBalance;
        MoneyManager.Instance.transactions = g.transactions ?? new List<Transaction>();
        BillsManager.Instance.bills = g.bills ?? new List<Bill>();
        // restore needs
        var n = g.needs;
        NeedsManager.Instance.hunger = n.hunger;
        NeedsManager.Instance.energy = n.energy;
        NeedsManager.Instance.hygiene = n.hygiene;
        NeedsManager.Instance.stress = n.stress;
        // restore quests
        QuestManager.Instance.ApplyQuestSaves(g.quests);
        Debug.Log("Game loaded");
    }
}
