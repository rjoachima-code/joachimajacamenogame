using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public List<Quest> quests = new List<Quest>();
    public event Action OnQuestsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
        TimeSystem.Instance.OnNewDay += GenerateDailyQuests;
    }

    void Start()
    {
        // seed sample quests if empty
        if (quests.Count == 0)
        {
            quests.Add(new Quest{ id="q_pay_rent", title="Pay Rent", description="Pay rent before due day", rewardMoney = 50});
        }
    }

    void GenerateDailyQuests()
    {
        // simple heuristic: if bills due this week, create a reminder quest
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
        OnQuestsChanged?.Invoke();
    }

    public List<QuestSave> GetQuestSaves()
    {
        var list = new List<QuestSave>();
        foreach(var q in quests) list.Add(new QuestSave{ id = q.id, completed = q.completed});
        return list;
    }

    public void ApplyQuestSaves(List<QuestSave> saves)
    {
        if (saves == null) return;
        foreach(var s in saves)
        {
            var q = quests.Find(x => x.id == s.id);
            if (q != null) q.completed = s.completed;
        }
    }

    public void CompleteQuest(string id)
    {
        var q = quests.Find(x => x.id == id);
        if (q == null) return;
        q.completed = true;
        MoneyManager.Instance.AddMoney(q.rewardMoney, $"Quest reward: {q.title}");
        OnQuestsChanged?.Invoke();
    }
}
