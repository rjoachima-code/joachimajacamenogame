using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hypermarket Stocking Mini-Game - Tetris-style shelf arrangement.
/// Player places products on shelves in optimal arrangement for points.
/// </summary>
public class HypermarketStockingMiniGame : MonoBehaviour
{
    public static HypermarketStockingMiniGame Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 120f;
    [SerializeField] private int productsToStock = 20;
    [SerializeField] private int shelfSlots = 12;
    [SerializeField] private float productSpawnInterval = 3f;

    [Header("UI Elements")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text currentProductText;
    [SerializeField] private TMP_Text categoryBonusText;
    [SerializeField] private Transform shelfContainer;
    [SerializeField] private Transform productQueueContainer;
    [SerializeField] private GameObject shelfSlotPrefab;
    [SerializeField] private GameObject productPrefab;

    [Header("Products")]
    [SerializeField] private ProductStockData[] productTypes;

    [Header("Scoring")]
    [SerializeField] private int basePointsPerProduct = 10;
    [SerializeField] private int categoryBonusPoints = 5;
    [SerializeField] private int perfectPlacementBonus = 15;
    [SerializeField] private int speedBonusThreshold = 2; // seconds
    [SerializeField] private int speedBonusPoints = 5;

    // Game state
    private bool isPlaying = false;
    private float currentTime;
    private int currentScore = 0;
    private int productsStocked = 0;
    private ProductStockData currentProduct;
    private Queue<ProductStockData> productQueue = new Queue<ProductStockData>();

    // Shelf state
    private ShelfSlot[] shelfSlots_array;
    private float lastProductTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        InitializeDefaultProducts();
    }

    private void InitializeDefaultProducts()
    {
        if (productTypes == null || productTypes.Length == 0)
        {
            productTypes = new ProductStockData[]
            {
                new ProductStockData { productId = "apple", productName = "Apples", category = "produce", color = Color.red, size = 1 },
                new ProductStockData { productId = "banana", productName = "Bananas", category = "produce", color = Color.yellow, size = 1 },
                new ProductStockData { productId = "orange", productName = "Oranges", category = "produce", color = new Color(1f, 0.5f, 0f), size = 1 },
                new ProductStockData { productId = "milk", productName = "Milk", category = "dairy", color = Color.white, size = 2 },
                new ProductStockData { productId = "cheese", productName = "Cheese", category = "dairy", color = Color.yellow, size = 1 },
                new ProductStockData { productId = "bread", productName = "Bread", category = "bakery", color = new Color(0.8f, 0.6f, 0.4f), size = 2 },
                new ProductStockData { productId = "cereal", productName = "Cereal", category = "grocery", color = Color.cyan, size = 2 },
                new ProductStockData { productId = "soup", productName = "Soup Can", category = "grocery", color = Color.red, size = 1 },
                new ProductStockData { productId = "pasta", productName = "Pasta", category = "grocery", color = Color.yellow, size = 1 },
                new ProductStockData { productId = "juice", productName = "Juice", category = "beverages", color = new Color(1f, 0.5f, 0f), size = 2 }
            };
        }
    }

    /// <summary>
    /// Start the stocking mini-game.
    /// </summary>
    public void StartMiniGame()
    {
        if (isPlaying) return;

        isPlaying = true;
        currentTime = gameDuration;
        currentScore = 0;
        productsStocked = 0;
        productQueue.Clear();

        InitializeShelf();
        GenerateProductQueue();
        GetNextProduct();

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        StartCoroutine(GameLoop());
        Debug.Log("[StockingMiniGame] Game started");
    }

    /// <summary>
    /// Initialize the shelf slots.
    /// </summary>
    private void InitializeShelf()
    {
        if (shelfContainer == null) return;

        // Clear existing slots
        foreach (Transform child in shelfContainer)
        {
            Destroy(child.gameObject);
        }

        shelfSlots_array = new ShelfSlot[shelfSlots];
        for (int i = 0; i < shelfSlots; i++)
        {
            GameObject slotObj;
            if (shelfSlotPrefab != null)
            {
                slotObj = Instantiate(shelfSlotPrefab, shelfContainer);
            }
            else
            {
                slotObj = new GameObject($"Slot_{i}");
                slotObj.transform.SetParent(shelfContainer);
            }

            shelfSlots_array[i] = new ShelfSlot
            {
                slotIndex = i,
                slotObject = slotObj,
                isEmpty = true,
                productCategory = ""
            };

            // Add click handler
            int index = i;
            var button = slotObj.GetComponent<Button>();
            if (button == null)
            {
                button = slotObj.AddComponent<Button>();
            }
            button.onClick.AddListener(() => OnSlotClicked(index));
        }
    }

