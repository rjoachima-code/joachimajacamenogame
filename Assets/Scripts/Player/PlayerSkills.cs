using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance { get; private set; }

    [Header("Skill Settings")]
    [SerializeField] private int maxSkillLevel = 10;
    [SerializeField] private int experiencePerLevel = 100;

    [Header("Events")]
    public UnityEvent<string, int> onSkillLeveledUp;

    private Dictionary<string, int> skills = new Dictionary<string, int>();
    private Dictionary<string, int> skillExperience = new Dictionary<string, int>();

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

        InitializeSkills();
    }

    private void InitializeSkills()
    {
        string[] skillNames = { "Cooking", "Work", "Fitness", "Charm" };
        foreach (string skill in skillNames)
        {
            skills[skill] = 1;
            skillExperience[skill] = 0;
        }
    }

    public void AddSkillExperience(string skillName, int experience)
    {
        if (skills.ContainsKey(skillName) && skills[skillName] < maxSkillLevel)
        {
            skillExperience[skillName] += experience;

            if (skillExperience[skillName] >= experiencePerLevel * skills[skillName])
            {
                skills[skillName]++;
                skillExperience[skillName] = 0;
                onSkillLeveledUp?.Invoke(skillName, skills[skillName]);
            }
        }
    }

    public int GetSkillLevel(string skillName)
    {
        return skills.ContainsKey(skillName) ? skills[skillName] : 1;
    }

    public float GetSkillMultiplier(string skillName)
    {
        return 1f + (GetSkillLevel(skillName) - 1) * 0.1f; // 10% bonus per level
    }
}
