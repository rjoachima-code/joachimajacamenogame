using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory System for managing business products, stock levels, and orders.
/// </summary>
public class InventorySystem : MonoBehaviour, ISaveable
{
    public static InventorySystem Instance { get; private set; }

    [Header("Inventory Configuration")]
    [SerializeField] private int maxInventorySlots = 100;
    [SerializeField] private float warehouseCapacity = 10000f;

    [Header("Current Inventory")]
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] private List<PurchaseOrder> pendingOrders = new List<PurchaseOrder>();
    [SerializeField] private List<PurchaseOrder> orderHistory = new List<PurchaseOrder>();

    // Events
    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemRemoved;
    public event Action<InventoryItem> OnLowStock;
    public event Action<PurchaseOrder> OnOrderPlaced;
    public event Action<PurchaseOrder> OnOrderReceived;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegisterSaveable(this);
        }
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnregisterSaveable(this);
        }
    }

    /// <summary>
    /// Add an item to inventory.
    /// </summary>
    public bool AddItem(InventoryItem item)
    {
        var existing = inventory.Find(i => i.productId == item.productId);
        if (existing != null)
        {
            existing.quantity += item.quantity;
            existing.lastRestockTime = DateTime.Now;
        }
        else
        {
            if (inventory.Count >= maxInventorySlots)
            {
                Debug.LogWarning("[InventorySystem] Inventory full, cannot add new item type");
                return false;
            }
            inventory.Add(item);
        }

        OnItemAdded?.Invoke(item);
        Debug.Log($"[InventorySystem] Added {item.quantity}x {item.productName}");
        return true;
    }

    /// <summary>
    /// Remove quantity from an item.
    /// </summary>
    public bool RemoveItem(string productId, int quantity)
    {
        var item = inventory.Find(i => i.productId == productId);
        if (item == null || item.quantity < quantity)
        {
            Debug.LogWarning($"[InventorySystem] Insufficient stock for {productId}");
            return false;
        }

        item.quantity -= quantity;

        if (item.quantity <= 0)
        {
            inventory.Remove(item);
            OnItemRemoved?.Invoke(item);
        }

        // Check for low stock
        if (item.quantity > 0 && item.quantity <= item.reorderPoint)
        {
            OnLowStock?.Invoke(item);
        }

        return true;
    }

    /// <summary>
    /// Get current stock level for a product.
    /// </summary>
    public int GetStockLevel(string productId)
    {
        var item = inventory.Find(i => i.productId == productId);
        return item?.quantity ?? 0;
    }

    /// <summary>
    /// Get all items in a category.
    /// </summary>
    public List<InventoryItem> GetItemsByCategory(string category)
    {
        return inventory.FindAll(i => i.category == category);
    }

    /// <summary>
    /// Get all low stock items.
    /// </summary>
    public List<InventoryItem> GetLowStockItems()
    {
        return inventory.FindAll(i => i.quantity <= i.reorderPoint);
    }

    /// <summary>
    /// Get total inventory value.
    /// </summary>
    public float GetTotalInventoryValue()
    {
        float total = 0f;
        foreach (var item in inventory)
        {
            total += item.quantity * item.purchasePrice;
        }
        return total;
    }

    /// <summary>
    /// Get current warehouse usage.
    /// </summary>
    public float GetWarehouseUsage()
    {
        float used = 0f;
        foreach (var item in inventory)
        {
            used += item.quantity * item.unitVolume;
        }
        return used;
    }

    /// <summary>
    /// Place a purchase order with a supplier.
    /// </summary>
    public bool PlaceOrder(PurchaseOrder order)
    {
        // Validate order
        if (order.items.Count == 0)
        {
            Debug.LogWarning("[InventorySystem] Cannot place empty order");
            return false;
        }

        // Check if can afford
        float totalCost = order.GetTotalCost();
        if (MoneyManager.Instance != null && MoneyManager.Instance.balance < totalCost)
        {
            Debug.LogWarning("[InventorySystem] Insufficient funds for order");
            return false;
        }

        // Deduct payment
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.Withdraw(totalCost, $"Inventory Order: {order.orderId}");
        }

        order.orderStatus = OrderStatus.Pending;
        order.orderDate = DateTime.Now;
        
        // Calculate delivery time (could be affected by events)
        order.expectedDelivery = DateTime.Now.AddHours(order.deliveryTimeHours);

        pendingOrders.Add(order);
        OnOrderPlaced?.Invoke(order);

        Debug.Log($"[InventorySystem] Order placed: {order.orderId}, expected delivery: {order.expectedDelivery}");
        return true;
    }

    /// <summary>
    /// Process delivered order and add items to inventory.
    /// </summary>
    public void ReceiveOrder(string orderId)
    {
        var order = pendingOrders.Find(o => o.orderId == orderId);
        if (order == null)
        {
            Debug.LogWarning($"[InventorySystem] Order not found: {orderId}");
            return;
        }

        // Add items to inventory
        foreach (var orderItem in order.items)
        {
            var invItem = new InventoryItem
            {
                productId = orderItem.productId,
                productName = orderItem.productName,
                quantity = orderItem.quantity,
                purchasePrice = orderItem.unitPrice,
                category = orderItem.category
            };
            AddItem(invItem);
        }

        order.orderStatus = OrderStatus.Delivered;
        order.actualDelivery = DateTime.Now;

        pendingOrders.Remove(order);
        orderHistory.Add(order);

        OnOrderReceived?.Invoke(order);
        Debug.Log($"[InventorySystem] Order received: {orderId}");
    }

    /// <summary>
    /// Check for orders ready to be delivered.
    /// </summary>
    public void CheckPendingOrders()
    {
        var readyOrders = pendingOrders.FindAll(o => 
            o.orderStatus == OrderStatus.Pending && 
            DateTime.Now >= o.expectedDelivery);

        foreach (var order in readyOrders)
        {
            order.orderStatus = OrderStatus.ReadyForDelivery;
            Debug.Log($"[InventorySystem] Order ready for delivery: {order.orderId}");
        }
    }

    /// <summary>
    /// Auto-reorder items that are below reorder point.
    /// </summary>
    public void ProcessAutoReorders()
    {
        var lowStock = GetLowStockItems();
        foreach (var item in lowStock)
        {
            if (item.autoReorder)
            {
                var orderItem = new OrderItem
                {
                    productId = item.productId,
                    productName = item.productName,
                    quantity = item.reorderQuantity,
                    unitPrice = item.purchasePrice,
                    category = item.category
                };

                var order = new PurchaseOrder
                {
                    orderId = Guid.NewGuid().ToString(),
                    supplierId = item.preferredSupplier,
                    items = new List<OrderItem> { orderItem }
                };

                PlaceOrder(order);
            }
        }
    }

    /// <summary>
    /// Get all inventory items.
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventory);
    }

    #region Save/Load

    [System.Serializable]
    private class InventorySaveData
    {
        public List<InventoryItem> inventory;
        public List<PurchaseOrder> pendingOrders;
    }

    public string SaveData()
    {
        var data = new InventorySaveData
        {
            inventory = inventory,
            pendingOrders = pendingOrders
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<InventorySaveData>(state);
        if (data != null)
        {
            inventory = data.inventory ?? new List<InventoryItem>();
            pendingOrders = data.pendingOrders ?? new List<PurchaseOrder>();
        }
    }

    #endregion
}

