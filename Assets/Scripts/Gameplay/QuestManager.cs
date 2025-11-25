using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour, ISaveable
{
    public static QuestManager Instance { get; private set; }
    public List<Quest> quests = new List<Quest>();
    public event Action OnQuestsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
        LoadQuestsFromResources();
        TimeSystem.Instance.OnNewDay += GenerateDailyQuests;
        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if (TimeSystem.Instance != null)
            TimeSystem.Instance.OnNewDay -= GenerateDailyQuests;
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    private void LoadQuestsFromResources()
    {
        TextAsset questsJson = Resources.Load<TextAsset>("quests");
        if (questsJson != null)
        {
            QuestList questList = JsonUtility.FromJson<QuestList>(questsJson.text);
            if (questList != null && questList.quests != null)
            {
                quests.AddRange(questList.quests);
            }
        }
    }

    void GenerateDailyQuests()
    {
        // simple heuristic: if bills due this week, create a reminder quest
        if (BillsManager.Instance != null)
        {
            foreach(var b in BillsManager.Instance.bills)
            {
                if (b.IsDueThisMonth())
                {
                    if (!quests.Exists(q => q.id == "q_pay_"+b.id))
                    {
                        quests.Add(new Quest{ id = "q_pay_"+b.id, title = $"Pay {b.name}", description = $"Pay {b.name} of ${b.amount}", rewardMoney = 20});
                    }
                }
            }
        }
        OnQuestsChanged?.Invoke();
    }

    public void CompleteQuest(string id)
    {
        var q = quests.Find(x => x.id == id);
        if (q == null) return;
        q.completed = true;
        MoneyManager.Instance.AddMoney(q.rewardMoney, $"Quest reward: {q.title}");
        OnQuestsChanged?.Invoke();
    }

    [System.Serializable]
    private struct QuestManagerData
    {
        public List<Quest> quests;
    }

    public string SaveData()
    {
        var data = new QuestManagerData { quests = this.quests };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<QuestManagerData>(state);
        this.quests = data.quests ?? new List<Quest>();
        OnQuestsChanged?.Invoke();
    }

    [System.Serializable]
    private class QuestList
    {
        public List<Quest> quests;
    }
}
