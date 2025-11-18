using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Events")]
    public UnityEvent<Quest> onQuestCompleted;

    private List<Quest> activeQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();

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

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
    }

    public void CompleteQuest(string questId)
    {
        Quest quest = activeQuests.Find(q => q.id == questId);
        if (quest != null)
        {
            quest.isCompleted = true;
            activeQuests.Remove(quest);
            completedQuests.Add(quest);

            // Give rewards
            MoneyManager.Instance.AddMoney(quest.rewardMoney);
            // Add experience logic here

            onQuestCompleted?.Invoke(quest);
        }
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    public List<Quest> GetCompletedQuests()
    {
        return completedQuests;
    }
}
