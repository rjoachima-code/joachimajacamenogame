using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model for a home/housing option.
/// </summary>
[Serializable]
public class HomeData
{
    public string homeId;
    public string homeName;
    public DistrictType district;
    public int rentCost;
    public int roomCount;
    public float qualityRating;
    public Vector3 location;
    public bool isAvailable;

    public HomeData(string id, string name, DistrictType district, int rent)
    {
        homeId = id;
        homeName = name;
        this.district = district;
        rentCost = rent;
        roomCount = 1;
        qualityRating = 1.0f;
        isAvailable = true;
    }
}

/// <summary>
/// ScriptableObject for home configuration.
/// </summary>
[CreateAssetMenu(fileName = "NewHome", menuName = "Jacameno/Home Data")]
public class HomeDataAsset : ScriptableObject
{
    [Header("Basic Info")]
    public string homeId;
    public string homeName;
    [TextArea(2, 4)]
    public string description;
    public DistrictType district;

    [Header("Costs")]
    public int rentCost = 500;
    public int depositCost = 1000;

    [Header("Features")]
    public int roomCount = 1;
    public int bathroomCount = 1;
    public float squareFootage = 500f;
    public float qualityRating = 1.0f;

    [Header("Location")]
    public Vector3 location;
    public string nearestTrainStation;

    [Header("Amenities")]
    public bool hasKitchen = true;
    public bool hasLaundry = false;
    public bool hasParking = false;
    public bool hasPets = false;

    [Header("Status")]
    public bool isAvailable = true;
}

/// <summary>
/// System for managing home selection and housing.
/// </summary>
public class HomeSelection : MonoBehaviour
{
    public static HomeSelection Instance { get; private set; }

    [Header("Available Homes")]
    [SerializeField] private HomeDataAsset[] availableHomes;

    [Header("Current Home")]
    [SerializeField] private HomeDataAsset currentHome;

    public event Action<HomeDataAsset> OnHomeSelected;
    public event Action<HomeDataAsset> OnHomePurchased;
    public event Action OnRentDue;

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
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnNewDay += CheckRentDue;
        }
    }

    private void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnNewDay -= CheckRentDue;
        }
    }

    private void CheckRentDue()
    {
        // Check if rent is due (e.g., every 7 days)
        if (TimeSystem.Instance != null && TimeSystem.Instance.Day % 7 == 0)
        {
            OnRentDue?.Invoke();
        }
    }

    /// <summary>
    /// Gets all available homes.
    /// </summary>
    public HomeDataAsset[] GetAvailableHomes()
    {
        return availableHomes;
    }

    /// <summary>
    /// Gets available homes in a specific district.
    /// </summary>
    public HomeDataAsset[] GetHomesInDistrict(DistrictType district)
    {
        var result = new List<HomeDataAsset>();
        foreach (var home in availableHomes)
        {
            if (home != null && home.district == district && home.isAvailable)
            {
                result.Add(home);
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets the current home.
    /// </summary>
    public HomeDataAsset GetCurrentHome()
    {
        return currentHome;
    }

    /// <summary>
    /// Selects a home for viewing.
    /// </summary>
    public void SelectHome(HomeDataAsset home)
    {
        OnHomeSelected?.Invoke(home);
    }

    /// <summary>
    /// Attempts to rent a home.
    /// </summary>
    public bool RentHome(HomeDataAsset home)
    {
        if (home == null || !home.isAvailable) return false;

        int totalCost = home.rentCost + home.depositCost;

        if (MoneyManager.Instance != null && MoneyManager.Instance.balance < totalCost)
        {
            return false;
        }

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.Withdraw(totalCost, $"Housing: {home.homeName}");
        }

        // If player had a previous home, mark it as available
        if (currentHome != null)
        {
            currentHome.isAvailable = true;
        }

        currentHome = home;
        home.isAvailable = false;

        OnHomePurchased?.Invoke(home);
        return true;
    }

    /// <summary>
    /// Pays rent for the current home.
    /// </summary>
    public bool PayRent()
    {
        if (currentHome == null) return false;

        if (MoneyManager.Instance != null && MoneyManager.Instance.balance >= currentHome.rentCost)
        {
            MoneyManager.Instance.Withdraw(currentHome.rentCost, $"Rent: {currentHome.homeName}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the rent cost for the current home.
    /// </summary>
    public int GetCurrentRentCost()
    {
        return currentHome != null ? currentHome.rentCost : 0;
    }

    /// <summary>
    /// Checks if player has a home.
    /// </summary>
    public bool HasHome()
    {
        return currentHome != null;
    }

    /// <summary>
    /// Gets homes sorted by price.
    /// </summary>
    public HomeDataAsset[] GetHomesSortedByPrice(bool ascending = true)
    {
        var homes = new List<HomeDataAsset>(availableHomes);
        homes.Sort((a, b) =>
        {
            int comparison = a.rentCost.CompareTo(b.rentCost);
            return ascending ? comparison : -comparison;
        });
        return homes.ToArray();
    }
}
