using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Business Dashboard UI Controller.
/// Displays business stats, tasks, staff, events, and provides management interface.
/// </summary>
public class BusinessDashboardUI : MonoBehaviour
{
    public static BusinessDashboardUI Instance { get; private set; }

    [Header("Panel References")]
    [SerializeField] private GameObject dashboardPanel;
    [SerializeField] private GameObject taskQueuePanel;
    [SerializeField] private GameObject hiringPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject schedulePanel;
    [SerializeField] private GameObject missionLogPanel;

    [Header("Dashboard Elements")]
    [SerializeField] private TMP_Text businessNameText;
    [SerializeField] private TMP_Text tierText;
    [SerializeField] private TMP_Text reputationText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private TMP_Text businessPointsText;
    [SerializeField] private TMP_Text revenueText;
    [SerializeField] private TMP_Text expensesText;
    [SerializeField] private TMP_Text profitText;
    [SerializeField] private TMP_Text customersServedText;
    [SerializeField] private TMP_Text footTrafficText;
    [SerializeField] private TMP_Text priceMarkupText;

    [Header("Task Queue Elements")]
    [SerializeField] private Transform taskListContainer;
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private TMP_Text completedTasksText;
    [SerializeField] private TMP_Text staffEfficiencyText;

    [Header("Staff Elements")]
    [SerializeField] private Transform staffListContainer;
    [SerializeField] private GameObject staffItemPrefab;

    [Header("Event Elements")]
    [SerializeField] private Transform eventListContainer;
    [SerializeField] private GameObject eventItemPrefab;

    [Header("Navigation Buttons")]
    [SerializeField] private Button dashboardButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button hiringButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button missionsButton;
    [SerializeField] private Button reportsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;

