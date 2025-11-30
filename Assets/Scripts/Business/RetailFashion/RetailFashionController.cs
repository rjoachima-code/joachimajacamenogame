using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Retail Fashion Store Business Controller.
/// Manages fashion inventory, visual merchandising, and styling services.
/// </summary>
public class RetailFashionController : MonoBehaviour
{
    public static RetailFashionController Instance { get; private set; }

    [Header("Business Reference")]
    [SerializeField] private string businessId;
    private BusinessState businessState;

    [Header("Store Layout")]
    [SerializeField] private List<FashionSection> sections = new List<FashionSection>();
    [SerializeField] private List<FittingRoom> fittingRooms = new List<FittingRoom>();
    [SerializeField] private List<DisplayMannequin> mannequins = new List<DisplayMannequin>();

    [Header("Customer Management")]
    [SerializeField] private List<FashionCustomer> activeCustomers = new List<FashionCustomer>();
    [SerializeField] private Queue<FashionCustomer> fittingRoomQueue = new Queue<FashionCustomer>();

    [Header("Tier Configuration")]
    [SerializeField] private RetailFashionTierConfig[] tierConfigs;

    // Events
    public event Action<FashionCustomer> OnCustomerEntered;
    public event Action<FashionCustomer> OnSaleCompleted;
    public event Action<string> OnSectionUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeTierConfigs();
    }

    private void Start()
    {
        if (BusinessManager.Instance != null)
        {
            businessState = BusinessManager.Instance.CreateBusiness(
                BusinessType.RetailFashion,
                "My Fashion Boutique"
            );
            businessId = businessState.businessId;
        }

        InitializeSections();
        InitializeFittingRooms();
    }

    private void InitializeTierConfigs()
    {
        tierConfigs = new RetailFashionTierConfig[]
        {
            new RetailFashionTierConfig
            {
                tier = 1, tierName = "Boutique Corner",
                floorSpace = 400, maxSections = 1, maxStaff = 0,
                fittingRooms = 1, inventoryCapacity = 100,
                dailyCustomerCap = 20, requiredBP = 0, requiredCash = 0
            },
            new RetailFashionTierConfig
            {
                tier = 2, tierName = "Fashion Boutique",
                floorSpace = 1000, maxSections = 2, maxStaff = 2,
                fittingRooms = 2, inventoryCapacity = 300,
                dailyCustomerCap = 50, requiredBP = 75, requiredCash = 4000
            },
            new RetailFashionTierConfig
            {
                tier = 3, tierName = "Style Studio",
                floorSpace = 2500, maxSections = 4, maxStaff = 5,
                fittingRooms = 4, inventoryCapacity = 750,
                dailyCustomerCap = 120, requiredBP = 200, requiredCash = 12000
            },
            new RetailFashionTierConfig
            {
                tier = 4, tierName = "Fashion House",
                floorSpace = 5000, maxSections = 6, maxStaff = 12,
                fittingRooms = 8, inventoryCapacity = 2000,
                dailyCustomerCap = 300, requiredBP = 400, requiredCash = 35000
            },
            new RetailFashionTierConfig
            {
                tier = 5, tierName = "Fashion Empire",
                floorSpace = 10000, maxSections = 10, maxStaff = 30,
                fittingRooms = 15, inventoryCapacity = 5000,
                dailyCustomerCap = 750, requiredBP = 800, requiredCash = 100000
            }
        };
    }

    private void InitializeSections()
    {
        sections.Clear();
        sections.Add(new FashionSection
        {
            sectionId = "casual",
            sectionName = "Casual Wear",
            categories = new List<string> { "tshirts", "jeans", "casual_dresses" }
        });
    }

    private void InitializeFittingRooms()
    {
        var config = GetCurrentTierConfig();
        fittingRooms.Clear();
        for (int i = 0; i < config.fittingRooms; i++)
        {
            fittingRooms.Add(new FittingRoom
            {
                roomId = $"fitting_{i + 1}",
                roomName = $"Fitting Room {i + 1}",
                isAvailable = true
            });
        }
    }

    /// <summary>
    /// Style a mannequin with an outfit.
    /// </summary>
    public void StyleMannequin(string mannequinId, OutfitData outfit)
    {
        var mannequin = mannequins.Find(m => m.mannequinId == mannequinId);
        if (mannequin != null)
        {
            mannequin.currentOutfit = outfit;
            mannequin.lastUpdated = DateTime.Now;
            // Affects ambiance score
            businessState.ambianceScore = Mathf.Min(5f, businessState.ambianceScore + 0.1f);
        }
    }

    /// <summary>
    /// Assign fitting room to customer.
    /// </summary>
    public bool AssignFittingRoom(FashionCustomer customer)
    {
        var room = fittingRooms.Find(r => r.isAvailable);
        if (room == null)
        {
            fittingRoomQueue.Enqueue(customer);
            return false;
        }

        room.isAvailable = false;
        room.currentCustomerId = customer.customerId;
        room.occupiedSince = DateTime.Now;
        customer.assignedFittingRoom = room.roomId;
        return true;
    }

    /// <summary>
    /// Release fitting room.
    /// </summary>
    public void ReleaseFittingRoom(string roomId)
    {
        var room = fittingRooms.Find(r => r.roomId == roomId);
        if (room != null)
        {
            room.isAvailable = true;
            room.currentCustomerId = null;

            // Check queue for waiting customers
            if (fittingRoomQueue.Count > 0)
            {
                var nextCustomer = fittingRoomQueue.Dequeue();
                AssignFittingRoom(nextCustomer);
            }
        }
    }

    /// <summary>
    /// Start style matching mini-game.
    /// </summary>
    public void StartStyleMatchingMiniGame(FashionCustomer customer)
    {
        Debug.Log($"[RetailFashion] Starting style matching for customer preferences: {customer.stylePreference}");
        // Integration with StyleMatchingMiniGame
    }

    public RetailFashionTierConfig GetCurrentTierConfig()
    {
        int tier = businessState?.tier ?? 1;
        return tierConfigs[Mathf.Clamp(tier - 1, 0, tierConfigs.Length - 1)];
    }

    public BusinessState GetBusinessState() => businessState;
}

[System.Serializable]
public class FashionSection
{
    public string sectionId;
    public string sectionName;
    public List<string> categories;
    public float displayQuality = 1f;
}

[System.Serializable]
public class FittingRoom
{
    public string roomId;
    public string roomName;
    public bool isAvailable = true;
    public bool isVIP = false;
    public string currentCustomerId;
    public DateTime occupiedSince;
}

[System.Serializable]
public class DisplayMannequin
{
    public string mannequinId;
    public string location;
    public OutfitData currentOutfit;
    public DateTime lastUpdated;
}

[System.Serializable]
public class OutfitData
{
    public string outfitId;
    public List<string> itemIds;
    public string theme;
    public string season;
}

[System.Serializable]
public class FashionCustomer
{
    public string customerId;
    public string stylePreference;
    public string sizeCategory;
    public float budget;
    public List<string> itemsToTry;
    public List<string> purchasedItems;
    public string assignedFittingRoom;
    public float satisfaction = 1f;
}

[System.Serializable]
public class RetailFashionTierConfig
{
    public int tier;
    public string tierName;
    public int floorSpace;
    public int maxSections;
    public int maxStaff;
    public int fittingRooms;
    public int inventoryCapacity;
    public int dailyCustomerCap;
    public int requiredBP;
    public float requiredCash;
}
