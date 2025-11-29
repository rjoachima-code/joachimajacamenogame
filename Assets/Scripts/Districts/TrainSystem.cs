using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for the train system, handling district navigation.
/// </summary>
public class TrainSystem : MonoBehaviour
{
    public static TrainSystem Instance { get; private set; }

    [Header("Train Stations")]
    [SerializeField] private TrainStationData[] trainStations;

    [Header("Settings")]
    [SerializeField] private float baseTravelTimeMinutes = 5f;

    public event Action<DistrictType> OnTrainArrival;
    public event Action<DistrictType, DistrictType> OnTrainDeparture;

    private TrainStationData currentStation;
    private bool isInTransit;

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
    /// Gets all available train stations.
    /// </summary>
    public TrainStationData[] GetAllStations()
    {
        return trainStations;
    }

    /// <summary>
    /// Gets the train station for a specific district.
    /// </summary>
    public TrainStationData GetStationByDistrict(DistrictType district)
    {
        foreach (var station in trainStations)
        {
            if (station != null && station.district == district)
            {
                return station;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the current station the player is at.
    /// </summary>
    public TrainStationData GetCurrentStation()
    {
        return currentStation;
    }

    /// <summary>
    /// Sets the current station.
    /// </summary>
    public void SetCurrentStation(TrainStationData station)
    {
        currentStation = station;
    }

    /// <summary>
    /// Initiates travel to a destination district.
    /// </summary>
    public bool TravelToDistrict(DistrictType destination)
    {
        if (isInTransit) return false;

        var targetStation = GetStationByDistrict(destination);
        if (targetStation == null || !targetStation.isActive) return false;

        DistrictType originDistrict = currentStation != null ? currentStation.district : DistrictType.Fame;
        OnTrainDeparture?.Invoke(originDistrict, destination);

        isInTransit = true;
        float travelTime = CalculateTravelTime(currentStation, targetStation);
        StartCoroutine(TravelCoroutine(targetStation, travelTime));

        return true;
    }

    private float CalculateTravelTime(TrainStationData from, TrainStationData to)
    {
        if (from == null) return baseTravelTimeMinutes;
        return to.travelTimeMinutes > 0 ? to.travelTimeMinutes : baseTravelTimeMinutes;
    }

    private System.Collections.IEnumerator TravelCoroutine(TrainStationData destination, float travelTimeMinutes)
    {
        // Advance game time
        if (TimeSystem.Instance != null)
        {
            int minutesToAdd = Mathf.RoundToInt(travelTimeMinutes);
            for (int i = 0; i < minutesToAdd; i++)
            {
                yield return new WaitForSeconds(TimeSystem.Instance.realSecondsPerInGameMinute);
            }
        }
        else
        {
            yield return new WaitForSeconds(travelTimeMinutes);
        }

        currentStation = destination;
        isInTransit = false;
        OnTrainArrival?.Invoke(destination.district);
    }

    /// <summary>
    /// Checks if a route exists between two districts.
    /// </summary>
    public bool HasRouteToDistrict(DistrictType from, DistrictType to)
    {
        var fromStation = GetStationByDistrict(from);
        var toStation = GetStationByDistrict(to);

        if (fromStation == null || toStation == null) return false;
        if (!fromStation.isActive || !toStation.isActive) return false;

        // All districts are connected in this simple implementation
        return true;
    }

    /// <summary>
    /// Gets the color of the train line for a district.
    /// </summary>
    public Color GetLineColor(DistrictType district)
    {
        var station = GetStationByDistrict(district);
        return station != null ? station.lineColor : Color.gray;
    }
}
