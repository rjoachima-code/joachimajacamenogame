using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Manager - Central management system for all player-owned businesses.
/// </summary>
public class BusinessManager : MonoBehaviour, ISaveable
{
    public static BusinessManager Instance { get; private set; }

    [Header("Player Businesses")]
    [SerializeField] private List<BusinessState> ownedBusinesses = new List<BusinessState>();
    [SerializeField] private BusinessState activeBusiness;

    [Header("Staff Pool")]
    [SerializeField] private List<StaffAI> allStaff = new List<StaffAI>();

    // Events
    public event Action<BusinessState> OnBusinessCreated;
    public event Action<BusinessState> OnBusinessSelected;
    public event Action<BusinessState> OnBusinessUpgraded;
    public event Action<BusinessState> OnDayEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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

    private void Start()
    {
        // Subscribe to time events
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnNewDay += ProcessNewDay;
        }
    }

    /// <summary>
    /// Create a new business for the player.
    /// </summary>
    public BusinessState CreateBusiness(BusinessType type, string name)
    {
        var business = new BusinessState
        {
            businessId = Guid.NewGuid().ToString(),
            businessName = name,
            businessType = type,
            tier = 1,
            reputation = 3.0f,
            cashBalance = 0f,
            businessPoints = 0,
            todayStats = new DailyStats()
        };

        ownedBusinesses.Add(business);

        if (activeBusiness == null)
        {
            activeBusiness = business;
        }

        OnBusinessCreated?.Invoke(business);
        Debug.Log($"[BusinessManager] Created new {type} business: {name}");
        return business;
    }

    /// <summary>
    /// Get the currently active business.
    /// </summary>
    public BusinessState GetActiveBusiness()
    {
        return activeBusiness;
    }

    /// <summary>
    /// Set the active business.
    /// </summary>
    public void SetActiveBusiness(string businessId)
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business != null)
        {
            activeBusiness = business;
            OnBusinessSelected?.Invoke(business);
        }
    }

    /// <summary>
    /// Get all owned businesses.
    /// </summary>
    public List<BusinessState> GetAllBusinesses()
    {
        return new List<BusinessState>(ownedBusinesses);
    }

    /// <summary>
    /// Get businesses by type.
    /// </summary>
    public List<BusinessState> GetBusinessesByType(BusinessType type)
    {
        return ownedBusinesses.FindAll(b => b.businessType == type);
    }

    /// <summary>
    /// Open a business for the day.
    /// </summary>
    public void OpenBusiness(string businessId)
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business != null)
        {
            business.isOpen = true;
            Debug.Log($"[BusinessManager] Opened business: {business.businessName}");
        }
    }

    /// <summary>
    /// Close a business for the day.
    /// </summary>
    public void CloseBusiness(string businessId)
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business != null)
        {
            business.isOpen = false;
            Debug.Log($"[BusinessManager] Closed business: {business.businessName}");
        }
    }

    /// <summary>
    /// Process new day for all businesses.
    /// </summary>
    private void ProcessNewDay()
    {
        foreach (var business in ownedBusinesses)
        {
            business.EndDay();
            OnDayEnded?.Invoke(business);
        }
    }

    /// <summary>
    /// Record a sale for a business.
    /// </summary>
    public void RecordSale(string businessId, float amount)
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business != null)
        {
            business.todayStats.RecordSale(amount);
            business.cashBalance += amount;
        }
    }

    /// <summary>
    /// Record an expense for a business.
    /// </summary>
    public void RecordExpense(string businessId, float amount, string description = "")
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business != null)
        {
            business.todayStats.RecordExpense(amount);
            business.cashBalance -= amount;
        }
    }

    /// <summary>
    /// Upgrade a business tier.
    /// </summary>
    public bool UpgradeBusiness(string businessId, int requiredBP, float requiredCash)
    {
        var business = ownedBusinesses.Find(b => b.businessId == businessId);
        if (business == null) return false;

        if (!business.CanUpgradeTier(requiredBP, requiredCash))
        {
            Debug.LogWarning("[BusinessManager] Cannot upgrade - insufficient resources");
            return false;
        }

        business.businessPoints -= requiredBP;
        business.cashBalance -= requiredCash;
        business.tier++;

        OnBusinessUpgraded?.Invoke(business);
        Debug.Log($"[BusinessManager] Upgraded {business.businessName} to tier {business.tier}");
        return true;
    }

    /// <summary>
    /// Add a staff member to a business.
    /// </summary>
    public void AddStaff(StaffAI staff)
    {
        if (!allStaff.Contains(staff))
        {
            allStaff.Add(staff);
        }
    }

    /// <summary>
    /// Get all staff for a business type.
    /// </summary>
    public List<StaffAI> GetStaffForBusiness(BusinessType type)
    {
        return allStaff.FindAll(s => s.businessType == type);
    }

    /// <summary>
    /// Get on-duty staff for current business.
    /// </summary>
    public List<StaffAI> GetOnDutyStaff()
    {
        if (activeBusiness == null) return new List<StaffAI>();
        return allStaff.FindAll(s => 
            s.businessType == activeBusiness.businessType && 
            s.isOnDuty);
    }

    /// <summary>
    /// Calculate total business points across all businesses.
    /// </summary>
    public int GetTotalBusinessPoints()
    {
        int total = 0;
        foreach (var business in ownedBusinesses)
        {
            total += business.businessPoints;
        }
        return total;
    }

    #region Save/Load

    [System.Serializable]
    private class BusinessManagerSaveData
    {
        public List<BusinessState> ownedBusinesses;
        public string activeBusinessId;
    }

    public string SaveData()
    {
        var data = new BusinessManagerSaveData
        {
            ownedBusinesses = ownedBusinesses,
            activeBusinessId = activeBusiness?.businessId ?? ""
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<BusinessManagerSaveData>(state);
        if (data != null)
        {
            ownedBusinesses = data.ownedBusinesses ?? new List<BusinessState>();
            if (!string.IsNullOrEmpty(data.activeBusinessId))
            {
                activeBusiness = ownedBusinesses.Find(b => b.businessId == data.activeBusinessId);
            }
        }
    }

    #endregion
}

