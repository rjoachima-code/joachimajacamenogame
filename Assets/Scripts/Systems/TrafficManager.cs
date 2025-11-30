using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages traffic AI, vehicle spawning, and traffic flow in the city.
/// </summary>
public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    [Header("Traffic Settings")]
    [SerializeField] private int maxVehicles = 20;
    [SerializeField] private int maxPedestrians = 50;
    [SerializeField] private float spawnRadius = 100f;
    [SerializeField] private float despawnDistance = 150f;

    [Header("Spawn Configuration")]
    [SerializeField] private GameObject[] vehiclePrefabs;
    [SerializeField] private GameObject[] pedestrianPrefabs;
    [SerializeField] private Transform[] vehicleSpawnPoints;
    [SerializeField] private Transform[] pedestrianSpawnPoints;

    [Header("Traffic Nodes")]
    [SerializeField] private TrafficNode[] trafficNodes;

    [Header("Current State")]
    [SerializeField] private List<TrafficVehicle> activeVehicles = new List<TrafficVehicle>();
    [SerializeField] private List<TrafficPedestrian> activePedestrians = new List<TrafficPedestrian>();

    public event Action<TrafficVehicle> OnVehicleSpawned;
    public event Action<TrafficVehicle> OnVehicleDespawned;
    public event Action<TrafficPedestrian> OnPedestrianSpawned;
    public event Action<TrafficPedestrian> OnPedestrianDespawned;

    private Transform playerTransform;
    private float spawnTimer;
    private float spawnInterval = 2f;

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
        // Find player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Subscribe to district changes
        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged += OnDistrictChanged;
        }
    }

    private void OnDestroy()
    {
        if (DistrictManager.Instance != null)
        {
            DistrictManager.Instance.OnDistrictChanged -= OnDistrictChanged;
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnTraffic();
        }

        UpdateTraffic();
        DespawnDistantTraffic();
    }

    /// <summary>
    /// Attempts to spawn new traffic entities based on current density.
    /// </summary>
    private void TrySpawnTraffic()
    {
        // Spawn vehicles
        if (activeVehicles.Count < maxVehicles && vehiclePrefabs.Length > 0 && vehicleSpawnPoints.Length > 0)
        {
            SpawnVehicle();
        }

        // Spawn pedestrians
        if (activePedestrians.Count < maxPedestrians && pedestrianPrefabs.Length > 0 && pedestrianSpawnPoints.Length > 0)
        {
            SpawnPedestrian();
        }
    }

    /// <summary>
    /// Spawns a vehicle at a random spawn point.
    /// </summary>
    private void SpawnVehicle()
    {
        if (playerTransform == null) return;

        // Find valid spawn point
        Transform spawnPoint = GetValidSpawnPoint(vehicleSpawnPoints);
        if (spawnPoint == null) return;

        // Select random vehicle prefab
        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        
        GameObject vehicleObj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        TrafficVehicle vehicle = vehicleObj.GetComponent<TrafficVehicle>();
        
        if (vehicle == null)
        {
            vehicle = vehicleObj.AddComponent<TrafficVehicle>();
        }

        vehicle.Initialize(this);
        activeVehicles.Add(vehicle);
        OnVehicleSpawned?.Invoke(vehicle);
    }

    /// <summary>
    /// Spawns a pedestrian at a random spawn point.
    /// </summary>
    private void SpawnPedestrian()
    {
        if (playerTransform == null) return;

        // Find valid spawn point
        Transform spawnPoint = GetValidSpawnPoint(pedestrianSpawnPoints);
        if (spawnPoint == null) return;

        // Select random pedestrian prefab
        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];
        
        GameObject pedestrianObj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        TrafficPedestrian pedestrian = pedestrianObj.GetComponent<TrafficPedestrian>();
        
        if (pedestrian == null)
        {
            pedestrian = pedestrianObj.AddComponent<TrafficPedestrian>();
        }

        pedestrian.Initialize(this);
        activePedestrians.Add(pedestrian);
        OnPedestrianSpawned?.Invoke(pedestrian);
    }

    /// <summary>
    /// Gets a valid spawn point within range but not too close to player.
    /// </summary>
    private Transform GetValidSpawnPoint(Transform[] spawnPoints)
    {
        if (spawnPoints.Length == 0 || playerTransform == null) return null;

        List<Transform> validPoints = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            float distance = Vector3.Distance(point.position, playerTransform.position);
            if (distance > 30f && distance < spawnRadius)
            {
                validPoints.Add(point);
            }
        }

        if (validPoints.Count > 0)
        {
            return validPoints[Random.Range(0, validPoints.Count)];
        }

        return null;
    }

    /// <summary>
    /// Updates all active traffic entities.
    /// </summary>
    private void UpdateTraffic()
    {
        // Vehicles update themselves via their own Update()
        // This is for any centralized traffic logic like traffic lights

        UpdateTrafficLights();
    }

    /// <summary>
    /// Updates traffic light states.
    /// </summary>
    private void UpdateTrafficLights()
    {
        // Traffic light logic would go here
        // For now, this is a placeholder for future implementation
    }

    /// <summary>
    /// Despawns traffic entities that are too far from the player.
    /// </summary>
    private void DespawnDistantTraffic()
    {
        if (playerTransform == null) return;

        // Despawn distant vehicles
        for (int i = activeVehicles.Count - 1; i >= 0; i--)
        {
            if (activeVehicles[i] == null)
            {
                activeVehicles.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(activeVehicles[i].transform.position, playerTransform.position);
            if (distance > despawnDistance)
            {
                DespawnVehicle(activeVehicles[i]);
            }
        }

        // Despawn distant pedestrians
        for (int i = activePedestrians.Count - 1; i >= 0; i--)
        {
            if (activePedestrians[i] == null)
            {
                activePedestrians.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(activePedestrians[i].transform.position, playerTransform.position);
            if (distance > despawnDistance)
            {
                DespawnPedestrian(activePedestrians[i]);
            }
        }
    }

    /// <summary>
    /// Despawns a specific vehicle.
    /// </summary>
    public void DespawnVehicle(TrafficVehicle vehicle)
    {
        if (vehicle == null) return;

        activeVehicles.Remove(vehicle);
        OnVehicleDespawned?.Invoke(vehicle);
        Destroy(vehicle.gameObject);
    }

    /// <summary>
    /// Despawns a specific pedestrian.
    /// </summary>
    public void DespawnPedestrian(TrafficPedestrian pedestrian)
    {
        if (pedestrian == null) return;

        activePedestrians.Remove(pedestrian);
        OnPedestrianDespawned?.Invoke(pedestrian);
        Destroy(pedestrian.gameObject);
    }

    /// <summary>
    /// Called when district changes - adjusts traffic density.
    /// </summary>
    private void OnDistrictChanged(DistrictType district)
    {
        // Adjust traffic density based on district
        switch (district)
        {
            case DistrictType.Fame:
                maxVehicles = 25;
                maxPedestrians = 60;
                break;
            case DistrictType.Zenin:
                maxVehicles = 30;
                maxPedestrians = 50;
                break;
            case DistrictType.Remi:
                maxVehicles = 15;
                maxPedestrians = 40;
                break;
            case DistrictType.Kiyo:
                maxVehicles = 20;
                maxPedestrians = 45;
                break;
            case DistrictType.Xero:
                maxVehicles = 20;
                maxPedestrians = 30;
                break;
        }
    }

    /// <summary>
    /// Gets the nearest traffic node to a position.
    /// </summary>
    public TrafficNode GetNearestNode(Vector3 position)
    {
        TrafficNode nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var node in trafficNodes)
        {
            float distance = Vector3.Distance(node.transform.position, position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = node;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Gets the count of active vehicles.
    /// </summary>
    public int GetActiveVehicleCount()
    {
        return activeVehicles.Count;
    }

    /// <summary>
    /// Gets the count of active pedestrians.
    /// </summary>
    public int GetActivePedestrianCount()
    {
        return activePedestrians.Count;
    }
}

/// <summary>
/// Represents a traffic node for vehicle pathfinding.
/// </summary>
public class TrafficNode : MonoBehaviour
{
    [SerializeField] private TrafficNode[] connectedNodes;
    [SerializeField] private float speedLimit = 30f;
    [SerializeField] private bool isIntersection;
    [SerializeField] private TrafficLightState trafficLightState = TrafficLightState.Green;

    public TrafficNode[] ConnectedNodes => connectedNodes;
    public float SpeedLimit => speedLimit;
    public bool IsIntersection => isIntersection;
    public TrafficLightState LightState => trafficLightState;

    public TrafficNode GetRandomConnectedNode()
    {
        if (connectedNodes.Length == 0) return null;
        return connectedNodes[Random.Range(0, connectedNodes.Length)];
    }

    public void SetTrafficLight(TrafficLightState state)
    {
        trafficLightState = state;
    }
}

/// <summary>
/// Traffic light states.
/// </summary>
public enum TrafficLightState
{
    Green,
    Yellow,
    Red
}

/// <summary>
/// Component for traffic vehicles.
/// </summary>
public class TrafficVehicle : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float stoppingDistance = 5f;
    [SerializeField] private TrafficNode currentNode;
    [SerializeField] private TrafficNode targetNode;

    private TrafficManager trafficManager;
    private bool isMoving = true;

    public void Initialize(TrafficManager manager)
    {
        trafficManager = manager;
        FindNearestNode();
    }

    private void Update()
    {
        if (!isMoving || targetNode == null) return;

        // Move towards target
        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);

        // Check if reached target
        if (Vector3.Distance(transform.position, targetNode.transform.position) < stoppingDistance)
        {
            currentNode = targetNode;
            targetNode = currentNode.GetRandomConnectedNode();

            // Check traffic light at intersections
            if (currentNode.IsIntersection && currentNode.LightState == TrafficLightState.Red)
            {
                isMoving = false;
                Invoke(nameof(ResumeMovement), 3f);
            }
        }
    }

    private void FindNearestNode()
    {
        if (trafficManager == null) return;
        currentNode = trafficManager.GetNearestNode(transform.position);
        if (currentNode != null)
        {
            targetNode = currentNode.GetRandomConnectedNode();
        }
    }

    private void ResumeMovement()
    {
        isMoving = true;
    }
}

/// <summary>
/// Component for traffic pedestrians.
/// </summary>
public class TrafficPedestrian : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 20f;
    [SerializeField] private float speed = 2f;

    private TrafficManager trafficManager;
    private NavMeshAgent navMeshAgent;
    private Vector3 targetPosition;

    public void Initialize(TrafficManager manager)
    {
        trafficManager = manager;
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (navMeshAgent == null)
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        navMeshAgent.speed = speed;
        SetRandomDestination();
    }

    private void Update()
    {
        if (navMeshAgent == null) return;

        // If reached destination, set new one
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            navMeshAgent.SetDestination(targetPosition);
        }
    }
}