/// <summary>
/// Represents an item in inventory.
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public string productId;
    public string productName;
    public string category;
    public int quantity;
    public float purchasePrice;
    public float sellPrice;
    public float unitVolume = 1f;

    public int reorderPoint = 10;
    public int reorderQuantity = 50;
    public bool autoReorder = false;
    public string preferredSupplier;

    public DateTime lastRestockTime;
    public DateTime expirationDate;
    public bool isPerishable = false;

    public float GetStockPercentage(int maxStock = 100)
    {
        return (float)quantity / maxStock;
    }

    public float GetProfitMargin()
    {
        return sellPrice > 0 ? (sellPrice - purchasePrice) / sellPrice : 0f;
    }
}

/// <summary>
/// Product data for the catalog.
/// </summary>
[System.Serializable]
public class ProductData
{
    public string productId;
    public string productName;
    public string description;
    public string category;
    public string subcategory;
    public float basePrice;
    public float wholesalePrice;
    public float unitVolume;
    public int shelfLife; // days, -1 for non-perishable
    public string[] tags;
    public int tier; // 1-5 quality tier
}

/// <summary>
/// Purchase order for restocking inventory.
/// </summary>
[System.Serializable]
public class PurchaseOrder
{
    public string orderId;
    public string supplierId;
    public List<OrderItem> items = new List<OrderItem>();
    public OrderStatus orderStatus = OrderStatus.Draft;
    public DateTime orderDate;
    public DateTime expectedDelivery;
    public DateTime actualDelivery;
    public float deliveryTimeHours = 24f;
    public float shippingCost = 0f;

    public float GetTotalCost()
    {
        float total = shippingCost;
        foreach (var item in items)
        {
            total += item.quantity * item.unitPrice;
        }
        return total;
    }
}

/// <summary>
/// Item in a purchase order.
/// </summary>
[System.Serializable]
public class OrderItem
{
    public string productId;
    public string productName;
    public string category;
    public int quantity;
    public float unitPrice;
}

/// <summary>
/// Order status enum.
/// </summary>
public enum OrderStatus
{
    Draft,
    Pending,
    Confirmed,
    Shipped,
    ReadyForDelivery,
    Delivered,
    Cancelled,
    Delayed
}