    private DashboardSummary currentSummary;
    private string currentBusinessId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupButtonListeners();
        HideAllPanels();
    }

    private void Start()
    {
        // Subscribe to business events
        if (BusinessManager.Instance != null)
        {
            BusinessManager.Instance.OnBusinessSelected += OnBusinessSelected;
        }

        if (TaskQueueSystem.Instance != null)
        {
            TaskQueueSystem.Instance.OnTaskAdded += OnTaskAdded;
            TaskQueueSystem.Instance.OnTaskCompleted += OnTaskCompleted;
        }

        if (BusinessEventSystem.Instance != null)
        {
            BusinessEventSystem.Instance.OnEventStarted += OnEventStarted;
            BusinessEventSystem.Instance.OnEventEnded += OnEventEnded;
        }
    }

    private void OnDestroy()
    {
        if (BusinessManager.Instance != null)
        {
            BusinessManager.Instance.OnBusinessSelected -= OnBusinessSelected;
        }

        if (TaskQueueSystem.Instance != null)
        {
            TaskQueueSystem.Instance.OnTaskAdded -= OnTaskAdded;
            TaskQueueSystem.Instance.OnTaskCompleted -= OnTaskCompleted;
        }

        if (BusinessEventSystem.Instance != null)
        {
            BusinessEventSystem.Instance.OnEventStarted -= OnEventStarted;
            BusinessEventSystem.Instance.OnEventEnded -= OnEventEnded;
        }
    }

    private void SetupButtonListeners()
    {
        if (dashboardButton != null)
            dashboardButton.onClick.AddListener(() => ShowPanel(dashboardPanel));
        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(() => ShowPanel(inventoryPanel));
        if (hiringButton != null)
            hiringButton.onClick.AddListener(() => ShowPanel(hiringPanel));
        if (missionsButton != null)
            missionsButton.onClick.AddListener(() => ShowPanel(missionLogPanel));
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDashboard);
    }

    /// <summary>
    /// Open the dashboard for a specific business.
    /// </summary>
    public void OpenDashboard(string businessId)
    {
        currentBusinessId = businessId;
        RefreshDashboard();
        
        if (dashboardPanel != null)
        {
            dashboardPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Close the dashboard.
    /// </summary>
    public void CloseDashboard()
    {
        HideAllPanels();
    }

    /// <summary>
    /// Refresh all dashboard data.
    /// </summary>
    public void RefreshDashboard()
    {
        if (string.IsNullOrEmpty(currentBusinessId)) return;

        currentSummary = BusinessDashboardAPI.GetDashboardSummary(currentBusinessId);
        if (currentSummary == null) return;

        UpdateHeaderInfo();
        UpdateStatsInfo();
        UpdateTaskList();
        UpdateStaffList();
        UpdateEventList();
    }

    private void UpdateHeaderInfo()
    {
        if (businessNameText != null)
            businessNameText.text = currentSummary.businessName;

        if (tierText != null)
            tierText.text = $"Tier {currentSummary.tier}";

        if (reputationText != null)
            reputationText.text = $"{currentSummary.reputation:F1}";

        // Update star images
        if (starImages != null)
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    float fillAmount = Mathf.Clamp01(currentSummary.reputation - i);
                    starImages[i].fillAmount = fillAmount;
                }
            }
        }

        if (businessPointsText != null)
            businessPointsText.text = $"BP: {currentSummary.businessPoints}";
    }

    private void UpdateStatsInfo()
    {
        if (revenueText != null)
            revenueText.text = $"${currentSummary.todayRevenue:F2}";

        if (expensesText != null)
            expensesText.text = $"${currentSummary.todayExpenses:F2}";

        if (profitText != null)
        {
            profitText.text = $"${currentSummary.todayProfit:F2}";
            profitText.color = currentSummary.todayProfit >= 0 ? Color.green : Color.red;
        }

        if (customersServedText != null)
            customersServedText.text = $"{currentSummary.customersServed}";

        if (footTrafficText != null)
            footTrafficText.text = $"{currentSummary.footTrafficMultiplier:P0}";

        if (priceMarkupText != null)
            priceMarkupText.text = $"{currentSummary.priceMarkup:+0%;-0%;0%}";
    }

    private void UpdateTaskList()
    {
        if (taskListContainer == null || taskItemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in taskListContainer)
        {
            Destroy(child.gameObject);
        }

        var tasks = BusinessDashboardAPI.GetTaskSummary();
        foreach (var task in tasks)
        {
            var taskItem = Instantiate(taskItemPrefab, taskListContainer);
            SetupTaskItem(taskItem, task);
        }

        if (completedTasksText != null)
        {
            int completed = tasks.FindAll(t => t.status == TaskStatus.Completed).Count;
            completedTasksText.text = $"Completed: {completed}/{tasks.Count}";
        }
    }

    private void SetupTaskItem(GameObject item, TaskSummary task)
    {
        var nameText = item.transform.Find("TaskName")?.GetComponent<TMP_Text>();
        var priorityText = item.transform.Find("Priority")?.GetComponent<TMP_Text>();
        var timeText = item.transform.Find("TimeRemaining")?.GetComponent<TMP_Text>();
        var assignButton = item.transform.Find("AssignButton")?.GetComponent<Button>();
        var doItButton = item.transform.Find("DoItButton")?.GetComponent<Button>();

        if (nameText != null)
            nameText.text = task.taskName;

        if (priorityText != null)
        {
            priorityText.text = task.priority.ToString().ToUpper();
            priorityText.color = GetPriorityColor(task.priority);
        }

        if (timeText != null)
        {
            timeText.text = FormatTimeRemaining(task.timeRemaining);
            if (task.isUrgent)
                timeText.color = Color.red;
        }

        if (assignButton != null)
        {
            assignButton.onClick.AddListener(() => OnAssignTaskClicked(task.taskId));
        }

        if (doItButton != null)
        {
            doItButton.onClick.AddListener(() => OnDoTaskClicked(task.taskId));
        }
    }

    private void UpdateStaffList()
    {
        if (staffListContainer == null || staffItemPrefab == null) return;

        foreach (Transform child in staffListContainer)
        {
            Destroy(child.gameObject);
        }

        var staff = BusinessDashboardAPI.GetStaffSummary();
        foreach (var member in staff)
        {
            var staffItem = Instantiate(staffItemPrefab, staffListContainer);
            SetupStaffItem(staffItem, member);
        }
    }

    private void SetupStaffItem(GameObject item, StaffSummary staff)
    {
        var nameText = item.transform.Find("StaffName")?.GetComponent<TMP_Text>();
        var roleText = item.transform.Find("Role")?.GetComponent<TMP_Text>();
        var statusImage = item.transform.Find("StatusIndicator")?.GetComponent<Image>();
        var moraleBar = item.transform.Find("MoraleBar")?.GetComponent<Slider>();

        if (nameText != null)
            nameText.text = staff.staffName;

        if (roleText != null)
            roleText.text = staff.role.ToString();

        if (statusImage != null)
            statusImage.color = staff.isOnDuty ? Color.green : Color.gray;

        if (moraleBar != null)
            moraleBar.value = staff.morale / 100f;
    }

    private void UpdateEventList()
    {
        if (eventListContainer == null || eventItemPrefab == null) return;

        foreach (Transform child in eventListContainer)
        {
            Destroy(child.gameObject);
        }

        var events = BusinessDashboardAPI.GetEventSummary();
        foreach (var evt in events)
        {
            var eventItem = Instantiate(eventItemPrefab, eventListContainer);
            SetupEventItem(eventItem, evt);
        }
    }

    private void SetupEventItem(GameObject item, EventSummary evt)
    {
        var nameText = item.transform.Find("EventName")?.GetComponent<TMP_Text>();
        var descText = item.transform.Find("Description")?.GetComponent<TMP_Text>();
        var timeText = item.transform.Find("TimeRemaining")?.GetComponent<TMP_Text>();
        var severityImage = item.transform.Find("SeverityIcon")?.GetComponent<Image>();
        var actionButton = item.transform.Find("ActionButton")?.GetComponent<Button>();

        if (nameText != null)
            nameText.text = evt.eventName;

        if (descText != null)
            descText.text = evt.description;

        if (timeText != null)
            timeText.text = evt.remainingHours > 0 ? $"{evt.remainingHours:F1}h remaining" : "Permanent";

        if (severityImage != null)
            severityImage.color = GetSeverityColor(evt.severity);

        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(evt.requiresAction);
            if (evt.requiresAction)
            {
                var buttonText = actionButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                    buttonText.text = evt.actionDescription;

                actionButton.onClick.AddListener(() => OnEventActionClicked(evt.eventId));
            }
        }
    }

    private void ShowPanel(GameObject panel)
    {
        HideAllPanels();
        if (panel != null)
            panel.SetActive(true);
    }

    private void HideAllPanels()
    {
        if (dashboardPanel != null) dashboardPanel.SetActive(false);
        if (taskQueuePanel != null) taskQueuePanel.SetActive(false);
        if (hiringPanel != null) hiringPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (schedulePanel != null) schedulePanel.SetActive(false);
        if (missionLogPanel != null) missionLogPanel.SetActive(false);
    }

    #region Event Handlers

    private void OnBusinessSelected(BusinessState business)
    {
        currentBusinessId = business.businessId;
        RefreshDashboard();
    }

    private void OnTaskAdded(BusinessTask task)
    {
        UpdateTaskList();
    }

    private void OnTaskCompleted(BusinessTask task)
    {
        UpdateTaskList();
    }

    private void OnEventStarted(BusinessEvent evt)
    {
        UpdateEventList();
    }

    private void OnEventEnded(BusinessEvent evt)
    {
        UpdateEventList();
    }

    private void OnAssignTaskClicked(string taskId)
    {
        Debug.Log($"[DashboardUI] Assign task clicked: {taskId}");
        // Open staff assignment dropdown
    }

    private void OnDoTaskClicked(string taskId)
    {
        Debug.Log($"[DashboardUI] Do task clicked: {taskId}");
        // Player performs task - may trigger mini-game
    }

    private void OnEventActionClicked(string eventId)
    {
        Debug.Log($"[DashboardUI] Event action clicked: {eventId}");
        BusinessEventSystem.Instance?.HandleEventAction(eventId, "default");
    }

    #endregion

    #region Helper Methods

    private Color GetPriorityColor(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Critical => Color.red,
            TaskPriority.Urgent => new Color(1f, 0.5f, 0f), // Orange
            TaskPriority.High => Color.yellow,
            TaskPriority.Normal => Color.white,
            TaskPriority.Low => Color.gray,
            _ => Color.white
        };
    }

    private Color GetSeverityColor(EventSeverity severity)
    {
        return severity switch
        {
            EventSeverity.Critical => Color.red,
            EventSeverity.High => new Color(1f, 0.5f, 0f), // Orange
            EventSeverity.Medium => Color.yellow,
            EventSeverity.Low => Color.white,
            EventSeverity.Positive => Color.green,
            _ => Color.white
        };
    }

    private string FormatTimeRemaining(TimeSpan time)
    {
        if (time.TotalHours >= 1)
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        if (time.TotalMinutes >= 1)
            return $"{(int)time.TotalMinutes}m";
        return $"{time.Seconds}s";
    }

    #endregion

    /// <summary>
    /// Toggle dashboard visibility.
    /// </summary>
    public void ToggleDashboard()
    {
        if (dashboardPanel != null)
        {
            bool isActive = dashboardPanel.activeSelf;
            if (isActive)
            {
                CloseDashboard();
            }
            else
            {
                var activeBusiness = BusinessManager.Instance?.GetActiveBusiness();
                if (activeBusiness != null)
                {
                    OpenDashboard(activeBusiness.businessId);
                }
            }
        }
    }
}
