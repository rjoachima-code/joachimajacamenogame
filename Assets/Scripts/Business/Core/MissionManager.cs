using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mission Manager handles mission creation, tracking, and completion for all businesses.
/// Manages mission chains, objectives, and rewards.
/// </summary>
public class MissionManager : MonoBehaviour, ISaveable
{
    public static MissionManager Instance { get; private set; }

    [Header("Active Missions")]
    [SerializeField] private List<BusinessMission> activeMissions = new List<BusinessMission>();
    [SerializeField] private List<BusinessMission> completedMissions = new List<BusinessMission>();
    [SerializeField] private List<BusinessMission> availableMissions = new List<BusinessMission>();

    public event Action<BusinessMission> OnMissionStarted;
    public event Action<BusinessMission> OnMissionCompleted;
    public event Action<BusinessMission, MissionObjective> OnObjectiveCompleted;
    public event Action<BusinessMission> OnMissionFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegisterSaveable(this);
        }
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnregisterSaveable(this);
        }
    }

    /// <summary>
    /// Start a mission for the player.
    /// </summary>
    public bool StartMission(string missionId)
    {
        var mission = availableMissions.Find(m => m.missionId == missionId);
        if (mission == null)
        {
            Debug.LogWarning($"[MissionManager] Mission not found: {missionId}");
            return false;
        }

        if (!CanStartMission(mission))
        {
            Debug.LogWarning($"[MissionManager] Cannot start mission: {missionId}");
            return false;
        }

        mission.status = MissionStatus.Active;
        mission.startTime = DateTime.Now;
        activeMissions.Add(mission);
        availableMissions.Remove(mission);

        OnMissionStarted?.Invoke(mission);
        Debug.Log($"[MissionManager] Started mission: {mission.title}");
        return true;
    }

    /// <summary>
    /// Check if a mission can be started.
    /// </summary>
    public bool CanStartMission(BusinessMission mission)
    {
        // Check prerequisites
        foreach (var prereq in mission.prerequisites)
        {
            if (!completedMissions.Exists(m => m.missionId == prereq))
            {
                return false;
            }
        }

        // Check tier requirement
        // TODO: Add business tier check

        return true;
    }

    /// <summary>
    /// Update progress on a mission objective.
    /// </summary>
    public void UpdateObjectiveProgress(string missionId, string objectiveId, int progress)
    {
        var mission = activeMissions.Find(m => m.missionId == missionId);
        if (mission == null) return;

        var objective = mission.objectives.Find(o => o.objectiveId == objectiveId);
        if (objective == null) return;

        objective.currentProgress = Mathf.Min(progress, objective.targetProgress);

        if (objective.IsComplete())
        {
            OnObjectiveCompleted?.Invoke(mission, objective);
            Debug.Log($"[MissionManager] Objective completed: {objective.description}");

            // Check if all objectives complete
            if (mission.AreAllObjectivesComplete())
            {
                CompleteMission(missionId);
            }
        }
    }

    /// <summary>
    /// Increment progress on a mission objective.
    /// </summary>
    public void IncrementObjectiveProgress(string missionId, string objectiveId, int amount = 1)
    {
        var mission = activeMissions.Find(m => m.missionId == missionId);
        if (mission == null) return;

        var objective = mission.objectives.Find(o => o.objectiveId == objectiveId);
        if (objective == null) return;

        UpdateObjectiveProgress(missionId, objectiveId, objective.currentProgress + amount);
    }

    /// <summary>
    /// Complete a mission and award rewards.
    /// </summary>
    public void CompleteMission(string missionId)
    {
        var mission = activeMissions.Find(m => m.missionId == missionId);
        if (mission == null) return;

        mission.status = MissionStatus.Completed;
        mission.completionTime = DateTime.Now;

        // Award rewards
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(mission.rewards.money, $"Mission: {mission.title}");
        }

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddExperience(mission.rewards.experience);
        }

        // Award business points - need to find active business
        // TODO: Link to business state

        activeMissions.Remove(mission);
        completedMissions.Add(mission);

        OnMissionCompleted?.Invoke(mission);
        Debug.Log($"[MissionManager] Mission completed: {mission.title}");

        // Unlock follow-up missions
        UnlockFollowUpMissions(mission);
    }

    /// <summary>
    /// Fail a mission.
    /// </summary>
    public void FailMission(string missionId, string reason)
    {
        var mission = activeMissions.Find(m => m.missionId == missionId);
        if (mission == null) return;

        mission.status = MissionStatus.Failed;
        activeMissions.Remove(mission);

        OnMissionFailed?.Invoke(mission);
        Debug.Log($"[MissionManager] Mission failed: {mission.title} - {reason}");
    }

    /// <summary>
    /// Unlock missions that have this mission as a prerequisite.
    /// </summary>
    private void UnlockFollowUpMissions(BusinessMission completedMission)
    {
        // TODO: Load available missions from data and check prerequisites
    }

    /// <summary>
    /// Get all active missions.
    /// </summary>
    public List<BusinessMission> GetActiveMissions()
    {
        return new List<BusinessMission>(activeMissions);
    }

    /// <summary>
    /// Get all completed missions.
    /// </summary>
    public List<BusinessMission> GetCompletedMissions()
    {
        return new List<BusinessMission>(completedMissions);
    }

    /// <summary>
    /// Get all available missions.
    /// </summary>
    public List<BusinessMission> GetAvailableMissions()
    {
        return new List<BusinessMission>(availableMissions);
    }

    /// <summary>
    /// Add a mission to the available list.
    /// </summary>
    public void AddAvailableMission(BusinessMission mission)
    {
        if (!availableMissions.Exists(m => m.missionId == mission.missionId))
        {
            availableMissions.Add(mission);
        }
    }

    #region Save/Load

    [System.Serializable]
    private class MissionSaveData
    {
        public List<BusinessMission> activeMissions;
        public List<BusinessMission> completedMissions;
        public List<string> availableMissionIds;
    }

    public string SaveData()
    {
        var data = new MissionSaveData
        {
            activeMissions = activeMissions,
            completedMissions = completedMissions,
            availableMissionIds = availableMissions.ConvertAll(m => m.missionId)
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<MissionSaveData>(state);
        if (data != null)
        {
            activeMissions = data.activeMissions ?? new List<BusinessMission>();
            completedMissions = data.completedMissions ?? new List<BusinessMission>();
            // Available missions need to be loaded from data based on IDs
        }
    }

    #endregion
}

