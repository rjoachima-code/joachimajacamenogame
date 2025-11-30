using UnityEngine;

/// <summary>
/// Enum representing types of NPCs found in different districts.
/// </summary>
public enum DistrictNPCType
{
    // General
    Pedestrian,
    Shopkeeper,
    SecurityGuard,

    // Fame District
    Celebrity,
    Paparazzi,
    TalentAgent,
    Entertainer,

    // Remi District
    FamilyPerson,
    Teacher,
    ParkRanger,
    NeighborhoodWatch,

    // Kiyo District
    TechEngineer,
    StartupFounder,
    Researcher,
    ITSupport,

    // Zenin District
    BusinessExecutive,
    Banker,
    Lawyer,
    StockBroker,

    // Xero District
    FactoryWorker,
    Mechanic,
    WarehouseManager,
    UnionRep
}

/// <summary>
/// Data for district-specific NPC configuration.
/// </summary>
[System.Serializable]
public class DistrictNPCData
{
    public DistrictNPCType npcType;
    public string displayName;
    public DistrictType[] spawnDistricts;
    public float spawnChance;
    public string[] dialogueLines;
    public bool canGiveQuests;

    public DistrictNPCData(DistrictNPCType type, string name)
    {
        npcType = type;
        displayName = name;
        spawnDistricts = new DistrictType[0];
        spawnChance = 0.5f;
        dialogueLines = new string[0];
        canGiveQuests = false;
    }
}

/// <summary>
/// ScriptableObject for NPC type configuration.
/// </summary>
[CreateAssetMenu(fileName = "NewDistrictNPC", menuName = "Jacameno/District NPC")]
public class DistrictNPCDataAsset : ScriptableObject
{
    [Header("Basic Info")]
    public DistrictNPCType npcType;
    public string displayName;
    [TextArea(2, 4)]
    public string description;

    [Header("Spawning")]
    public DistrictType[] spawnDistricts;
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
    public int maxSpawnCount = 10;

    [Header("Appearance")]
    public GameObject prefab;
    public Sprite portrait;

    [Header("Interaction")]
    [TextArea(1, 3)]
    public string[] dialogueLines;
    public bool canGiveQuests = false;
    public string[] availableQuests;

    [Header("Schedule")]
    public int activeStartHour = 8;
    public int activeEndHour = 20;
}

/// <summary>
/// Manager for district NPC types and spawning.
/// </summary>
public class DistrictNPCManager : MonoBehaviour
{
    public static DistrictNPCManager Instance { get; private set; }

    [SerializeField] private DistrictNPCDataAsset[] npcTypes;

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
    /// Gets NPC types that spawn in a specific district.
    /// </summary>
    public DistrictNPCDataAsset[] GetNPCTypesForDistrict(DistrictType district)
    {
        var result = new System.Collections.Generic.List<DistrictNPCDataAsset>();
        foreach (var npc in npcTypes)
        {
            if (npc == null) continue;
            if (npc.spawnDistricts == null || npc.spawnDistricts.Length == 0)
            {
                result.Add(npc);
                continue;
            }
            foreach (var spawnDistrict in npc.spawnDistricts)
            {
                if (spawnDistrict == district)
                {
                    result.Add(npc);
                    break;
                }
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Gets a random NPC type for a district.
    /// </summary>
    public DistrictNPCDataAsset GetRandomNPCForDistrict(DistrictType district)
    {
        var npcs = GetNPCTypesForDistrict(district);
        if (npcs.Length == 0) return null;

        // Weight by spawn chance
        float totalChance = 0f;
        foreach (var npc in npcs)
        {
            totalChance += npc.spawnChance;
        }

        float roll = Random.value * totalChance;
        float cumulative = 0f;
        foreach (var npc in npcs)
        {
            cumulative += npc.spawnChance;
            if (roll <= cumulative) return npc;
        }

        return npcs[npcs.Length - 1];
    }

    /// <summary>
    /// Checks if an NPC type is currently active based on time.
    /// </summary>
    public bool IsNPCActive(DistrictNPCDataAsset npc)
    {
        if (npc == null) return false;
        if (TimeSystem.Instance == null) return true;

        int currentHour = TimeSystem.Instance.Hour;
        return currentHour >= npc.activeStartHour && currentHour < npc.activeEndHour;
    }

    /// <summary>
    /// Gets all NPC types.
    /// </summary>
    public DistrictNPCDataAsset[] GetAllNPCTypes()
    {
        return npcTypes;
    }
}
