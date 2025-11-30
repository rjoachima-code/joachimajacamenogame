using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Construction/Home Remodeling Business Controller.
/// Manages projects, crews, and on-site construction work.
/// </summary>
public class ConstructionController : MonoBehaviour
{
    public static ConstructionController Instance { get; private set; }

    [Header("Business Reference")]
    [SerializeField] private string businessId;
    private BusinessState businessState;

    [Header("Projects")]
    [SerializeField] private List<ConstructionProject> activeProjects = new List<ConstructionProject>();
    [SerializeField] private List<ConstructionProject> completedProjects = new List<ConstructionProject>();
    [SerializeField] private List<ProjectBid> availableBids = new List<ProjectBid>();

    [Header("Equipment")]
    [SerializeField] private List<ConstructionTool> ownedTools = new List<ConstructionTool>();
    [SerializeField] private List<ConstructionVehicle> vehicles = new List<ConstructionVehicle>();

    [Header("Tier Configuration")]
    [SerializeField] private ConstructionTierConfig[] tierConfigs;

    // Events
    public event Action<ConstructionProject> OnProjectStarted;
    public event Action<ConstructionProject> OnProjectCompleted;
    public event Action<ProjectBid> OnBidWon;

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
                BusinessType.Construction,
                "My Construction Co."
            );
            businessId = businessState.businessId;
        }

        InitializeStarterTools();
        GenerateAvailableBids();
    }

    private void Update()
    {
        if (businessState == null) return;
        UpdateActiveProjects();
    }

    private void InitializeTierConfigs()
    {
        tierConfigs = new ConstructionTierConfig[]
        {
            new ConstructionTierConfig
            {
                tier = 1, tierName = "Handyman",
                projectTypes = new string[] { "repair" },
                maxConcurrentProjects = 1, maxCrew = 0,
                projectValueCap = 500, requiredBP = 0, requiredCash = 0
            },
            new ConstructionTierConfig
            {
                tier = 2, tierName = "Small Contractor",
                projectTypes = new string[] { "repair", "minor_renovation" },
                maxConcurrentProjects = 2, maxCrew = 2,
                projectValueCap = 5000, requiredBP = 100, requiredCash = 5000
            },
            new ConstructionTierConfig
            {
                tier = 3, tierName = "General Contractor",
                projectTypes = new string[] { "repair", "minor_renovation", "room_remodel" },
                maxConcurrentProjects = 4, maxCrew = 8,
                projectValueCap = 50000, requiredBP = 300, requiredCash = 25000
            },
            new ConstructionTierConfig
            {
                tier = 4, tierName = "Construction Company",
                projectTypes = new string[] { "repair", "minor_renovation", "room_remodel", "whole_home" },
                maxConcurrentProjects = 8, maxCrew = 25,
                projectValueCap = 250000, requiredBP = 600, requiredCash = 75000
            },
            new ConstructionTierConfig
            {
                tier = 5, tierName = "Development Firm",
                projectTypes = new string[] { "all" },
                maxConcurrentProjects = 15, maxCrew = 100,
                projectValueCap = 1000000, requiredBP = 1200, requiredCash = 250000
            }
        };
    }

    private void InitializeStarterTools()
    {
        ownedTools.Clear();
        ownedTools.Add(new ConstructionTool
        {
            toolId = "hammer_basic",
            toolName = "Basic Hammer",
            toolType = ToolType.HandTool,
            condition = 100f
        });
        ownedTools.Add(new ConstructionTool
        {
            toolId = "tape_measure",
            toolName = "Tape Measure",
            toolType = ToolType.Measuring,
            condition = 100f
        });
        ownedTools.Add(new ConstructionTool
        {
            toolId = "screwdriver_set",
            toolName = "Screwdriver Set",
            toolType = ToolType.HandTool,
            condition = 100f
        });
    }

    /// <summary>
    /// Generate available project bids based on tier.
    /// </summary>
    public void GenerateAvailableBids()
    {
        availableBids.Clear();
        var config = GetCurrentTierConfig();

        // Generate 3-5 random bids
        int bidCount = UnityEngine.Random.Range(3, 6);
        for (int i = 0; i < bidCount; i++)
        {
            availableBids.Add(GenerateRandomBid(config));
        }
    }

    private ProjectBid GenerateRandomBid(ConstructionTierConfig config)
    {
        string[] projectNames = { "Leaky Faucet Repair", "Cabinet Installation", "Wall Painting", "Deck Repair", "Bathroom Tile" };
        
        return new ProjectBid
        {
            bidId = Guid.NewGuid().ToString(),
            projectName = projectNames[UnityEngine.Random.Range(0, projectNames.Length)],
            clientName = GenerateClientName(),
            estimatedValue = UnityEngine.Random.Range(100f, config.projectValueCap * 0.3f),
            deadline = DateTime.Now.AddDays(UnityEngine.Random.Range(3, 14)),
            bidExpiration = DateTime.Now.AddDays(2)
        };
    }

    private string GenerateClientName()
    {
        string[] firstNames = { "John", "Sarah", "Mike", "Emily", "David" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones" };
        return $"{firstNames[UnityEngine.Random.Range(0, firstNames.Length)]} {lastNames[UnityEngine.Random.Range(0, lastNames.Length)]}";
    }

    /// <summary>
    /// Submit a bid for a project.
    /// </summary>
    public bool SubmitBid(string bidId, float bidAmount)
    {
        var bid = availableBids.Find(b => b.bidId == bidId);
        if (bid == null) return false;

        var config = GetCurrentTierConfig();
        if (activeProjects.Count >= config.maxConcurrentProjects)
        {
            Debug.LogWarning("[Construction] Max concurrent projects reached");
            return false;
        }

        // Bid success chance based on price
        float successChance = 1f - (bidAmount / bid.estimatedValue);
        successChance = Mathf.Clamp(successChance, 0.2f, 0.9f);

        if (UnityEngine.Random.value < successChance)
        {
            // Won bid
            var project = CreateProjectFromBid(bid, bidAmount);
            activeProjects.Add(project);
            availableBids.Remove(bid);
            
            OnBidWon?.Invoke(bid);
            OnProjectStarted?.Invoke(project);
            return true;
        }

        availableBids.Remove(bid);
        return false;
    }

    private ConstructionProject CreateProjectFromBid(ProjectBid bid, float agreedPrice)
    {
        return new ConstructionProject
        {
            projectId = Guid.NewGuid().ToString(),
            projectName = bid.projectName,
            clientName = bid.clientName,
            agreedPrice = agreedPrice,
            startDate = DateTime.Now,
            deadline = bid.deadline,
            status = ProjectStatus.NotStarted,
            phases = GenerateProjectPhases(bid.projectName)
        };
    }

    private List<ProjectPhase> GenerateProjectPhases(string projectType)
    {
        return new List<ProjectPhase>
        {
            new ProjectPhase { phaseId = "planning", phaseName = "Planning", progress = 0f },
            new ProjectPhase { phaseId = "materials", phaseName = "Material Procurement", progress = 0f },
            new ProjectPhase { phaseId = "execution", phaseName = "Execution", progress = 0f },
            new ProjectPhase { phaseId = "inspection", phaseName = "Final Inspection", progress = 0f }
        };
    }

    /// <summary>
    /// Travel to a project site.
    /// </summary>
    public void TravelToSite(string projectId)
    {
        var project = activeProjects.Find(p => p.projectId == projectId);
        if (project != null)
        {
            project.status = ProjectStatus.InProgress;
            Debug.Log($"[Construction] Arrived at site: {project.projectName}");
        }
    }

    /// <summary>
    /// Perform work on a project phase.
    /// </summary>
    public void PerformWork(string projectId, string phaseId, float workAmount)
    {
        var project = activeProjects.Find(p => p.projectId == projectId);
        if (project == null) return;

        var phase = project.phases.Find(p => p.phaseId == phaseId);
        if (phase != null)
        {
            phase.progress = Mathf.Min(100f, phase.progress + workAmount);
            
            if (phase.progress >= 100f)
            {
                phase.isComplete = true;
                CheckProjectCompletion(project);
            }
        }
    }

    private void CheckProjectCompletion(ConstructionProject project)
    {
        bool allComplete = project.phases.TrueForAll(p => p.isComplete);
        if (allComplete)
        {
            project.status = ProjectStatus.Completed;
            project.completionDate = DateTime.Now;
            
            // Pay out
            BusinessManager.Instance?.RecordSale(businessId, project.agreedPrice);
            
            activeProjects.Remove(project);
            completedProjects.Add(project);
            
            OnProjectCompleted?.Invoke(project);
        }
    }

    /// <summary>
    /// Start measuring mini-game.
    /// </summary>
    public void StartMeasuringMiniGame()
    {
        Debug.Log("[Construction] Starting measuring mini-game");
    }

    /// <summary>
    /// Start cutting mini-game.
    /// </summary>
    public void StartCuttingMiniGame()
    {
        Debug.Log("[Construction] Starting cutting mini-game");
    }

    /// <summary>
    /// Start installation mini-game.
    /// </summary>
    public void StartInstallationMiniGame()
    {
        Debug.Log("[Construction] Starting installation mini-game");
    }

    private void UpdateActiveProjects()
    {
        // Update project timers and check for deadlines
    }

    public ConstructionTierConfig GetCurrentTierConfig()
    {
        int tier = businessState?.tier ?? 1;
        return tierConfigs[Mathf.Clamp(tier - 1, 0, tierConfigs.Length - 1)];
    }

    public BusinessState GetBusinessState() => businessState;
    public List<ProjectBid> GetAvailableBids() => new List<ProjectBid>(availableBids);
    public List<ConstructionProject> GetActiveProjects() => new List<ConstructionProject>(activeProjects);
}

