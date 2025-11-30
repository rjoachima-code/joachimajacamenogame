using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Restaurant Business Controller.
/// Manages kitchen operations, dining service, and food preparation.
/// </summary>
public class RestaurantController : MonoBehaviour
{
    public static RestaurantController Instance { get; private set; }

    [Header("Business Reference")]
    [SerializeField] private string businessId;
    private BusinessState businessState;

    [Header("Kitchen")]
    [SerializeField] private List<KitchenStation> kitchenStations = new List<KitchenStation>();
    [SerializeField] private List<MenuItem> menu = new List<MenuItem>();

    [Header("Dining")]
    [SerializeField] private List<DiningTable> tables = new List<DiningTable>();
    [SerializeField] private Queue<RestaurantCustomer> waitingQueue = new Queue<RestaurantCustomer>();

    [Header("Orders")]
    [SerializeField] private List<FoodOrder> activeOrders = new List<FoodOrder>();
    [SerializeField] private List<FoodOrder> completedOrders = new List<FoodOrder>();

    [Header("Tier Configuration")]
    [SerializeField] private RestaurantTierConfig[] tierConfigs;

    // Events
    public event Action<FoodOrder> OnOrderReceived;
    public event Action<FoodOrder> OnOrderCompleted;
    public event Action<RestaurantCustomer> OnCustomerSeated;

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
                BusinessType.Restaurant,
                "My Restaurant"
            );
            businessId = businessState.businessId;
        }

        InitializeKitchen();
        InitializeDining();
        InitializeMenu();
    }

    private void Update()
    {
        if (businessState == null || !businessState.isOpen) return;
        ProcessOrders();
        UpdateKitchenStations();
    }

    private void InitializeTierConfigs()
    {
        tierConfigs = new RestaurantTierConfig[]
        {
            new RestaurantTierConfig
            {
                tier = 1, tierName = "Food Cart",
                seatingCapacity = 0, menuItems = 5, maxStaff = 0,
                kitchenStations = 1, dailyCovers = 30,
                requiredBP = 0, requiredCash = 0
            },
            new RestaurantTierConfig
            {
                tier = 2, tierName = "Diner",
                seatingCapacity = 20, menuItems = 15, maxStaff = 3,
                kitchenStations = 2, dailyCovers = 80,
                requiredBP = 100, requiredCash = 6000
            },
            new RestaurantTierConfig
            {
                tier = 3, tierName = "Family Restaurant",
                seatingCapacity = 50, menuItems = 35, maxStaff = 8,
                kitchenStations = 4, dailyCovers = 200,
                requiredBP = 250, requiredCash = 20000
            },
            new RestaurantTierConfig
            {
                tier = 4, tierName = "Fine Dining",
                seatingCapacity = 80, menuItems = 50, maxStaff = 20,
                kitchenStations = 6, dailyCovers = 150,
                requiredBP = 500, requiredCash = 60000
            },
            new RestaurantTierConfig
            {
                tier = 5, tierName = "Restaurant Group",
                seatingCapacity = 150, menuItems = 75, maxStaff = 50,
                kitchenStations = 10, dailyCovers = 400,
                requiredBP = 1000, requiredCash = 200000
            }
        };
    }

    private void InitializeKitchen()
    {
        kitchenStations.Clear();
        kitchenStations.Add(new KitchenStation
        {
            stationId = "grill_main",
            stationName = "Main Grill",
            stationType = StationType.Grill,
            efficiency = 1f
        });
    }

    private void InitializeDining()
    {
        var config = GetCurrentTierConfig();
        tables.Clear();
        int tableCount = config.seatingCapacity / 4;
        for (int i = 0; i < tableCount; i++)
        {
            tables.Add(new DiningTable
            {
                tableId = $"table_{i + 1}",
                tableName = $"Table {i + 1}",
                capacity = 4,
                isOccupied = false
            });
        }
    }

    private void InitializeMenu()
    {
        menu.Clear();
        menu.Add(new MenuItem
        {
            itemId = "burger_classic",
            itemName = "Classic Burger",
            category = "Entrees",
            price = 12.99f,
            prepTime = 10f,
            requiredStation = StationType.Grill
        });
    }

    /// <summary>
    /// Seat a customer at an available table.
    /// </summary>
    public bool SeatCustomer(RestaurantCustomer customer)
    {
        var table = tables.Find(t => !t.isOccupied && t.capacity >= customer.partySize);
        if (table == null)
        {
            waitingQueue.Enqueue(customer);
            return false;
        }

        table.isOccupied = true;
        table.currentCustomerId = customer.customerId;
        table.seatedTime = DateTime.Now;
        customer.assignedTable = table.tableId;

        OnCustomerSeated?.Invoke(customer);
        return true;
    }

    /// <summary>
    /// Take an order from a table.
    /// </summary>
    public void TakeOrder(string tableId, List<string> itemIds)
    {
        var table = tables.Find(t => t.tableId == tableId);
        if (table == null) return;

        var order = new FoodOrder
        {
            orderId = Guid.NewGuid().ToString(),
            tableId = tableId,
            items = itemIds,
            orderTime = DateTime.Now,
            status = OrderStatus.Pending
        };

        activeOrders.Add(order);
        OnOrderReceived?.Invoke(order);
    }

    /// <summary>
    /// Start cooking an order item.
    /// </summary>
    public bool StartCooking(string orderId, string itemId)
    {
        var order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null) return false;

        var menuItem = menu.Find(m => m.itemId == itemId);
        if (menuItem == null) return false;

        var station = kitchenStations.Find(s => 
            s.stationType == menuItem.requiredStation && !s.isOccupied);
        if (station == null) return false;

        station.isOccupied = true;
        station.currentOrderId = orderId;
        station.currentItemId = itemId;
        station.cookingStartTime = DateTime.Now;

        return true;
    }

    /// <summary>
    /// Start cooking timing mini-game.
    /// </summary>
    public void StartCookingMiniGame(string stationId)
    {
        Debug.Log($"[Restaurant] Starting cooking mini-game at station {stationId}");
        // Integration with CookingTimingMiniGame
    }

    private void ProcessOrders()
    {
        foreach (var station in kitchenStations)
        {
            if (!station.isOccupied) continue;

            var menuItem = menu.Find(m => m.itemId == station.currentItemId);
            if (menuItem == null) continue;

            float elapsed = (float)(DateTime.Now - station.cookingStartTime).TotalSeconds;
            if (elapsed >= menuItem.prepTime)
            {
                // Item ready
                station.isOccupied = false;
                // Move to completed items
            }
        }
    }

    private void UpdateKitchenStations()
    {
        // Update station efficiency and cleanliness
    }

    public RestaurantTierConfig GetCurrentTierConfig()
    {
        int tier = businessState?.tier ?? 1;
        return tierConfigs[Mathf.Clamp(tier - 1, 0, tierConfigs.Length - 1)];
    }

    public BusinessState GetBusinessState() => businessState;
}

