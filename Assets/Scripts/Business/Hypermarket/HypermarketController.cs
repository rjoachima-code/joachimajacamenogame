using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hypermarket Business Controller - Manages all Hypermarket-specific operations.
/// Includes department management, checkout, stocking, and mini-game integration.
/// </summary>
public class HypermarketController : MonoBehaviour
{
    public static HypermarketController Instance { get; private set; }

    [Header("Business Reference")]
    [SerializeField] private string businessId;
    private BusinessState businessState;

    [Header("Departments")]
    [SerializeField] private List<HypermarketDepartment> departments = new List<HypermarketDepartment>();
    [SerializeField] private List<string> unlockedDepartments = new List<string> { "grocery" };

    [Header("Checkout")]
    [SerializeField] private List<CheckoutLane> checkoutLanes = new List<CheckoutLane>();
    [SerializeField] private int maxCheckoutLanes = 1;

    [Header("Customer Queue")]
    [SerializeField] private Queue<HypermarketCustomer> customerQueue = new Queue<HypermarketCustomer>();
    [SerializeField] private List<HypermarketCustomer> activeCustomers = new List<HypermarketCustomer>();

    [Header("Tier Configuration")]
    [SerializeField] private HypermarketTierConfig[] tierConfigs;

    // Events
    public event Action<HypermarketCustomer> OnCustomerEntered;
    public event Action<HypermarketCustomer> OnCustomerServed;
    public event Action<HypermarketCustomer> OnCustomerLeft;
    public event Action<string> OnDepartmentUnlocked;
    public event Action<int> OnTierUpgraded;

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
        // Register with business manager
        if (BusinessManager.Instance != null)
        {
            businessState = BusinessManager.Instance.CreateBusiness(
                BusinessType.Hypermarket, 
                "My Hypermarket"
            );
            businessId = businessState.businessId;
        }

