using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Load saved data or start new game
        SaveManager.Instance.LoadGame();

        // Initialize quests
        QuestManager.Instance.AddQuest(new Quest("daily_eat", "Eat Breakfast", "Start your day with a meal", Quest.QuestType.Daily, 10, 5));
        QuestManager.Instance.AddQuest(new Quest("daily_work", "Go to Work", "Earn some money", Quest.QuestType.Daily, 50, 10));
        QuestManager.Instance.AddQuest(new Quest("weekly_pay_bills", "Pay Bills", "Pay rent and utilities", Quest.QuestType.Weekly, 0, 20));
    }
}