/// <summary>
/// API interface for Business Dashboard UI.
/// </summary>
public static class BusinessDashboardAPI
{
    /// <summary>
    /// Get dashboard summary data for a business.
    /// </summary>
    public static DashboardSummary GetDashboardSummary(string businessId)
    {
        var business = BusinessManager.Instance?.GetAllBusinesses()
            .Find(b => b.businessId == businessId);
        
        if (business == null) return null;

        return new DashboardSummary
        {
            businessName = business.businessName,
            businessType = business.businessType,
            tier = business.tier,
            reputation = business.reputation,
            businessPoints = business.businessPoints,
            todayRevenue = business.todayStats.revenue,
            todayExpenses = business.todayStats.expenses,
            todayProfit = business.todayStats.profit,
            customersServed = business.todayStats.customersServed,
            isOpen = business.isOpen,
            currentCustomers = business.currentCustomers,
            maxCustomers = business.maxCustomers,
            footTrafficMultiplier = business.GetFootTrafficMultiplier(),
            priceMarkup = business.GetPriceMarkupAllowed()
        };
    }

    /// <summary>
    /// Get staff summary for dashboard.
    /// </summary>
    public static List<StaffSummary> GetStaffSummary()
    {
        var staffList = BusinessManager.Instance?.GetOnDutyStaff();
        if (staffList == null) return new List<StaffSummary>();

        var summaries = new List<StaffSummary>();
        foreach (var staff in staffList)
        {
            summaries.Add(new StaffSummary
            {
                staffId = staff.staffId,
                staffName = staff.staffName,
                role = staff.role,
                isOnDuty = staff.isOnDuty,
                currentState = staff.currentState,
                morale = staff.morale,
                fatigue = staff.fatigue,
                level = staff.level
            });
        }
        return summaries;
    }

    /// <summary>
    /// Get pending tasks for dashboard.
    /// </summary>
    public static List<TaskSummary> GetTaskSummary()
    {
        var tasks = TaskQueueSystem.Instance?.GetPendingTasks();
        if (tasks == null) return new List<TaskSummary>();

        var summaries = new List<TaskSummary>();
        foreach (var task in tasks)
        {
            summaries.Add(new TaskSummary
            {
                taskId = task.taskId,
                taskName = task.taskName,
                priority = task.priority,
                status = task.status,
                timeRemaining = task.GetTimeRemaining(),
                isUrgent = task.IsUrgent(),
                assignedStaffId = task.assignedStaffId,
                experienceReward = task.experienceReward
            });
        }
        return summaries;
    }

    /// <summary>
    /// Get active events for dashboard.
    /// </summary>
    public static List<EventSummary> GetEventSummary()
    {
        var events = BusinessEventSystem.Instance?.GetActiveEvents();
        if (events == null) return new List<EventSummary>();

        var summaries = new List<EventSummary>();
        foreach (var evt in events)
        {
            summaries.Add(new EventSummary
            {
                eventId = evt.eventId,
                eventName = evt.eventName,
                description = evt.description,
                severity = evt.severity,
                remainingHours = evt.GetRemainingHours(),
                requiresAction = evt.requiresAction,
                actionDescription = evt.actionDescription
            });
        }
        return summaries;
    }
}

/// <summary>
/// Dashboard summary data structure.
/// </summary>
[System.Serializable]
public class DashboardSummary
{
    public string businessName;
    public BusinessType businessType;
    public int tier;
    public float reputation;
    public int businessPoints;
    public float todayRevenue;
    public float todayExpenses;
    public float todayProfit;
    public int customersServed;
    public bool isOpen;
    public int currentCustomers;
    public int maxCustomers;
    public float footTrafficMultiplier;
    public float priceMarkup;
}

/// <summary>
/// Staff summary for dashboard display.
/// </summary>
[System.Serializable]
public class StaffSummary
{
    public string staffId;
    public string staffName;
    public StaffRole role;
    public bool isOnDuty;
    public StaffState currentState;
    public float morale;
    public float fatigue;
    public int level;
}

/// <summary>
/// Task summary for dashboard display.
/// </summary>
[System.Serializable]
public class TaskSummary
{
    public string taskId;
    public string taskName;
    public TaskPriority priority;
    public TaskStatus status;
    public TimeSpan timeRemaining;
    public bool isUrgent;
    public string assignedStaffId;
    public int experienceReward;
}

/// <summary>
/// Event summary for dashboard display.
/// </summary>
[System.Serializable]
public class EventSummary
{
    public string eventId;
    public string eventName;
    public string description;
    public EventSeverity severity;
    public float remainingHours;
    public bool requiresAction;
    public string actionDescription;
}