    /// <summary>
    /// Generate queue of products to stock.
    /// </summary>
    private void GenerateProductQueue()
    {
        for (int i = 0; i < productsToStock; i++)
        {
            var randomProduct = productTypes[Random.Range(0, productTypes.Length)];
            productQueue.Enqueue(randomProduct);
        }
    }

    /// <summary>
    /// Get the next product from queue.
    /// </summary>
    private void GetNextProduct()
    {
        if (productQueue.Count == 0)
        {
            currentProduct = null;
            return;
        }

        currentProduct = productQueue.Dequeue();
        lastProductTime = Time.time;

        if (currentProductText != null)
        {
            currentProductText.text = $"Place: {currentProduct.productName}\nCategory: {currentProduct.category}";
        }

        UpdateProductQueueDisplay();
    }

    /// <summary>
    /// Handle slot click - attempt to place product.
    /// </summary>
    private void OnSlotClicked(int slotIndex)
    {
        if (!isPlaying || currentProduct == null) return;
        if (slotIndex < 0 || slotIndex >= shelfSlots_array.Length) return;

        var slot = shelfSlots_array[slotIndex];

        // Check if slot is available
        if (!slot.isEmpty)
        {
            // Can't place here - slot occupied
            ShowFeedback("Slot occupied!", Color.red);
            return;
        }

        // Check if product fits (for larger products)
        if (currentProduct.size > 1)
        {
            for (int i = 0; i < currentProduct.size; i++)
            {
                int checkIndex = slotIndex + i;
                if (checkIndex >= shelfSlots_array.Length || !shelfSlots_array[checkIndex].isEmpty)
                {
                    ShowFeedback("Not enough space!", Color.red);
                    return;
                }
            }
        }

        // Place the product
        PlaceProduct(slotIndex);
    }

    /// <summary>
    /// Place product on shelf and calculate score.
    /// </summary>
    private void PlaceProduct(int slotIndex)
    {
        int pointsEarned = basePointsPerProduct;
        string bonusText = "";

        // Mark slots as occupied
        for (int i = 0; i < currentProduct.size; i++)
        {
            var slot = shelfSlots_array[slotIndex + i];
            slot.isEmpty = false;
            slot.productCategory = currentProduct.category;
            slot.productId = currentProduct.productId;

            // Visual update
            if (slot.slotObject != null)
            {
                var image = slot.slotObject.GetComponent<Image>();
                if (image != null)
                {
                    image.color = currentProduct.color;
                }
            }
        }

        // Category bonus - check adjacent slots for same category
        int adjacentSameCategory = CountAdjacentSameCategory(slotIndex, currentProduct.category);
        if (adjacentSameCategory > 0)
        {
            int categoryBonus = categoryBonusPoints * adjacentSameCategory;
            pointsEarned += categoryBonus;
            bonusText += $"+{categoryBonus} Category Bonus! ";
        }

        // Speed bonus
        float placementTime = Time.time - lastProductTime;
        if (placementTime < speedBonusThreshold)
        {
            pointsEarned += speedBonusPoints;
            bonusText += $"+{speedBonusPoints} Speed Bonus! ";
        }

        // Perfect placement - all products of same category together
        if (IsPerfectPlacement(slotIndex, currentProduct.category))
        {
            pointsEarned += perfectPlacementBonus;
            bonusText += $"+{perfectPlacementBonus} Perfect! ";
        }

        currentScore += pointsEarned;
        productsStocked++;

        if (categoryBonusText != null)
        {
            categoryBonusText.text = string.IsNullOrEmpty(bonusText) ? $"+{pointsEarned}" : bonusText;
            StartCoroutine(FadeBonusText());
        }

        UpdateUI();
        ShowFeedback($"+{pointsEarned}", Color.green);

        // Get next product
        GetNextProduct();
    }

    /// <summary>
    /// Count adjacent slots with same category.
    /// </summary>
    private int CountAdjacentSameCategory(int slotIndex, string category)
    {
        int count = 0;

        // Check left
        if (slotIndex > 0 && shelfSlots_array[slotIndex - 1].productCategory == category)
        {
            count++;
        }

        // Check right
        if (slotIndex < shelfSlots_array.Length - 1 && shelfSlots_array[slotIndex + 1].productCategory == category)
        {
            count++;
        }

        return count;
    }