[System.Serializable]
public class ConstructionProject
{
    public string projectId;
    public string projectName;
    public string clientName;
    public string address;
    public float agreedPrice;
    public float materialsCost;
    public DateTime startDate;
    public DateTime deadline;
    public DateTime completionDate;
    public ProjectStatus status;
    public List<ProjectPhase> phases;
    public float clientSatisfaction = 1f;
}

[System.Serializable]
public class ProjectPhase
{
    public string phaseId;
    public string phaseName;
    public float progress;
    public bool isComplete;
    public List<string> requiredTasks;
}

public enum ProjectStatus
{
    NotStarted, InProgress, OnHold, Completed, Cancelled
}

[System.Serializable]
public class ProjectBid
{
    public string bidId;
    public string projectName;
    public string clientName;
    public string description;
    public float estimatedValue;
    public DateTime deadline;
    public DateTime bidExpiration;
}

[System.Serializable]
public class ConstructionTool
{
    public string toolId;
    public string toolName;
    public ToolType toolType;
    public float condition;
    public float efficiency = 1f;
}

public enum ToolType
{
    HandTool, PowerTool, Measuring, Safety, Heavy
}

[System.Serializable]
public class ConstructionVehicle
{
    public string vehicleId;
    public string vehicleName;
    public string vehicleType;
    public float condition;
    public float fuelLevel;
}

[System.Serializable]
public class ConstructionTierConfig
{
    public int tier;
    public string tierName;
    public string[] projectTypes;
    public int maxConcurrentProjects;
    public int maxCrew;
    public float projectValueCap;
    public int requiredBP;
    public float requiredCash;
}
