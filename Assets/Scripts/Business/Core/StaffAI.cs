using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Staff AI system that handles employee behavior, task execution, and performance.
/// </summary>
public class StaffAI : MonoBehaviour
{
    [Header("Staff Identity")]
    public string staffId;
    public string staffName;
    public StaffRole role;
    public BusinessType businessType;

    [Header("Attributes")]
    [Range(1, 10)] public int speed = 5;
    [Range(1, 10)] public int accuracy = 5;
    [Range(1, 10)] public int charisma = 5;
    [Range(1, 10)] public int maintenance = 5;
    [Range(1, 10)] public int stamina = 5;
    [Range(1, 10)] public int loyalty = 5;

    [Header("Experience & Skills")]
    public int experience = 0;
    public int level = 1;
    public List<string> unlockedSkills = new List<string>();

    [Header("Employment")]
    public float hourlyWage = 12f;
    public StaffSchedule schedule;
    public bool isOnDuty = false;
    public float morale = 80f;
    public float fatigue = 0f;

    [Header("Current State")]
    public StaffState currentState = StaffState.Idle;
    public BusinessTask currentTask;
    public float taskProgress = 0f;

    [Header("Navigation")]
    private NavMeshAgent navAgent;
    private Vector3 targetPosition;

    // Events
    public event Action<BusinessTask> OnTaskCompleted;
    public event Action<BusinessTask> OnTaskFailed;
    public event Action OnShiftEnded;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!isOnDuty) return;

        UpdateFatigue();
        UpdateState();
    }

    /// <summary>
    /// Update staff fatigue over time based on stamina.
    /// </summary>
    private void UpdateFatigue()
    {
        // Higher stamina = slower fatigue gain
        float fatigueRate = (11 - stamina) * 0.1f * Time.deltaTime;
        fatigue = Mathf.Min(fatigue + fatigueRate, 100f);

        // High fatigue reduces performance
        if (fatigue > 80f)
        {
            morale -= 0.01f * Time.deltaTime;
        }
    }

    /// <summary>
    /// Update AI state machine.
    /// </summary>
    private void UpdateState()
    {
        switch (currentState)
        {
            case StaffState.Idle:
                // Look for next task in queue
                break;

            case StaffState.MovingToTask:
                if (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    currentState = StaffState.PerformingTask;
                }
                break;

            case StaffState.PerformingTask:
                if (currentTask != null)
                {
                    PerformTask();
                }
                break;

            case StaffState.OnBreak:
                // Recover fatigue during break
                fatigue = Mathf.Max(fatigue - 5f * Time.deltaTime, 0f);
                break;

            case StaffState.Leaving:
                // Moving to exit
                break;
        }
    }

    /// <summary>
    /// Assign a task to this staff member.
    /// </summary>
    public bool AssignTask(BusinessTask task)
    {
        if (!isOnDuty || currentState == StaffState.OnBreak)
        {
            return false;
        }

        if (!CanPerformTask(task))
        {
            return false;
        }

        currentTask = task;
        currentTask.status = TaskStatus.InProgress;
        currentTask.assignedStaffId = staffId;
        taskProgress = 0f;

        // Navigate to task location if needed
        if (task.taskLocation != Vector3.zero && navAgent != null)
        {
            targetPosition = task.taskLocation;
            navAgent.SetDestination(targetPosition);
            currentState = StaffState.MovingToTask;
        }
        else
        {
            currentState = StaffState.PerformingTask;
        }

        Debug.Log($"[StaffAI] {staffName} assigned task: {task.taskName}");
        return true;
    }

    /// <summary>
    /// Check if staff can perform a specific task.
    /// </summary>
    public bool CanPerformTask(BusinessTask task)
    {
        // Check if role matches
        if (task.requiredRoles.Count > 0 && !task.requiredRoles.Contains(role))
        {
            return false;
        }

        // Check if has required skills
        foreach (var skill in task.requiredSkills)
        {
            if (!unlockedSkills.Contains(skill))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Execute current task with speed and accuracy calculations.
    /// </summary>
    private void PerformTask()
    {
        if (currentTask == null) return;

        // Calculate effective work rate based on attributes and fatigue
        float speedMultiplier = speed * 0.2f;
        float fatigueMultiplier = 1f - (fatigue / 200f); // Max 50% reduction at full fatigue
        float effectiveRate = speedMultiplier * fatigueMultiplier;

        taskProgress += effectiveRate * Time.deltaTime;

        // Check if task complete
        if (taskProgress >= currentTask.estimatedDuration)
        {
            CompleteCurrentTask();
        }
    }

    /// <summary>
    /// Complete the current task.
    /// </summary>
    private void CompleteCurrentTask()
    {
        if (currentTask == null) return;

        // Calculate quality based on accuracy
        float errorChance = (10 - accuracy) * 0.02f;
        bool hasError = UnityEngine.Random.value < errorChance;

        if (hasError)
        {
            currentTask.quality = Mathf.Max(0.5f, 1f - UnityEngine.Random.Range(0.1f, 0.3f));
        }
        else
        {
            currentTask.quality = 1f;
        }

        currentTask.status = TaskStatus.Completed;
        currentTask.completionTime = DateTime.Now;

        // Award experience
        experience += currentTask.experienceReward;
        CheckLevelUp();

        OnTaskCompleted?.Invoke(currentTask);
        Debug.Log($"[StaffAI] {staffName} completed task: {currentTask.taskName} with quality: {currentTask.quality:P0}");

        currentTask = null;
        taskProgress = 0f;
        currentState = StaffState.Idle;
    }

    /// <summary>
    /// Start shift for this staff member.
    /// </summary>
    public void StartShift()
    {
        isOnDuty = true;
        currentState = StaffState.Idle;
        fatigue = 0f;
        Debug.Log($"[StaffAI] {staffName} started shift");
    }

    /// <summary>
    /// End shift for this staff member.
    /// </summary>
    public void EndShift()
    {
        isOnDuty = false;
        currentState = StaffState.Leaving;

        if (currentTask != null)
        {
            // Incomplete task - return to queue
            currentTask.status = TaskStatus.Pending;
            currentTask.assignedStaffId = null;
            currentTask = null;
        }

        OnShiftEnded?.Invoke();
        Debug.Log($"[StaffAI] {staffName} ended shift");
    }

    /// <summary>
    /// Take a break to recover fatigue.
    /// </summary>
    public void TakeBreak(float duration)
    {
        currentState = StaffState.OnBreak;
        // Schedule return from break after duration
    }

    /// <summary>
    /// Check and process level up.
    /// </summary>
    private void CheckLevelUp()
    {
        int requiredXP = GetRequiredXPForLevel(level + 1);
        if (experience >= requiredXP)
        {
            level++;
            Debug.Log($"[StaffAI] {staffName} leveled up to {level}!");
        }
    }

    /// <summary>
    /// Get required XP for a specific level.
    /// </summary>
    private int GetRequiredXPForLevel(int targetLevel)
    {
        // Progressive XP curve: 50, 150, 300, 500, etc.
        return targetLevel switch
        {
            2 => 50,
            3 => 150,
            4 => 300,
            5 => 500,
            _ => 500 + (targetLevel - 5) * 200
        };
    }

    /// <summary>
    /// Learn a new skill.
    /// </summary>
    public void LearnSkill(string skillId)
    {
        if (!unlockedSkills.Contains(skillId))
        {
            unlockedSkills.Add(skillId);
            Debug.Log($"[StaffAI] {staffName} learned skill: {skillId}");
        }
    }

    /// <summary>
    /// Calculate customer satisfaction bonus from charisma.
    /// </summary>
    public float GetCharismaBonus()
    {
        return charisma * 0.03f; // 3% per point
    }

    /// <summary>
    /// Calculate equipment durability bonus from maintenance.
    /// </summary>
    public float GetMaintenanceBonus()
    {
        return maintenance * 0.05f; // 5% per point
    }

    /// <summary>
    /// Get effective performance multiplier considering all factors.
    /// </summary>
    public float GetEffectivePerformance()
    {
        float basePerformance = (speed + accuracy) / 20f; // Average of speed and accuracy
        float moraleBonus = morale / 100f;
        float fatigueReduction = 1f - (fatigue / 200f);
        return basePerformance * moraleBonus * fatigueReduction;
    }
}

/// <summary>
/// Staff member current state.
/// </summary>
public enum StaffState
{
    Idle,
    MovingToTask,
    PerformingTask,
    OnBreak,
    Leaving,
    Training
}

/// <summary>
/// Staff roles that can be assigned.
/// </summary>
public enum StaffRole
{
    // Hypermarket
    Cashier,
    Stocker,
    FreshDepartment,
    CustomerService,
    FloorManager,

    // Retail Fashion
    SalesAssociate,
    VisualMerchandiser,
    PersonalStylist,
    InventorySpecialist,
    StoreManager,

    // Restaurant
    LineCook,
    Server,
    Host,
    SousChef,
    Dishwasher,
    RestaurantManager,

    // Construction
    Carpenter,
    Electrician,
    Plumber,
    Painter,
    Laborer,
    ProjectManager,

    // Taxi/Uber
    Driver,
    Dispatcher,
    FleetMechanic,
    FleetManager,
    CustomerServiceRep
}

/// <summary>
/// Staff schedule configuration.
/// </summary>
[System.Serializable]
public class StaffSchedule
{
    public Dictionary<DayOfWeek, ShiftInfo> weeklySchedule = new Dictionary<DayOfWeek, ShiftInfo>();

    public StaffSchedule()
    {
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            weeklySchedule[day] = new ShiftInfo();
        }
    }
}

