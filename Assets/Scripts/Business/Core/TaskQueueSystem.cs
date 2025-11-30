using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Task Queue System for managing business tasks and dispatch.
/// </summary>
public class TaskQueueSystem : MonoBehaviour
{
    public static TaskQueueSystem Instance { get; private set; }

    [Header("Task Queue")]
    [SerializeField] private List<BusinessTask> taskQueue = new List<BusinessTask>();
    [SerializeField] private List<BusinessTask> completedTasks = new List<BusinessTask>();

    [Header("Settings")]
    [SerializeField] private int maxQueueSize = 50;

    // Events
    public event Action<BusinessTask> OnTaskAdded;
    public event Action<BusinessTask> OnTaskAssigned;
    public event Action<BusinessTask> OnTaskCompleted;
    public event Action<BusinessTask> OnTaskFailed;
    public event Action<BusinessTask> OnTaskExpired;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        CheckExpiredTasks();
    }

    /// <summary>
    /// Add a task to the queue.
    /// </summary>
    public bool AddTask(BusinessTask task)
    {
        if (taskQueue.Count >= maxQueueSize)
        {
            Debug.LogWarning("[TaskQueue] Queue is full");
            return false;
        }

        task.taskId = Guid.NewGuid().ToString();
        task.creationTime = DateTime.Now;
        task.status = TaskStatus.Pending;

        // Calculate deadline if not set
        if (task.deadline == DateTime.MinValue)
        {
            task.deadline = DateTime.Now.AddMinutes(task.deadlineMinutes);
        }

        taskQueue.Add(task);
        SortQueue();

        OnTaskAdded?.Invoke(task);
        Debug.Log($"[TaskQueue] Task added: {task.taskName}");
        return true;
    }

    /// <summary>
    /// Get next task for a specific staff member based on their role.
    /// </summary>
    public BusinessTask GetNextTaskForStaff(StaffAI staff)
    {
        foreach (var task in taskQueue)
        {
            if (task.status != TaskStatus.Pending) continue;
            if (staff.CanPerformTask(task))
            {
                return task;
            }
        }
        return null;
    }

    /// <summary>
    /// Get next task for player (can do any task).
    /// </summary>
    public BusinessTask GetNextTask()
    {
        return taskQueue.Find(t => t.status == TaskStatus.Pending);
    }

    /// <summary>
    /// Assign a task to a staff member.
    /// </summary>
    public bool AssignTask(string taskId, StaffAI staff)
    {
        var task = taskQueue.Find(t => t.taskId == taskId);
        if (task == null)
        {
            Debug.LogWarning($"[TaskQueue] Task not found: {taskId}");
            return false;
        }

        if (staff.AssignTask(task))
        {
            task.assignedStaffId = staff.staffId;
            task.status = TaskStatus.InProgress;
            OnTaskAssigned?.Invoke(task);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Mark a task as completed.
    /// </summary>
    public void CompleteTask(string taskId, float quality = 1f)
    {
        var task = taskQueue.Find(t => t.taskId == taskId);
        if (task == null) return;

        task.status = TaskStatus.Completed;
        task.completionTime = DateTime.Now;
        task.quality = quality;

        taskQueue.Remove(task);
        completedTasks.Add(task);

        // Keep only recent history
        while (completedTasks.Count > 100)
        {
            completedTasks.RemoveAt(0);
        }

        OnTaskCompleted?.Invoke(task);
        Debug.Log($"[TaskQueue] Task completed: {task.taskName}");
    }

    /// <summary>
    /// Mark a task as failed.
    /// </summary>
    public void FailTask(string taskId, string reason)
    {
        var task = taskQueue.Find(t => t.taskId == taskId);
        if (task == null) return;

        task.status = TaskStatus.Failed;
        task.failureReason = reason;

        taskQueue.Remove(task);
        completedTasks.Add(task);

        OnTaskFailed?.Invoke(task);
        Debug.Log($"[TaskQueue] Task failed: {task.taskName} - {reason}");
    }

    /// <summary>
    /// Check for expired tasks.
    /// </summary>
    private void CheckExpiredTasks()
    {
        var expired = taskQueue.FindAll(t => 
            t.status == TaskStatus.Pending && 
            DateTime.Now > t.deadline);

        foreach (var task in expired)
        {
            task.status = TaskStatus.Expired;
            taskQueue.Remove(task);
            OnTaskExpired?.Invoke(task);
            Debug.Log($"[TaskQueue] Task expired: {task.taskName}");
        }
    }

    /// <summary>
    /// Sort queue by priority and deadline.
    /// </summary>
    private void SortQueue()
    {
        taskQueue.Sort((a, b) =>
        {
            int priorityCompare = b.priority.CompareTo(a.priority);
            if (priorityCompare != 0) return priorityCompare;
            return a.deadline.CompareTo(b.deadline);
        });
    }

    /// <summary>
    /// Get all pending tasks.
    /// </summary>
    public List<BusinessTask> GetPendingTasks()
    {
        return taskQueue.FindAll(t => t.status == TaskStatus.Pending);
    }

    /// <summary>
    /// Get all tasks in queue.
    /// </summary>
    public List<BusinessTask> GetAllTasks()
    {
        return new List<BusinessTask>(taskQueue);
    }

    /// <summary>
    /// Get task counts by status.
    /// </summary>
    public Dictionary<TaskStatus, int> GetTaskCounts()
    {
        var counts = new Dictionary<TaskStatus, int>();
        foreach (TaskStatus status in Enum.GetValues(typeof(TaskStatus)))
        {
            counts[status] = taskQueue.FindAll(t => t.status == status).Count;
        }
        return counts;
    }

    /// <summary>
    /// Generate common business tasks based on business state.
    /// </summary>
    public void GenerateRoutineTasks(BusinessState business)
    {
        // This would be overridden by specific business types
        // Base implementation generates generic tasks

        // Cleaning task
        AddTask(new BusinessTask
        {
            taskName = "Clean Floor",
            taskType = TaskType.Cleaning,
            priority = TaskPriority.Normal,
            estimatedDuration = 15f,
            deadlineMinutes = 60,
            experienceReward = 10
        });
    }
}

/// <summary>
/// Represents a task in the business queue.
/// </summary>
[System.Serializable]
public class BusinessTask
{
    public string taskId;
    public string taskName;
    public string description;
    public TaskType taskType;
    public TaskPriority priority = TaskPriority.Normal;
    public TaskStatus status = TaskStatus.Pending;

    public List<StaffRole> requiredRoles = new List<StaffRole>();
    public List<string> requiredSkills = new List<string>();
    public int minimumLevel = 1;

    public float estimatedDuration = 10f; // seconds in game time
    public float deadlineMinutes = 30f;
    public DateTime deadline;
    public DateTime creationTime;
    public DateTime completionTime;

    public string assignedStaffId;
    public Vector3 taskLocation;

    public int experienceReward = 10;
    public float moneyReward = 0f;
    public float quality = 1f;
    public string failureReason;

    // For mini-game tasks
    public bool requiresMiniGame = false;
    public string miniGameId;

    /// <summary>
    /// Get time remaining until deadline.
    /// </summary>
    public TimeSpan GetTimeRemaining()
    {
        return deadline - DateTime.Now;
    }

    /// <summary>
    /// Check if task is urgent (less than 5 minutes remaining).
    /// </summary>
    public bool IsUrgent()
    {
        return GetTimeRemaining().TotalMinutes < 5;
    }
}

/// <summary>
/// Task types available.
/// </summary>
public enum TaskType
{
    // General
    Cleaning,
    CustomerService,
    RegisterOperation,

    // Inventory
    Stocking,
    InventoryCount,
    DeliveryIntake,
    PriceUpdate,

    // Food/Restaurant
    Cooking,
    FoodPrep,
    TableService,
    Dishwashing,

    // Fashion
    VisualMerchandising,
    FittingRoomService,
    StylingConsultation,

    // Construction
    Measuring,
    Cutting,
    Installation,
    Painting,
    Demolition,

    // Taxi
    RidePickup,
    RideDropoff,
    VehicleMaintenance,
    Dispatch,

    // Management
    StaffSupervision,
    ScheduleCreation,
    ReportGeneration,

    // Other
    Training,
    EquipmentMaintenance,
    Custom
}

/// <summary>
/// Task priority levels.
/// </summary>
public enum TaskPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3,
    Critical = 4
}

/// <summary>
/// Task status.
/// </summary>
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Expired,
    Cancelled
}
