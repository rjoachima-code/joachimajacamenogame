using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for district-specific features and data.
/// </summary>
public class DistrictManager : MonoBehaviour
{
    public static DistrictManager Instance { get; private set; }

    [Header("District Data")]
    [SerializeField] private DistrictData[] districts;

    [Header("Current State")]
    [SerializeField] private DistrictType currentDistrict = DistrictType.Fame;

    public event Action<DistrictType> OnDistrictChanged;
    public event Action<DistrictData> OnDistrictDataLoaded;

    private Dictionary<DistrictType, DistrictData> districtLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeDistrictLookup();
    }

    private void InitializeDistrictLookup()
    {
        districtLookup = new Dictionary<DistrictType, DistrictData>();
        if (districts != null)
        {
            foreach (var district in districts)
            {
                if (district != null && !districtLookup.ContainsKey(district.districtType))
                {
                    districtLookup[district.districtType] = district;
                }
            }
        }
    }

    /// <summary>
    /// Gets the current district type.
    /// </summary>
    public DistrictType GetCurrentDistrict()
    {
        return currentDistrict;
    }

    /// <summary>
    /// Gets the data for the current district.
    /// </summary>
    public DistrictData GetCurrentDistrictData()
    {
        return GetDistrictData(currentDistrict);
    }

    /// <summary>
    /// Gets the data for a specific district.
    /// </summary>
    public DistrictData GetDistrictData(DistrictType district)
    {
        if (districtLookup != null && districtLookup.TryGetValue(district, out var data))
        {
            return data;
        }
        return null;
    }

    /// <summary>
    /// Changes the current district.
    /// </summary>
    public void ChangeDistrict(DistrictType newDistrict)
    {
        if (currentDistrict == newDistrict) return;

        currentDistrict = newDistrict;
        OnDistrictChanged?.Invoke(currentDistrict);

        var districtData = GetDistrictData(currentDistrict);
        if (districtData != null)
        {
            OnDistrictDataLoaded?.Invoke(districtData);
        }
    }

    /// <summary>
    /// Gets the base rent cost for a district.
    /// </summary>
    public int GetBaseRentCost(DistrictType district)
    {
        var data = GetDistrictData(district);
        return data != null ? data.baseRentCost : 500;
    }

    /// <summary>
    /// Gets the cost of living multiplier for a district.
    /// </summary>
    public float GetCostOfLivingMultiplier(DistrictType district)
    {
        var data = GetDistrictData(district);
        return data != null ? data.costOfLivingMultiplier : 1.0f;
    }

    /// <summary>
    /// Gets the available business types for a district.
    /// </summary>
    public string[] GetAvailableBusinessTypes(DistrictType district)
    {
        var data = GetDistrictData(district);
        return data != null ? data.availableBusinessTypes : new string[0];
    }

    /// <summary>
    /// Gets all districts.
    /// </summary>
    public DistrictData[] GetAllDistricts()
    {
        return districts;
    }

    /// <summary>
    /// Gets the district color.
    /// </summary>
    public Color GetDistrictColor(DistrictType district)
    {
        var data = GetDistrictData(district);
        return data != null ? data.districtColor : Color.white;
    }
}
