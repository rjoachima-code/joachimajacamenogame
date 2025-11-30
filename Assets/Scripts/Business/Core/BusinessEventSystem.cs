using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event System for business challenges: supply delays, breakdowns, inspections, weather, competitors.
/// </summary>
public class BusinessEventSystem : MonoBehaviour
{
    public static BusinessEventSystem Instance { get; private set; }

    [Header("Active Events")]
    [SerializeField] private List<BusinessEvent> activeEvents = new List<BusinessEvent>();
    [SerializeField] private List<BusinessEvent> eventHistory = new List<BusinessEvent>();

    [Header("Event Probabilities")]
    [SerializeField] private float supplyDelayChance = 0.30f;
    [SerializeField] private float supplyShortageChance = 0.10f;
    [SerializeField] private float bulkDiscountChance = 0.15f;
    [SerializeField] private float minorBreakdownChance = 0.05f;
    [SerializeField] private float majorBreakdownChance = 0.02f;

    [Header("Scheduled Events")]
    [SerializeField] private List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    // Events
    public event Action<BusinessEvent> OnEventStarted;
    public event Action<BusinessEvent> OnEventEnded;
    public event Action<BusinessEvent> OnEventUpdated;

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
        // Subscribe to time events
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick += CheckTimedEvents;
            TimeSystem.Instance.OnNewDay += ProcessDailyEvents;
        }
    }

    private void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick -= CheckTimedEvents;
            TimeSystem.Instance.OnNewDay -= ProcessDailyEvents;
        }
    }

    /// <summary>
    /// Check and update timed events every game tick.
    /// </summary>
    private void CheckTimedEvents(int hour, int minute)
    {
        // Check for expired events
        var expiredEvents = activeEvents.FindAll(e => DateTime.Now >= e.endTime);
        foreach (var evt in expiredEvents)
        {
            EndEvent(evt.eventId);
        }

        // Check scheduled events
        foreach (var scheduled in scheduledEvents.ToArray())
        {
            if (TimeSystem.Instance.Day >= scheduled.triggerDay)
            {
                TriggerEvent(scheduled.eventType, scheduled.affectedCategory);
                scheduledEvents.Remove(scheduled);
            }
        }
    }

    /// <summary>
    /// Process daily random events.
    /// </summary>
    private void ProcessDailyEvents()
    {
        // Roll for supply events
        if (UnityEngine.Random.value < supplyDelayChance)
        {
            string[] categories = { "dairy", "produce", "bakery", "meat", "frozen" };
            string category = categories[UnityEngine.Random.Range(0, categories.Length)];
            TriggerEvent(EventType.SupplyDelay, category);
        }

        if (UnityEngine.Random.value < supplyShortageChance)
        {
            TriggerEvent(EventType.SupplyShortage, "all");
        }

        if (UnityEngine.Random.value < bulkDiscountChance)
        {
            string[] categories = { "grocery", "household", "beverages" };
            string category = categories[UnityEngine.Random.Range(0, categories.Length)];
            TriggerEvent(EventType.BulkDiscount, category);
        }

        // Roll for equipment events
        if (UnityEngine.Random.value < minorBreakdownChance)
        {
            TriggerEvent(EventType.MinorBreakdown, "random_equipment");
        }

        if (UnityEngine.Random.value < majorBreakdownChance)
        {
            TriggerEvent(EventType.MajorBreakdown, "random_equipment");
        }
    }

    /// <summary>
    /// Trigger a new business event.
    /// </summary>
    public BusinessEvent TriggerEvent(EventType type, string affectedTarget)
    {
        var evt = CreateEvent(type, affectedTarget);
        activeEvents.Add(evt);
        OnEventStarted?.Invoke(evt);
        Debug.Log($"[EventSystem] Event started: {evt.eventName} affecting {affectedTarget}");
        return evt;
    }

    /// <summary>
    /// Create an event based on type.
    /// </summary>
    private BusinessEvent CreateEvent(EventType type, string target)
    {
        var evt = new BusinessEvent
        {
            eventId = Guid.NewGuid().ToString(),
            eventType = type,
            affectedTarget = target,
            startTime = DateTime.Now
        };

        switch (type)
        {
            case EventType.SupplyDelay:
                evt.eventName = $"Supply Delay - {target}";
                evt.description = $"Supplies for {target} are delayed due to logistics issues.";
                evt.durationHours = UnityEngine.Random.Range(24f, 72f);
                evt.severity = EventSeverity.Medium;
                evt.effects.Add("supply_delay", target);
                break;

            case EventType.SupplyShortage:
                evt.eventName = "Regional Supply Shortage";
                evt.description = "A regional shortage is affecting all suppliers.";
                evt.durationHours = UnityEngine.Random.Range(72f, 168f);
                evt.severity = EventSeverity.High;
                evt.effects.Add("supply_shortage", "all");
                break;

            case EventType.BulkDiscount:
                evt.eventName = $"Bulk Discount - {target}";
                evt.description = $"Supplier offering 20% off {target} for 24 hours!";
                evt.durationHours = 24f;
                evt.severity = EventSeverity.Positive;
                evt.effects.Add("discount", "0.20");
                evt.effects.Add("category", target);
                break;

            case EventType.MinorBreakdown:
                evt.eventName = "Equipment Malfunction";
                evt.description = "Equipment operating at reduced efficiency.";
                evt.durationHours = UnityEngine.Random.Range(2f, 4f);
                evt.severity = EventSeverity.Low;
                evt.effects.Add("efficiency", "0.50");
                break;

            case EventType.MajorBreakdown:
                evt.eventName = "Equipment Breakdown";
                evt.description = "Equipment non-functional - repair required.";
                evt.durationHours = UnityEngine.Random.Range(4f, 24f);
                evt.severity = EventSeverity.High;
                evt.effects.Add("equipment_offline", target);
                evt.requiresAction = true;
                evt.actionDescription = "Call repair service";
                break;

            case EventType.HealthInspection:
                evt.eventName = "Health Inspection";
                evt.description = "Health inspector will arrive soon.";
                evt.durationHours = 4f;
                evt.severity = EventSeverity.High;
                evt.requiresAction = true;
                evt.actionDescription = "Ensure cleanliness standards";
                break;

            case EventType.SafetyInspection:
                evt.eventName = "Safety Inspection";
                evt.description = "Safety inspector scheduled.";
                evt.durationHours = 4f;
                evt.severity = EventSeverity.Medium;
                evt.requiresAction = true;
                evt.actionDescription = "Check safety equipment";
                break;

            case EventType.SurpriseAudit:
                evt.eventName = "Surprise Financial Audit";
                evt.description = "Auditor arriving - no notice given!";
                evt.durationHours = 8f;
                evt.severity = EventSeverity.High;
                evt.requiresAction = true;
                evt.actionDescription = "Prepare financial records";
                break;

            case EventType.Rain:
                evt.eventName = "Rainy Weather";
                evt.description = "Rain affecting foot traffic.";
                evt.durationHours = UnityEngine.Random.Range(4f, 12f);
                evt.severity = EventSeverity.Low;
                evt.effects.Add("foot_traffic", "-0.20");
                evt.effects.Add("delivery_demand", "+0.10");
                break;

            case EventType.Storm:
                evt.eventName = "Severe Storm";
                evt.description = "Storm conditions - potential power issues.";
                evt.durationHours = UnityEngine.Random.Range(2f, 8f);
                evt.severity = EventSeverity.High;
                evt.effects.Add("foot_traffic", "-0.50");
                evt.effects.Add("power_outage_chance", "0.30");
                break;

            case EventType.HeatWave:
                evt.eventName = "Heat Wave";
                evt.description = "Extreme heat affecting demand patterns.";
                evt.durationHours = UnityEngine.Random.Range(24f, 72f);
                evt.severity = EventSeverity.Medium;
                evt.effects.Add("cold_demand", "+0.20");
                evt.effects.Add("equipment_strain", "true");
                break;

            case EventType.Snow:
                evt.eventName = "Snowy Conditions";
                evt.description = "Snow affecting travel and deliveries.";
                evt.durationHours = UnityEngine.Random.Range(12f, 48f);
                evt.severity = EventSeverity.Medium;
                evt.effects.Add("foot_traffic", "-0.40");
                evt.effects.Add("supply_delay_chance", "+0.20");
                break;

            case EventType.NewCompetitor:
                evt.eventName = "New Competitor Opened";
                evt.description = "A competitor opened nearby!";
                evt.durationHours = 336f; // 2 weeks
                evt.severity = EventSeverity.Medium;
                evt.effects.Add("foot_traffic", "-0.15");
                break;

            case EventType.CompetitorSale:
                evt.eventName = "Competitor Sale Event";
                evt.description = "Competitor running major sale.";
                evt.durationHours = 72f; // 3 days
                evt.severity = EventSeverity.Low;
                evt.effects.Add("revenue", "-0.10");
                break;

            case EventType.CompetitorClosure:
                evt.eventName = "Competitor Closed";
                evt.description = "A competitor has closed - opportunity!";
                evt.durationHours = -1f; // Permanent
                evt.severity = EventSeverity.Positive;
                evt.effects.Add("foot_traffic", "+0.20");
                break;

            case EventType.SeasonalDemand:
                evt.eventName = "Seasonal Demand Surge";
                evt.description = "Holiday season increasing demand.";
                evt.durationHours = 168f; // 1 week
                evt.severity = EventSeverity.Positive;
                evt.effects.Add("demand", "+0.30");
                break;

            default:
                evt.eventName = "Unknown Event";
                evt.description = "Something happened...";
                evt.durationHours = 24f;
                evt.severity = EventSeverity.Low;
                break;
        }

        evt.endTime = evt.durationHours > 0 
            ? DateTime.Now.AddHours(evt.durationHours) 
            : DateTime.MaxValue;

        return evt;
    }

    /// <summary>
    /// End an active event.
    /// </summary>
    public void EndEvent(string eventId)
    {
        var evt = activeEvents.Find(e => e.eventId == eventId);
        if (evt == null) return;

        evt.isActive = false;
        activeEvents.Remove(evt);
        eventHistory.Add(evt);

        OnEventEnded?.Invoke(evt);
        Debug.Log($"[EventSystem] Event ended: {evt.eventName}");
    }

    /// <summary>
    /// Handle player action on an event (e.g., call repair service).
    /// </summary>
    public void HandleEventAction(string eventId, string action)
    {
        var evt = activeEvents.Find(e => e.eventId == eventId);
        if (evt == null) return;

        switch (evt.eventType)
        {
            case EventType.MajorBreakdown:
                if (action == "repair")
                {
                    // Schedule repair completion
                    evt.durationHours = Mathf.Max(evt.durationHours - 12f, 1f);
                    evt.endTime = DateTime.Now.AddHours(evt.durationHours);
                    evt.requiresAction = false;
                    OnEventUpdated?.Invoke(evt);
                }
                break;

            case EventType.HealthInspection:
            case EventType.SafetyInspection:
                evt.requiresAction = false;
                OnEventUpdated?.Invoke(evt);
                break;
        }
    }

    /// <summary>
    /// Schedule a future event.
    /// </summary>
    public void ScheduleEvent(EventType type, int daysFromNow, string target = "")
    {
        var scheduled = new ScheduledEvent
        {
            eventType = type,
            triggerDay = TimeSystem.Instance.Day + daysFromNow,
            affectedCategory = target
        };
        scheduledEvents.Add(scheduled);
        Debug.Log($"[EventSystem] Scheduled {type} for day {scheduled.triggerDay}");
    }

    /// <summary>
    /// Get all active events.
    /// </summary>
    public List<BusinessEvent> GetActiveEvents()
    {
        return new List<BusinessEvent>(activeEvents);
    }

    /// <summary>
    /// Get active events by type.
    /// </summary>
    public List<BusinessEvent> GetEventsByType(EventType type)
    {
        return activeEvents.FindAll(e => e.eventType == type);
    }

    /// <summary>
    /// Check if an event type is currently active.
    /// </summary>
    public bool IsEventActive(EventType type)
    {
        return activeEvents.Exists(e => e.eventType == type && e.isActive);
    }

    /// <summary>
    /// Get cumulative effect modifiers from all active events.
    /// </summary>
    public Dictionary<string, float> GetCumulativeEffects()
    {
        var cumulative = new Dictionary<string, float>();

        foreach (var evt in activeEvents)
        {
            foreach (var effect in evt.effects)
            {
                if (float.TryParse(effect.Value, out float value))
                {
                    if (cumulative.ContainsKey(effect.Key))
                    {
                        cumulative[effect.Key] += value;
                    }
                    else
                    {
                        cumulative[effect.Key] = value;
                    }
                }
            }
        }

        return cumulative;
    }
}

