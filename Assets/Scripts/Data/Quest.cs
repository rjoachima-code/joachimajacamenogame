using System;

[Serializable]
public class Quest
{
    public string id;
    public string title;
    public string description;
    public QuestType type;
    public bool isCompleted;
    public int rewardMoney;
    public int rewardExperience;

    public enum QuestType
    {
        Daily,
        Weekly,
        Story
    }

    public Quest(string id, string title, string description, QuestType type, int rewardMoney, int rewardExperience)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.type = type;
        this.isCompleted = false;
        this.rewardMoney = rewardMoney;
        this.rewardExperience = rewardExperience;
    }
}
