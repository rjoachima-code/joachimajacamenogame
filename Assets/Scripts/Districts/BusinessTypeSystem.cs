using UnityEngine;

/// <summary>
/// Enum representing the types of starter businesses available.
/// </summary>
public enum BusinessCategory
{
    Food,
    Retail,
    Service,
    Entertainment,
    Technology,
    Manufacturing
}

/// <summary>
/// Data model for a business type.
/// </summary>
[System.Serializable]
public class BusinessType
{
    public string businessId;
    public string businessName;
    public BusinessCategory category;
    public int startupCost;
    public int dailyOperatingCost;
    public int potentialDailyRevenue;
    public DistrictType[] allowedDistricts;
    public string[] requiredSkills;
    public int requiredLevel;

    public BusinessType(string id, string name, BusinessCategory category, int startupCost)
    {
        businessId = id;
        businessName = name;
        this.category = category;
        this.startupCost = startupCost;
        dailyOperatingCost = startupCost / 10;
        potentialDailyRevenue = startupCost / 5;
        allowedDistricts = new DistrictType[0];
        requiredSkills = new string[0];
        requiredLevel = 1;
    }
}

/// <summary>
/// ScriptableObject for business type configuration.
/// </summary>
[CreateAssetMenu(fileName = "NewBusinessType", menuName = "Jacameno/Business Type")]
public class BusinessTypeData : ScriptableObject
{
    [Header("Basic Info")]
    public string businessId;
    public string businessName;
    [TextArea(2, 4)]
    public string description;
    public BusinessCategory category;

    [Header("Costs")]
    public int startupCost = 1000;
    public int dailyOperatingCost = 100;
    public int potentialDailyRevenue = 200;

    [Header("Requirements")]
    public DistrictType[] allowedDistricts;
    public string[] requiredSkills;
    public int requiredLevel = 1;

    [Header("Upgrades")]
    public int maxUpgradeLevel = 5;
    public int upgradeBaseCost = 500;
}

/// <summary>
/// Manager for business type system.
/// </summary>
public class BusinessTypeSystem : MonoBehaviour
{
    public static BusinessTypeSystem Instance { get; private set; }

    [SerializeField] private BusinessTypeData[] availableBusinessTypes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Gets all available business types.
    /// </summary>
    public BusinessTypeData[] GetAllBusinessTypes()
    {
        return availableBusinessTypes;
    }

    /// <summary>
    /// Gets business types available in a specific district.
    /// </summary>
    public BusinessTypeData[] GetBusinessTypesForDistrict(DistrictType district)
    {
        var result = new System.Collections.Generic.List<BusinessTypeData>();
        foreach (var business in availableBusinessTypes)
        {
            if (business == null) continue;
            if (business.allowedDistricts == null || business.allowedDistricts.Length == 0)
            {
                result.Add(business);
                continue;
            }
            foreach (var allowedDistrict in business.allowedDistricts)
            {
                if (allowedDistrict == district)
                {
                    result.Add(business);
                    break;
                }
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets business types by category.
    /// </summary>
    public BusinessTypeData[] GetBusinessTypesByCategory(BusinessCategory category)
    {
        var result = new System.Collections.Generic.List<BusinessTypeData>();
        foreach (var business in availableBusinessTypes)
        {
            if (business != null && business.category == category)
            {
                result.Add(business);
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Checks if a player can start a business.
    /// </summary>
    public bool CanStartBusiness(BusinessTypeData businessType, int playerMoney, int playerLevel)
    {
        if (businessType == null) return false;
        if (playerMoney < businessType.startupCost) return false;
        if (playerLevel < businessType.requiredLevel) return false;
        return true;
    }
}