/// <summary>
/// Represents a business event affecting operations.
/// </summary>
[System.Serializable]
public class BusinessEvent
{
    public string eventId;
    public string eventName;
    public string description;
    public EventType eventType;
    public EventSeverity severity;
    public string affectedTarget;

    public DateTime startTime;
    public DateTime endTime;
    public float durationHours;
    public bool isActive = true;

    public bool requiresAction = false;
    public string actionDescription;

    public Dictionary<string, string> effects = new Dictionary<string, string>();

    public float GetRemainingHours()
    {
        return (float)(endTime - DateTime.Now).TotalHours;
    }
}

/// <summary>
/// Scheduled event for future triggering.
/// </summary>
[System.Serializable]
public class ScheduledEvent
{
    public EventType eventType;
    public int triggerDay;
    public string affectedCategory;
}

/// <summary>
/// Types of business events.
/// </summary>
public enum EventType
{
    // Supply Events
    SupplyDelay,
    SupplyShortage,
    BulkDiscount,

    // Equipment Events
    MinorBreakdown,
    MajorBreakdown,
    MaintenanceDue,

    // Inspection Events
    HealthInspection,
    SafetyInspection,
    SurpriseAudit,

    // Weather Events
    Rain,
    Storm,
    HeatWave,
    Snow,

    // Competitor Events
    NewCompetitor,
    CompetitorSale,
    CompetitorClosure,

    // Other Events
    SeasonalDemand,
    SpecialPromotion,
    VIPCustomer
}

/// <summary>
/// Event severity levels.
/// </summary>
public enum EventSeverity
{
    Positive,
    Low,
    Medium,
    High,
    Critical
}