        InitializeDepartments();
        InitializeCheckoutLanes();
    }

    private void Update()
    {
        if (businessState == null || !businessState.isOpen) return;

        ProcessCustomerQueue();
        UpdateDepartments();
    }

    /// <summary>
    /// Initialize tier configurations.
    /// </summary>
    private void InitializeTierConfigs()
    {
        tierConfigs = new HypermarketTierConfig[]
        {
            new HypermarketTierConfig
            {
                tier = 1,
                tierName = "Corner Store",
                floorSpace = 500,
                maxDepartments = 1,
                maxStaff = 0,
                maxCheckoutLanes = 1,
                maxProductSKUs = 50,
                dailyCustomerCap = 30,
                requiredBP = 0,
                requiredCash = 0
            },
            new HypermarketTierConfig
            {
                tier = 2,
                tierName = "Neighborhood Market",
                floorSpace = 1500,
                maxDepartments = 2,
                maxStaff = 2,
                maxCheckoutLanes = 2,
                maxProductSKUs = 150,
                dailyCustomerCap = 75,
                requiredBP = 100,
                requiredCash = 5000
            },
            new HypermarketTierConfig
            {
                tier = 3,
                tierName = "Community Supermarket",
                floorSpace = 4000,
                maxDepartments = 4,
                maxStaff = 6,
                maxCheckoutLanes = 4,
                maxProductSKUs = 400,
                dailyCustomerCap = 200,
                requiredBP = 250,
                requiredCash = 15000
            },
            new HypermarketTierConfig
            {
                tier = 4,
                tierName = "District Hypermarket",
                floorSpace = 10000,
                maxDepartments = 6,
                maxStaff = 15,
                maxCheckoutLanes = 8,
                maxProductSKUs = 1000,
                dailyCustomerCap = 500,
                requiredBP = 500,
                requiredCash = 50000
            },
            new HypermarketTierConfig
            {
                tier = 5,
                tierName = "Regional Megamart",
                floorSpace = 25000,
                maxDepartments = 10,
                maxStaff = 40,
                maxCheckoutLanes = 15,
                maxProductSKUs = 5000,
                dailyCustomerCap = 1500,
                requiredBP = 1000,
                requiredCash = 150000
            }
        };
    }

    /// <summary>
    /// Initialize departments based on unlocked list.
    /// </summary>
    private void InitializeDepartments()
    {
        departments.Clear();

        if (unlockedDepartments.Contains("grocery"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "grocery",
                departmentName = "Grocery",
                productCategories = new List<string> { "canned_goods", "snacks", "beverages", "condiments" }
            });
        }

        if (unlockedDepartments.Contains("fresh_produce"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "fresh_produce",
                departmentName = "Fresh Produce",
                productCategories = new List<string> { "fruits", "vegetables", "herbs" },
                requiresRefrigeration = true
            });
        }

        if (unlockedDepartments.Contains("bakery"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "bakery",
                departmentName = "Bakery",
                productCategories = new List<string> { "bread", "pastries", "cakes" },
                requiresEquipment = new List<string> { "bakery_oven" }
            });
        }

        if (unlockedDepartments.Contains("deli"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "deli",
                departmentName = "Deli",
                productCategories = new List<string> { "meats", "cheeses", "prepared_foods" },
                requiresEquipment = new List<string> { "deli_slicer", "refrigeration" }
            });
        }

        if (unlockedDepartments.Contains("dairy"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "dairy",
                departmentName = "Dairy",
                productCategories = new List<string> { "milk", "cheese", "yogurt", "eggs" },
                requiresRefrigeration = true
            });
        }

        if (unlockedDepartments.Contains("household"))
        {
            departments.Add(new HypermarketDepartment
            {
                departmentId = "household",
                departmentName = "Household",
                productCategories = new List<string> { "cleaning", "paper_goods", "home_essentials" }
            });
        }
    }

    /// <summary>
    /// Initialize checkout lanes based on tier.
    /// </summary>
    private void InitializeCheckoutLanes()
    {
        checkoutLanes.Clear();
        for (int i = 0; i < maxCheckoutLanes; i++)
        {
            checkoutLanes.Add(new CheckoutLane
            {
                laneId = $"lane_{i + 1}",
                laneName = $"Lane {i + 1}",
                isOpen = i == 0, // Only first lane open by default
                isExpress = false
            });
        }
    }

    /// <summary>
    /// Unlock a new department.
    /// </summary>
    public bool UnlockDepartment(string departmentId, int bpCost, float cashCost)
    {
        if (unlockedDepartments.Contains(departmentId)) return false;

        if (businessState.businessPoints < bpCost || businessState.cashBalance < cashCost)
        {
            Debug.LogWarning("[Hypermarket] Insufficient resources to unlock department");
            return false;
        }

        businessState.businessPoints -= bpCost;
        businessState.cashBalance -= cashCost;
        unlockedDepartments.Add(departmentId);

        InitializeDepartments();
        OnDepartmentUnlocked?.Invoke(departmentId);

        Debug.Log($"[Hypermarket] Unlocked department: {departmentId}");
        return true;
    }

    /// <summary>
    /// Open the store for business.
    /// </summary>
    public void OpenStore()
    {
        if (businessState != null)
        {
            businessState.isOpen = true;
            BusinessManager.Instance?.OpenBusiness(businessId);
            Debug.Log("[Hypermarket] Store opened");
        }
    }

    /// <summary>
    /// Close the store.
    /// </summary>
    public void CloseStore()
    {
        if (businessState != null)
        {
            businessState.isOpen = false;
            BusinessManager.Instance?.CloseBusiness(businessId);
            Debug.Log("[Hypermarket] Store closed");
        }
    }

    /// <summary>
    /// Spawn a new customer.
    /// </summary>
    public void SpawnCustomer()
    {
        if (!businessState.isOpen) return;

        var tierConfig = GetCurrentTierConfig();
        if (activeCustomers.Count >= tierConfig.dailyCustomerCap) return;

        var customer = new HypermarketCustomer
        {
            customerId = Guid.NewGuid().ToString(),
            arrivalTime = DateTime.Now,
            shoppingList = GenerateShoppingList(),
            patience = UnityEngine.Random.Range(60f, 180f)
        };

        activeCustomers.Add(customer);
        OnCustomerEntered?.Invoke(customer);
    }

    /// <summary>
    /// Generate a random shopping list for a customer.
    /// </summary>
    private List<ShoppingListItem> GenerateShoppingList()
    {
        var list = new List<ShoppingListItem>();
        int itemCount = UnityEngine.Random.Range(3, 15);

        // Get products from unlocked departments
        // This would be connected to the inventory system
        for (int i = 0; i < itemCount; i++)
        {
            list.Add(new ShoppingListItem
            {
                productId = $"product_{i}",
                quantity = UnityEngine.Random.Range(1, 5)
            });
        }

        return list;
    }

    /// <summary>
    /// Process customer queue and checkout.
    /// </summary>
    private void ProcessCustomerQueue()
    {
        // Move customers ready for checkout to queue
        var readyCustomers = activeCustomers.FindAll(c => c.isReadyForCheckout && !c.isInQueue);
        foreach (var customer in readyCustomers)
        {
            customer.isInQueue = true;
            customerQueue.Enqueue(customer);
        }

        // Process checkout lanes
        foreach (var lane in checkoutLanes)
        {
            if (!lane.isOpen || lane.currentCustomer != null) continue;

            if (customerQueue.Count > 0)
            {
                lane.currentCustomer = customerQueue.Dequeue();
                // Start checkout process
            }
        }
    }

    /// <summary>
    /// Complete a checkout transaction.
    /// </summary>
    public void CompleteCheckout(string laneId, float transactionAmount)
    {
        var lane = checkoutLanes.Find(l => l.laneId == laneId);
        if (lane == null || lane.currentCustomer == null) return;

        var customer = lane.currentCustomer;
        
        // Record the sale
        BusinessManager.Instance?.RecordSale(businessId, transactionAmount);

        // Update satisfaction based on wait time
        float waitTime = (float)(DateTime.Now - customer.queueStartTime).TotalSeconds;
        float satisfaction = Mathf.Clamp01(1f - (waitTime / customer.patience));
        customer.satisfaction = satisfaction;

        // Update reputation
        businessState.serviceSpeedScore = Mathf.Lerp(
            businessState.serviceSpeedScore,
            satisfaction * 5f,
            0.1f
        );
        businessState.CalculateReputation();

        activeCustomers.Remove(customer);
        lane.currentCustomer = null;

        OnCustomerServed?.Invoke(customer);
        Debug.Log($"[Hypermarket] Customer served. Transaction: ${transactionAmount:F2}");
    }

    /// <summary>
    /// Update department states.
    /// </summary>
    private void UpdateDepartments()
    {
        foreach (var dept in departments)
        {
            // Check stock levels
            // Update cleanliness decay
            dept.cleanliness -= 0.01f * Time.deltaTime;
            dept.cleanliness = Mathf.Max(0f, dept.cleanliness);
        }
    }

    /// <summary>
    /// Get current tier configuration.
    /// </summary>
    public HypermarketTierConfig GetCurrentTierConfig()
    {
        int tier = businessState?.tier ?? 1;
        return tierConfigs[Mathf.Clamp(tier - 1, 0, tierConfigs.Length - 1)];
    }

    /// <summary>
    /// Attempt to upgrade to next tier.
    /// </summary>
    public bool TryUpgradeTier()
    {
        if (businessState == null) return false;

        int nextTier = businessState.tier + 1;
        if (nextTier > tierConfigs.Length) return false;

        var config = tierConfigs[nextTier - 1];
        
        if (BusinessManager.Instance.UpgradeBusiness(
            businessId, 
            config.requiredBP, 
            config.requiredCash))
        {
            maxCheckoutLanes = config.maxCheckoutLanes;
            InitializeCheckoutLanes();
            OnTierUpgraded?.Invoke(nextTier);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get business state.
    /// </summary>
    public BusinessState GetBusinessState()
    {
        return businessState;
    }

    /// <summary>
    /// Generate routine tasks for the day.
    /// </summary>
    public void GenerateDailyTasks()
    {
        if (TaskQueueSystem.Instance == null) return;

        // Stocking tasks
        foreach (var dept in departments)
        {
            TaskQueueSystem.Instance.AddTask(new BusinessTask
            {
                taskName = $"Stock {dept.departmentName}",
                taskType = TaskType.Stocking,
                priority = TaskPriority.High,
                estimatedDuration = 20f,
                deadlineMinutes = 120,
                experienceReward = 15,
                requiredRoles = new List<StaffRole> { StaffRole.Stocker, StaffRole.FreshDepartment }
            });
        }

        // Cleaning tasks
        TaskQueueSystem.Instance.AddTask(new BusinessTask
        {
            taskName = "Morning Floor Clean",
            taskType = TaskType.Cleaning,
            priority = TaskPriority.Normal,
            estimatedDuration = 15f,
            deadlineMinutes = 60,
            experienceReward = 10
        });

        // Register preparation
        foreach (var lane in checkoutLanes.FindAll(l => l.isOpen))
        {
            TaskQueueSystem.Instance.AddTask(new BusinessTask
            {
                taskName = $"Prepare {lane.laneName}",
                taskType = TaskType.RegisterOperation,
                priority = TaskPriority.High,
                estimatedDuration = 5f,
                deadlineMinutes = 30,
                experienceReward = 5
            });
        }
    }

    /// <summary>
    /// Start the stocking mini-game.
    /// </summary>
    public void StartStockingMiniGame(string departmentId)
    {
        var dept = departments.Find(d => d.departmentId == departmentId);
        if (dept == null) return;

        // Trigger mini-game system
        Debug.Log($"[Hypermarket] Starting stocking mini-game for {dept.departmentName}");
        // Integration with HypermarketStockingMiniGame
    }

    /// <summary>
    /// Start the cashier mini-game.
    /// </summary>
    public void StartCashierMiniGame(string laneId)
    {
        var lane = checkoutLanes.Find(l => l.laneId == laneId);
        if (lane == null || lane.currentCustomer == null) return;

        // Trigger mini-game system
        Debug.Log($"[Hypermarket] Starting cashier mini-game at {lane.laneName}");
        // Integration with CashierMiniGame
        if (CashierMiniGame.Instance != null)
        {
            CashierMiniGame.Instance.StartMiniGame();
        }
    }
}

/// <summary>
/// Hypermarket department configuration.
/// </summary>
[System.Serializable]
public class HypermarketDepartment
{
    public string departmentId;
    public string departmentName;
    public List<string> productCategories = new List<string>();
    public bool requiresRefrigeration = false;
    public List<string> requiresEquipment = new List<string>();
    public float cleanliness = 100f;
    public float stockLevel = 100f;
}

/// <summary>
/// Checkout lane configuration.
/// </summary>
[System.Serializable]
public class CheckoutLane
{
    public string laneId;
    public string laneName;
    public bool isOpen = false;
    public bool isExpress = false;
    public int maxItems = -1; // -1 = no limit
    public HypermarketCustomer currentCustomer;
    public string assignedStaffId;
}

/// <summary>
/// Hypermarket customer data.
/// </summary>
[System.Serializable]
public class HypermarketCustomer
{
    public string customerId;
    public List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();
    public List<ShoppingListItem> cart = new List<ShoppingListItem>();
    public DateTime arrivalTime;
    public DateTime queueStartTime;
    public float patience = 120f;
    public float satisfaction = 1f;
    public bool isReadyForCheckout = false;
    public bool isInQueue = false;

    public float GetTotalCartValue()
    {
        float total = 0f;
        foreach (var item in cart)
        {
            total += item.price * item.quantity;
        }
        return total;
    }
}

/// <summary>
/// Shopping list item.
/// </summary>
[System.Serializable]
public class ShoppingListItem
{
    public string productId;
    public string productName;
    public int quantity;
    public float price;
    public bool found = false;
}

/// <summary>
/// Hypermarket tier configuration.
/// </summary>
[System.Serializable]
public class HypermarketTierConfig
{
    public int tier;
    public string tierName;
    public int floorSpace;
    public int maxDepartments;
    public int maxStaff;
    public int maxCheckoutLanes;
    public int maxProductSKUs;
    public int dailyCustomerCap;
    public int requiredBP;
    public float requiredCash;
    public string[] unlockedEquipment;
}
