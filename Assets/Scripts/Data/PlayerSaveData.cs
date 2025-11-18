using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public float hunger;
    public float energy;
    public int money;
    public float stress;
    public int experience;
    public Dictionary<string, int> skills;
    public List<string> inventory;
    public List<string> completedQuests;
    public float gameTime;

    public PlayerSaveData()
    {
        hunger = 100f;
        energy = 100f;
        money = 100;
        stress = 0f;
        experience = 0;
        skills = new Dictionary<string, int>
        {
            { "Cooking", 1 },
            { "Work", 1 },
            { "Fitness", 1 },
            { "Charm", 1 }
        };
        inventory = new List<string>();
        completedQuests = new List<string>();
        gameTime = 0f;
    }
}
