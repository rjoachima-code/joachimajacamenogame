using System;
using UnityEngine;

/// <summary>
/// Script for The Grid - the central starting location connecting all districts.
/// </summary>
public class TheGrid : MonoBehaviour
{
    public static TheGrid Instance { get; private set; }

    [Header("Location Settings")]
    [SerializeField] private string locationName = "The Grid";
    [SerializeField] private Vector3 spawnPoint = Vector3.zero;

    [Header("Train Connections")]
    [SerializeField] private Transform[] trainPlatforms;
    [SerializeField] private TrainStationData centralStation;

    [Header("District Exits")]
    [SerializeField] private Transform fameExit;
    [SerializeField] private Transform remiExit;
    [SerializeField] private Transform kiyoExit;
    [SerializeField] private Transform zeninExit;
    [SerializeField] private Transform xeroExit;

    [Header("Key Locations")]
    [SerializeField] private Transform informationKiosk;
    [SerializeField] private Transform starterJobBoard;
    [SerializeField] private Transform housingOffice;

    public event Action OnPlayerArrived;
    public event Action<DistrictType> OnExitUsed;

    private bool isPlayerInGrid;

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
        // Initialize The Grid as starting point
        if (JocGuide.Instance != null)
        {
            JocGuide.Instance.ShowIntroduction();
        }
    }

    /// <summary>
    /// Gets the spawn point for new players.
    /// </summary>
    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    /// <summary>
    /// Gets the location name.
    /// </summary>
    public string GetLocationName()
    {
        return locationName;
    }

    /// <summary>
    /// Teleports player to The Grid.
    /// </summary>
    public void TeleportPlayerToGrid(GameObject player)
    {
        if (player != null)
        {
            player.transform.position = spawnPoint;
            isPlayerInGrid = true;
            OnPlayerArrived?.Invoke();
        }
    }

    /// <summary>
    /// Gets the exit transform for a specific district.
    /// </summary>
    public Transform GetDistrictExit(DistrictType district)
    {
        switch (district)
        {
            case DistrictType.Fame: return fameExit;
            case DistrictType.Remi: return remiExit;
            case DistrictType.Kiyo: return kiyoExit;
            case DistrictType.Zenin: return zeninExit;
            case DistrictType.Xero: return xeroExit;
            default: return null;
        }
    }

    /// <summary>
    /// Called when player uses a district exit.
    /// </summary>
    public void UseDistrictExit(DistrictType district)
    {
        isPlayerInGrid = false;
        OnExitUsed?.Invoke(district);

        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.ChangeDistrict(district);
        }
    }

    /// <summary>
    /// Gets the information kiosk position.
    /// </summary>
    public Transform GetInformationKiosk()
    {
        return informationKiosk;
    }

    /// <summary>
    /// Gets the starter job board position.
    /// </summary>
    public Transform GetStarterJobBoard()
    {
        return starterJobBoard;
    }

    /// <summary>
    /// Gets the housing office position.
    /// </summary>
    public Transform GetHousingOffice()
    {
        return housingOffice;
    }

    /// <summary>
    /// Checks if player is currently in The Grid.
    /// </summary>
    public bool IsPlayerInGrid()
    {
        return isPlayerInGrid;
    }

    /// <summary>
    /// Gets the central train station.
    /// </summary>
    public TrainStationData GetCentralStation()
    {
        return centralStation;
    }

    /// <summary>
    /// Gets available train platforms.
    /// </summary>
    public Transform[] GetTrainPlatforms()
    {
        return trainPlatforms;
    }

    /// <summary>
    /// Shows guide information about The Grid.
    /// </summary>
    public void ShowGridInfo()
    {
        if (JocGuide.Instance != null)
        {
            JocGuide.Instance.ShowMessage("Welcome to The Grid - the central hub of Jacameno!");
            JocGuide.Instance.ShowMessage("From here, you can reach any of the five districts by train.");
            JocGuide.Instance.ShowMessage("Visit the information kiosk to learn more about each district.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInGrid = true;
            OnPlayerArrived?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInGrid = false;
        }
    }
}
