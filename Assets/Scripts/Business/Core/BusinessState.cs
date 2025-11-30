using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core business state management system for all business types.
/// Handles reputation, business points, daily stats, and progression.
/// </summary>
[System.Serializable]
public class BusinessState
{
    [Header("Identification")]
    public string businessId;
    public string businessName;
    public BusinessType businessType;
    public int tier = 1;

    [Header("Reputation")]
    [Range(0f, 5f)]
    public float reputation = 3.0f;
    public float serviceSpeedScore = 3.0f;
    public float qualityScore = 3.0f;
    public float cleanlinessScore = 3.0f;
    public float ambianceScore = 3.0f;
    public float valueScore = 3.0f;

    [Header("Currency")]
    public int businessPoints = 0;
    public float cashBalance = 0f;

    [Header("Daily Statistics")]
    public DailyStats todayStats;
    public List<DailyStats> statsHistory = new List<DailyStats>();

    [Header("State")]
    public bool isOpen = false;
    public int currentCustomers = 0;
    public int maxCustomers = 30;

    /// <summary>
    /// Calculate weighted reputation score based on all factors.
    /// </summary>
    public float CalculateReputation()
    {
        float weighted = (serviceSpeedScore * 0.25f) +
                        (qualityScore * 0.30f) +
                        (cleanlinessScore * 0.15f) +
                        (ambianceScore * 0.15f) +
                        (valueScore * 0.15f);
        reputation = Mathf.Clamp(weighted, 0f, 5f);
        return reputation;
    }

    /// <summary>
    /// Get foot traffic multiplier based on reputation.
    /// </summary>
    public float GetFootTrafficMultiplier()
    {
        if (reputation >= 4.5f) return 2.0f;
        if (reputation >= 4.0f) return 1.5f;
        if (reputation >= 3.5f) return 1.25f;
        if (reputation >= 3.0f) return 1.0f;
        if (reputation >= 2.0f) return 0.75f;
        return 0.5f;
    }

    /// <summary>
    /// Get allowed price markup based on reputation.
    /// </summary>
    public float GetPriceMarkupAllowed()
    {
        if (reputation >= 4.5f) return 0.30f;
        if (reputation >= 4.0f) return 0.20f;
        if (reputation >= 3.5f) return 0.10f;
        if (reputation >= 3.0f) return 0f;
        if (reputation >= 2.0f) return -0.10f;
        return -0.20f;
    }

    /// <summary>
    /// Award business points for various achievements.
    /// </summary>
    public void AwardBusinessPoints(int amount, string reason)
    {
        businessPoints += amount;
        Debug.Log($"[BusinessState] Awarded {amount} BP for: {reason}. Total: {businessPoints}");
    }

    /// <summary>
    /// Check and award daily milestone business points.
    /// </summary>
    public void CheckDailyMilestones()
    {
        if (todayStats.profit >= 1000f)
        {
            AwardBusinessPoints(10, "Daily profit $1,000+");
        }
        else if (todayStats.profit >= 500f)
        {
            AwardBusinessPoints(5, "Daily profit $500+");
        }

        if (todayStats.incidentCount == 0)
        {
            AwardBusinessPoints(5, "Perfect day - no incidents");
        }
    }

    /// <summary>
    /// End the day and archive stats.
    /// </summary>
    public void EndDay()
    {
        CheckDailyMilestones();
        statsHistory.Add(todayStats);
        
        // Keep only last 30 days of history
        while (statsHistory.Count > 30)
        {
            statsHistory.RemoveAt(0);
        }

        todayStats = new DailyStats();
        isOpen = false;
        currentCustomers = 0;
    }

    /// <summary>
    /// Check if tier upgrade requirements are met.
    /// </summary>
    public bool CanUpgradeTier(int requiredBP, float requiredCash)
    {
        return businessPoints >= requiredBP && cashBalance >= requiredCash;
    }
}

/// <summary>
/// Daily statistics tracking for a business.
/// </summary>
[System.Serializable]
public class DailyStats
{
    public int dayNumber;
    public float revenue;
    public float expenses;
    public float profit => revenue - expenses;
    public int customersServed;
    public int tasksCompleted;
    public int incidentCount;
    public float averageServiceTime;
    public float customerSatisfaction;

    public DailyStats()
    {
        dayNumber = TimeSystem.Instance != null ? TimeSystem.Instance.Day : 0;
        revenue = 0f;
        expenses = 0f;
        customersServed = 0;
        tasksCompleted = 0;
        incidentCount = 0;
        averageServiceTime = 0f;
        customerSatisfaction = 0f;
    }

    public void RecordSale(float amount)
    {
        revenue += amount;
        customersServed++;
    }

    public void RecordExpense(float amount)
    {
        expenses += amount;
    }

    public void RecordIncident()
    {
        incidentCount++;
    }
}

/// <summary>
/// Enum for all business types in the game.
/// </summary>
public enum BusinessType
{
    Hypermarket,
    RetailFashion,
    Restaurant,
    Construction,
    TaxiCompany
}

/// <summary>
/// Business tier configuration data.
/// </summary>
[System.Serializable]
public class BusinessTierConfig
{
    public int tier;
    public string tierName;
    public int floorSpace;
    public int maxStaff;
    public int maxCustomers;
    public int maxProductSKUs;
    public int requiredBP;
    public float requiredCash;
    public string[] unlockedFeatures;
}