/// <summary>
/// Represents a business mission with objectives and rewards.
/// </summary>
[System.Serializable]
public class BusinessMission
{
    public string missionId;
    public string title;
    [TextArea(2, 4)]
    public string description;
    public BusinessType businessType;
    public int requiredTier = 1;
    public MissionStatus status = MissionStatus.Available;

    public List<MissionObjective> objectives = new List<MissionObjective>();
    public MissionRewards rewards;
    public List<string> prerequisites = new List<string>();
    public List<string> unlocks = new List<string>();

    public DateTime startTime;
    public DateTime completionTime;
    public float timeLimit = -1f; // -1 = no limit

    /// <summary>
    /// Check if all objectives are complete.
    /// </summary>
    public bool AreAllObjectivesComplete()
    {
        foreach (var objective in objectives)
        {
            if (!objective.IsComplete()) return false;
        }
        return true;
    }

    /// <summary>
    /// Get overall progress percentage.
    /// </summary>
    public float GetProgressPercentage()
    {
        if (objectives.Count == 0) return 0f;

        float totalProgress = 0f;
        foreach (var objective in objectives)
        {
            totalProgress += objective.GetProgressPercentage();
        }
        return totalProgress / objectives.Count;
    }
}

/// <summary>
/// Mission objective that tracks progress toward a goal.
/// </summary>
[System.Serializable]
public class MissionObjective
{
    public string objectiveId;
    public string description;
    public ObjectiveType type;
    public int targetProgress = 1;
    public int currentProgress = 0;
    public bool isOptional = false;

    public bool IsComplete() => currentProgress >= targetProgress;
    public float GetProgressPercentage() => targetProgress > 0 ? (float)currentProgress / targetProgress * 100f : 0f;
}

/// <summary>
/// Types of mission objectives.
/// </summary>
public enum ObjectiveType
{
    ServeCustomers,
    ReachProfit,
    MaintainRating,
    CompleteTask,
    HireStaff,
    PurchaseEquipment,
    CompleteTraining,
    HostEvent,
    CompleteMiniGame,
    Other
}

/// <summary>
/// Mission status enum.
/// </summary>
public enum MissionStatus
{
    Locked,
    Available,
    Active,
    Completed,
    Failed
}

/// <summary>
/// Rewards for completing a mission.
/// </summary>
[System.Serializable]
public class MissionRewards
{
    public float money;
    public int experience;
    public int businessPoints;
    public List<string> unlockedItems = new List<string>();
    public List<string> unlockedFeatures = new List<string>();
    public string badge;
}
