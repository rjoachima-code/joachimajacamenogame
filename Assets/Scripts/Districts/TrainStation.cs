using UnityEngine;

/// <summary>
/// Data model for a train station in the color-coded train system.
/// </summary>
[System.Serializable]
public class TrainStation
{
    public string stationId;
    public string stationName;
    public DistrictType district;
    public Color lineColor;
    public Vector3 position;
    public string[] connectedStations;
    public bool isActive = true;

    public TrainStation(string id, string name, DistrictType district, Color color, Vector3 pos)
    {
        stationId = id;
        stationName = name;
        this.district = district;
        lineColor = color;
        position = pos;
        connectedStations = new string[0];
    }
}

/// <summary>
/// ScriptableObject containing train station data.
/// </summary>
[CreateAssetMenu(fileName = "NewTrainStation", menuName = "Jacameno/Train Station")]
public class TrainStationData : ScriptableObject
{
    public string stationId;
    public string stationName;
    public DistrictType district;
    public Color lineColor = Color.gray;
    public Vector3 stationPosition;
    public TrainStationData[] connectedStations;
    public bool isActive = true;
    public float travelTimeMinutes = 5f;
}