/// <summary>
/// Information about a single shift.
/// </summary>
[System.Serializable]
public class ShiftInfo
{
    public ShiftType shift = ShiftType.Off;
    public int hours = 0;
    public int startHour = 0;

    public ShiftInfo() { }

    public ShiftInfo(ShiftType type, int startHr, int hrs)
    {
        shift = type;
        startHour = startHr;
        hours = hrs;
    }
}

/// <summary>
/// Types of shifts available.
/// </summary>
public enum ShiftType
{
    Off,
    Morning,    // 6 AM - 2 PM
    Afternoon,  // 2 PM - 10 PM
    Evening,    // 10 PM - 6 AM
    Split,      // Custom 4-hour blocks
    OnCall      // As needed
}

/// <summary>
/// Staff data template for hiring.
/// </summary>
[System.Serializable]
public class StaffTemplate
{
    public string templateId;
    public string templateName;
    public StaffRole role;
    public BusinessType businessType;

    // Attribute ranges for generation
    public int minSpeed = 3;
    public int maxSpeed = 8;
    public int minAccuracy = 3;
    public int maxAccuracy = 8;
    public int minCharisma = 3;
    public int maxCharisma = 8;
    public int minMaintenance = 3;
    public int maxMaintenance = 8;
    public int minStamina = 3;
    public int maxStamina = 8;
    public int minLoyalty = 3;
    public int maxLoyalty = 8;