    /// <summary>
    /// Check if placement creates a perfect grouping.
    /// </summary>
    private bool IsPerfectPlacement(int slotIndex, string category)
    {
        // Check if this creates a continuous group of same category
        int groupStart = slotIndex;
        int groupEnd = slotIndex;

        // Find group start
        while (groupStart > 0 && shelfSlots_array[groupStart - 1].productCategory == category)
        {
            groupStart--;
        }

        // Find group end
        while (groupEnd < shelfSlots_array.Length - 1 && shelfSlots_array[groupEnd + 1].productCategory == category)
        {
            groupEnd++;
        }

        // Check if all items of this category are in this group
        int categoryCount = 0;
        int groupCount = groupEnd - groupStart + 1;

        foreach (var slot in shelfSlots_array)
        {
            if (slot.productCategory == category)
            {
                categoryCount++;
            }
        }

        return categoryCount == groupCount && categoryCount >= 3;
    }

    /// <summary>
    /// Main game loop.
    /// </summary>
    private IEnumerator GameLoop()
    {
        while (isPlaying && currentTime > 0 && productsStocked < productsToStock)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }

        EndGame();
    }

    /// <summary>
    /// Update UI elements.
    /// </summary>
    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.Ceil(currentTime)}s";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore} | Stocked: {productsStocked}/{productsToStock}";
        }
    }

    /// <summary>
    /// Update the product queue display.
    /// </summary>
    private void UpdateProductQueueDisplay()
    {
        if (productQueueContainer == null) return;

        foreach (Transform child in productQueueContainer)
        {
            Destroy(child.gameObject);
        }

        int displayCount = Mathf.Min(3, productQueue.Count);
        var queueArray = productQueue.ToArray();
        for (int i = 0; i < displayCount; i++)
        {
            if (productPrefab != null)
            {
                var queueItem = Instantiate(productPrefab, productQueueContainer);
                var image = queueItem.GetComponent<Image>();
                if (image != null)
                {
                    image.color = queueArray[i].color;
                }
            }
        }
    }

    /// <summary>
    /// Show feedback text.
    /// </summary>
    private void ShowFeedback(string message, Color color)
    {
        Debug.Log($"[StockingMiniGame] {message}");
        // Could show floating text or particle effect
    }

    /// <summary>
    /// Fade bonus text coroutine.
    /// </summary>
    private IEnumerator FadeBonusText()
    {
        yield return new WaitForSeconds(1.5f);
        if (categoryBonusText != null)
        {
            categoryBonusText.text = "";
        }
    }

    /// <summary>
    /// End the mini-game and calculate results.
    /// </summary>
    private void EndGame()
    {
        isPlaying = false;

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        // Calculate efficiency bonus
        float efficiency = (float)productsStocked / productsToStock;
        int timeBonus = Mathf.RoundToInt(currentTime);
        int finalScore = currentScore + timeBonus;

        // Calculate performance rating
        string performance;
        if (finalScore >= 250) performance = "Excellent";
        else if (finalScore >= 200) performance = "Great";
        else if (finalScore >= 150) performance = "Good";
        else if (finalScore >= 100) performance = "Fair";
        else performance = "Needs Improvement";

        // Award XP
        int xpEarned = finalScore / 5;
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddExperience(xpEarned);
        }

        // Show results
        if (JobResultsUI.Instance != null)
        {
            JobResultsUI.Instance.ShowResults("Stocking", finalScore, performance);
        }

        // Update business reputation
        var hypermarket = HypermarketController.Instance;
        if (hypermarket != null)
        {
            var state = hypermarket.GetBusinessState();
            if (state != null)
            {
                state.qualityScore = Mathf.Lerp(state.qualityScore, efficiency * 5f, 0.2f);
                state.CalculateReputation();
            }
        }

        Debug.Log($"[StockingMiniGame] Game ended. Final score: {finalScore}, Performance: {performance}");
    }

    /// <summary>
    /// Cancel the mini-game.
    /// </summary>
    public void CancelGame()
    {
        if (!isPlaying) return;

        isPlaying = false;
        StopAllCoroutines();

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        Debug.Log("[StockingMiniGame] Game cancelled");
    }
}

/// <summary>
/// Product data for stocking mini-game.
/// </summary>
[System.Serializable]
public class ProductStockData
{
    public string productId;
    public string productName;
    public string category;
    public Color color;
    public int size = 1; // Number of slots occupied
}

/// <summary>
/// Shelf slot state.
/// </summary>
[System.Serializable]
public class ShelfSlot
{
    public int slotIndex;
    public GameObject slotObject;
    public bool isEmpty = true;
    public string productCategory;
    public string productId;
}
