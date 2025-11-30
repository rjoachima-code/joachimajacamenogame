using System.Collections;
using UnityEngine;

/// <summary>
/// Initializes the game and manages the startup sequence.
/// Ensures proper manager initialization order.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }

    [Header("Initialization Settings")]
    [SerializeField] private bool autoInitialize = true;
    [SerializeField] private bool spawnAtGrid = true;

    [Header("Manager Prefabs (Optional)")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject saveManagerPrefab;
    [SerializeField] private GameObject timeSystemPrefab;
    [SerializeField] private GameObject districtManagerPrefab;

    [Header("Initialization State")]
    [SerializeField] private bool isInitialized;
    [SerializeField] private float initializationProgress;

    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (autoInitialize)
        {
            StartCoroutine(InitializeGame());
        }
    }

    /// <summary>
    /// Initializes the game systems in the correct order.
    /// </summary>
    public IEnumerator InitializeGame()
    {
        Debug.Log("[GameInitializer] Starting game initialization...");
        initializationProgress = 0f;

        // Step 1: Core Managers (order matters!)
        yield return InitializeCoreManagers();
        initializationProgress = 0.2f;

        // Step 2: Save System
        yield return InitializeSaveSystem();
        initializationProgress = 0.4f;

        // Step 3: Game Systems
        yield return InitializeGameSystems();
        initializationProgress = 0.6f;

        // Step 4: District System
        yield return InitializeDistrictSystem();
        initializationProgress = 0.8f;

        // Step 5: Player Spawn
        if (spawnAtGrid)
        {
            yield return SpawnPlayerAtGrid();
        }
        initializationProgress = 1f;

        isInitialized = true;
        Debug.Log("[GameInitializer] Game initialization complete!");

        // Show welcome notification
        if (NotificationSystem.Instance != null)
        {
            NotificationSystem.Instance.ShowGuideMessage("Welcome to Jacamenoville! I'm Joc, your guide to adult life.");
        }
    }

    /// <summary>
    /// Initializes core manager singletons.
    /// </summary>
    private IEnumerator InitializeCoreManagers()
    {
        Debug.Log("[GameInitializer] Initializing core managers...");

        // GameManager
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }
        yield return null;

        // SaveManager (must be early for other systems to register)
        if (SaveManager.Instance == null && saveManagerPrefab != null)
        {
            Instantiate(saveManagerPrefab);
        }
        yield return null;
    }

    /// <summary>
    /// Initializes the save system and attempts to load existing save.
    /// </summary>
    private IEnumerator InitializeSaveSystem()
    {
        Debug.Log("[GameInitializer] Initializing save system...");

        if (SaveManager.Instance != null)
        {
            // Check for existing save
            // SaveManager.Instance.LoadGame(); // Uncomment when ready
        }
        yield return null;
    }

    /// <summary>
    /// Initializes game systems like time, weather, etc.
    /// </summary>
    private IEnumerator InitializeGameSystems()
    {
        Debug.Log("[GameInitializer] Initializing game systems...");

        // TimeSystem
        if (TimeSystem.Instance == null && timeSystemPrefab != null)
        {
            Instantiate(timeSystemPrefab);
        }
        yield return null;

        // Wait for TimeSystem to be ready
        while (TimeSystem.Instance == null)
        {
            yield return null;
        }

        // Other systems will auto-initialize through their own Awake/Start
        yield return null;
    }

    /// <summary>
    /// Initializes the district system.
    /// </summary>
    private IEnumerator InitializeDistrictSystem()
    {
        Debug.Log("[GameInitializer] Initializing district system...");

        if (DistrictManager.Instance == null && districtManagerPrefab != null)
        {
            Instantiate(districtManagerPrefab);
        }
        yield return null;

        // Wait for DistrictManager to be ready
        while (DistrictManager.Instance == null)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Spawns the player at The Grid station.
    /// </summary>
    private IEnumerator SpawnPlayerAtGrid()
    {
        Debug.Log("[GameInitializer] Spawning player at The Grid...");

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null && TheGrid.Instance != null)
        {
            Vector3 spawnPoint = TheGrid.Instance.GetSpawnPoint();
            player.transform.position = spawnPoint;
            Debug.Log($"[GameInitializer] Player spawned at {spawnPoint}");

            // Trigger arrival event
            TheGrid.Instance.TeleportPlayerToGrid(player);
        }
        else
        {
            Debug.LogWarning("[GameInitializer] Could not spawn player - Player or TheGrid not found");
        }

        yield return null;
    }

    /// <summary>
    /// Gets the current initialization progress (0-1).
    /// </summary>
    public float GetInitializationProgress()
    {
        return initializationProgress;
    }

    /// <summary>
    /// Manually triggers game initialization.
    /// </summary>
    public void Initialize()
    {
        if (!isInitialized)
        {
            StartCoroutine(InitializeGame());
        }
    }

    /// <summary>
    /// Resets the game state (for new game).
    /// </summary>
    public void ResetGame()
    {
        isInitialized = false;
        initializationProgress = 0f;

        // Reset managers as needed
        // This would typically involve resetting PlayerStats, clearing quests, etc.
    }
}
