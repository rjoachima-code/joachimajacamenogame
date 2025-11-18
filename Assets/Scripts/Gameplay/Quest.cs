using System;

[Serializable]
public class Quest
{
    public string id;
    public string title;
    public string description;
    public bool completed = false;
    public int rewardMoney = 0;
    public int rewardXP = 0;
}