    public float baseWage = 12f;
    public string[] startingSkills;

    /// <summary>
    /// Generate a new staff member from this template.
    /// </summary>
    public StaffAI GenerateStaff(GameObject prefab)
    {
        var staffGO = UnityEngine.Object.Instantiate(prefab);
        var staff = staffGO.GetComponent<StaffAI>();
        if (staff == null) staff = staffGO.AddComponent<StaffAI>();

        staff.staffId = Guid.NewGuid().ToString();
        staff.staffName = GenerateRandomName();
        staff.role = role;
        staff.businessType = businessType;

        staff.speed = UnityEngine.Random.Range(minSpeed, maxSpeed + 1);
        staff.accuracy = UnityEngine.Random.Range(minAccuracy, maxAccuracy + 1);
        staff.charisma = UnityEngine.Random.Range(minCharisma, maxCharisma + 1);
        staff.maintenance = UnityEngine.Random.Range(minMaintenance, maxMaintenance + 1);
        staff.stamina = UnityEngine.Random.Range(minStamina, maxStamina + 1);
        staff.loyalty = UnityEngine.Random.Range(minLoyalty, maxLoyalty + 1);

        staff.hourlyWage = baseWage;

        if (startingSkills != null)
        {
            foreach (var skill in startingSkills)
            {
                staff.LearnSkill(skill);
            }
        }

        return staff;
    }

    private string GenerateRandomName()
    {
        string[] firstNames = { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "David", "Elizabeth" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        return $"{firstNames[UnityEngine.Random.Range(0, firstNames.Length)]} {lastNames[UnityEngine.Random.Range(0, lastNames.Length)]}";
    }
}
