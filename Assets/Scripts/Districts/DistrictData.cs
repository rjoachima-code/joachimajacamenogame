using UnityEngine;

/// <summary>
/// ScriptableObject containing configuration data for a district.
/// </summary>
[CreateAssetMenu(fileName = "NewDistrict", menuName = "Jacameno/District Data")]
public class DistrictData : ScriptableObject
{
    [Header("Basic Info")]
    public string districtName;
    public DistrictType districtType;
    [TextArea(3, 5)]
    public string description;

    [Header("Visual Settings")]
    public Color districtColor = Color.white;
    public Color trainLineColor = Color.gray;

    [Header("Housing")]
    public int baseRentCost = 500;
    public int maxHousingSlots = 10;

    [Header("Economy")]
    public float costOfLivingMultiplier = 1.0f;
    public string[] availableBusinessTypes;

    [Header("NPCs")]
    public string[] npcTypes;
    public int maxNPCCount = 50;

    [Header("Audio")]
    public AudioClip ambientMusic;
    public AudioClip[] ambientSounds;

    [Header("Weather")]
    public float rainChance = 0.3f;
    public float fogChance = 0.1f;

    [Header("Train Station")]
    public string trainStationName;
    public Vector3 trainStationPosition;
}