[System.Serializable]
public class KitchenStation
{
    public string stationId;
    public string stationName;
    public StationType stationType;
    public bool isOccupied;
    public string currentOrderId;
    public string currentItemId;
    public DateTime cookingStartTime;
    public float efficiency = 1f;
    public float cleanliness = 100f;
}

public enum StationType
{
    Grill, Fryer, Oven, Prep, Salad, Dessert, Beverage
}

[System.Serializable]
public class MenuItem
{
    public string itemId;
    public string itemName;
    public string description;
    public string category;
    public float price;
    public float prepTime;
    public float cost;
    public StationType requiredStation;
    public List<string> ingredients;
}

[System.Serializable]
public class DiningTable
{
    public string tableId;
    public string tableName;
    public int capacity;
    public bool isOccupied;
    public string currentCustomerId;
    public DateTime seatedTime;
    public bool needsService;
}

[System.Serializable]
public class FoodOrder
{
    public string orderId;
    public string tableId;
    public List<string> items;
    public DateTime orderTime;
    public OrderStatus status;
    public float totalAmount;
}

[System.Serializable]
public class RestaurantCustomer
{
    public string customerId;
    public int partySize;
    public string assignedTable;
    public bool hasOrdered;
    public float satisfaction = 1f;
}

[System.Serializable]
public class RestaurantTierConfig
{
    public int tier;
    public string tierName;
    public int seatingCapacity;
    public int menuItems;
    public int maxStaff;
    public int kitchenStations;
    public int dailyCovers;
    public int requiredBP;
    public float requiredCash;
}
