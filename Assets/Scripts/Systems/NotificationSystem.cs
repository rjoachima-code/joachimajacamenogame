using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// System for managing in-game notifications (toasts, alerts, guide messages).
/// </summary>
public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 5f;
    [SerializeField] private int maxVisibleNotifications = 3;
    [SerializeField] private float notificationSpacing = 0.5f;

    [Header("Current State")]
    [SerializeField] private List<Notification> activeNotifications = new List<Notification>();
    [SerializeField] private Queue<Notification> pendingNotifications = new Queue<Notification>();

    public event Action<Notification> OnNotificationShown;
    public event Action<Notification> OnNotificationDismissed;
    public event Action<Notification> OnNotificationClicked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Subscribe to events that should trigger notifications
        SubscribeToGameEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromGameEvents();
    }

    private void SubscribeToGameEvents()
    {
        // Train arrivals
        if (TrainSystem.Instance != null)
        {
            TrainSystem.Instance.OnTrainArrival += OnTrainArrival;
        }

        // Quest completions
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestsChanged += OnQuestsChanged;
        }

        // Time events
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnNewDay += OnNewDay;
        }

        // Money changes
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnBalanceChanged += OnBalanceChanged;
        }
    }

    private void UnsubscribeFromGameEvents()
    {
        if (TrainSystem.Instance != null)
        {
            TrainSystem.Instance.OnTrainArrival -= OnTrainArrival;
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestsChanged -= OnQuestsChanged;
        }

        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnNewDay -= OnNewDay;
        }

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnBalanceChanged -= OnBalanceChanged;
        }
    }

    /// <summary>
    /// Shows a simple text notification.
    /// </summary>
    public void ShowNotification(string message, NotificationType type = NotificationType.Info)
    {
        var notification = new Notification
        {
            id = Guid.NewGuid().ToString(),
            message = message,
            type = type,
            duration = defaultDuration,
            timestamp = DateTime.Now
        };

        QueueNotification(notification);
    }

    /// <summary>
    /// Shows a notification with a title.
    /// </summary>
    public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
    {
        var notification = new Notification
        {
            id = Guid.NewGuid().ToString(),
            title = title,
            message = message,
            type = type,
            duration = defaultDuration,
            timestamp = DateTime.Now
        };

        QueueNotification(notification);
    }

    /// <summary>
    /// Shows a notification with full configuration.
    /// </summary>
    public void ShowNotification(Notification notification)
    {
        if (string.IsNullOrEmpty(notification.id))
        {
            notification.id = Guid.NewGuid().ToString();
        }
        notification.timestamp = DateTime.Now;

        QueueNotification(notification);
    }

    /// <summary>
    /// Shows a guide message from JocGuide.
    /// </summary>
    public void ShowGuideMessage(string message)
    {
        var notification = new Notification
        {
            id = Guid.NewGuid().ToString(),
            title = "Joc",
            message = message,
            type = NotificationType.Guide,
            duration = 8f, // Guide messages stay longer
            timestamp = DateTime.Now,
            icon = null // Would be set to Joc's icon
        };

        QueueNotification(notification);
    }

    /// <summary>
    /// Shows an achievement notification.
    /// </summary>
    public void ShowAchievement(string achievementName, string description)
    {
        var notification = new Notification
        {
            id = Guid.NewGuid().ToString(),
            title = "Achievement Unlocked!",
            message = $"{achievementName}\n{description}",
            type = NotificationType.Achievement,
            duration = 7f,
            timestamp = DateTime.Now
        };

        QueueNotification(notification);
    }

    /// <summary>
    /// Shows a money change notification.
    /// </summary>
    public void ShowMoneyNotification(float amount, string reason)
    {
        string sign = amount >= 0 ? "+" : "";
        var notification = new Notification
        {
            id = Guid.NewGuid().ToString(),
            title = $"{sign}${amount:F0}",
            message = reason,
            type = amount >= 0 ? NotificationType.Success : NotificationType.Warning,
            duration = 3f,
            timestamp = DateTime.Now
        };

        QueueNotification(notification);
    }

    /// <summary>
    /// Queues a notification for display.
    /// </summary>
    private void QueueNotification(Notification notification)
    {
        if (activeNotifications.Count < maxVisibleNotifications)
        {
            DisplayNotification(notification);
        }
        else
        {
            pendingNotifications.Enqueue(notification);
        }
    }

    /// <summary>
    /// Displays a notification immediately.
    /// </summary>
    private void DisplayNotification(Notification notification)
    {
        activeNotifications.Add(notification);
        OnNotificationShown?.Invoke(notification);

        // Auto-dismiss after duration
        if (notification.duration > 0)
        {
            StartCoroutine(AutoDismissNotification(notification));
        }
    }

    /// <summary>
    /// Coroutine to auto-dismiss notifications.
    /// </summary>
    private IEnumerator AutoDismissNotification(Notification notification)
    {
        yield return new WaitForSeconds(notification.duration);
        DismissNotification(notification.id);
    }

    /// <summary>
    /// Dismisses a notification by ID.
    /// </summary>
    public void DismissNotification(string notificationId)
    {
        var notification = activeNotifications.Find(n => n.id == notificationId);
        if (notification != null)
        {
            activeNotifications.Remove(notification);
            OnNotificationDismissed?.Invoke(notification);

            // Show next pending notification
            if (pendingNotifications.Count > 0 && activeNotifications.Count < maxVisibleNotifications)
            {
                DisplayNotification(pendingNotifications.Dequeue());
            }
        }
    }

    /// <summary>
    /// Dismisses all notifications.
    /// </summary>
    public void DismissAllNotifications()
    {
        foreach (var notification in new List<Notification>(activeNotifications))
        {
            OnNotificationDismissed?.Invoke(notification);
        }
        activeNotifications.Clear();
        pendingNotifications.Clear();
    }

    /// <summary>
    /// Handles notification click.
    /// </summary>
    public void OnNotificationClick(string notificationId)
    {
        var notification = activeNotifications.Find(n => n.id == notificationId);
        if (notification != null)
        {
            OnNotificationClicked?.Invoke(notification);
            notification.onClick?.Invoke();
            DismissNotification(notificationId);
        }
    }

    // Event handlers for automatic notifications

    private void OnTrainArrival(DistrictType district)
    {
        ShowNotification("Train Arrived", $"Welcome to {district} District!", NotificationType.Info);
    }

    private void OnQuestsChanged()
    {
        // Could show quest completion notifications here
    }

    private void OnNewDay()
    {
        if (TimeSystem.Instance != null)
        {
            ShowNotification($"Day {TimeSystem.Instance.Day}", "A new day begins!", NotificationType.Info);
        }
    }

    private void OnBalanceChanged()
    {
        // Balance change notifications are handled by direct calls from MoneyManager
    }

    /// <summary>
    /// Gets all active notifications.
    /// </summary>
    public List<Notification> GetActiveNotifications()
    {
        return activeNotifications;
    }

    /// <summary>
    /// Gets the count of pending notifications.
    /// </summary>
    public int GetPendingCount()
    {
        return pendingNotifications.Count;
    }
}

/// <summary>
/// Represents a single notification.
/// </summary>
[Serializable]
public class Notification
{
    public string id;
    public string title;
    public string message;
    public NotificationType type;
    public float duration;
    public DateTime timestamp;
    public Sprite icon;
    public Action onClick;
}

/// <summary>
/// Types of notifications with different visual styles.
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    Quest,
    Achievement,
    Guide,
    Social,
    Money
}
