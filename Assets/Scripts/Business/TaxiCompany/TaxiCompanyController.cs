using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uber/Taxi Company Business Controller.
/// Manages ride requests, drivers, and fleet operations.
/// </summary>
public class TaxiCompanyController : MonoBehaviour
{
    public static TaxiCompanyController Instance { get; private set; }

    [Header("Business Reference")]
    [SerializeField] private string businessId;
    private BusinessState businessState;

    [Header("Fleet")]
    [SerializeField] private List<TaxiVehicle> fleet = new List<TaxiVehicle>();
    [SerializeField] private List<TaxiDriver> drivers = new List<TaxiDriver>();

    [Header("Rides")]
    [SerializeField] private Queue<RideRequest> pendingRides = new Queue<RideRequest>();
    [SerializeField] private List<RideRequest> activeRides = new List<RideRequest>();
    [SerializeField] private List<RideRequest> completedRides = new List<RideRequest>();

    [Header("Service Area")]
    [SerializeField] private List<string> unlockedDistricts = new List<string> { "downtown" };
    [SerializeField] private bool hasAirportAccess = false;

    [Header("Tier Configuration")]
    [SerializeField] private TaxiTierConfig[] tierConfigs;

    // Player driving state
    [Header("Player Driving")]
    [SerializeField] private bool playerIsDriving = false;
    [SerializeField] private RideRequest currentPlayerRide;

    // Events
    public event Action<RideRequest> OnRideRequested;
    public event Action<RideRequest> OnRideAccepted;
    public event Action<RideRequest> OnRideCompleted;
    public event Action<TaxiVehicle> OnVehicleAdded;

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
                BusinessType.TaxiCompany,
                "My Taxi Service"
            );
            businessId = businessState.businessId;
        }

        InitializeStarterVehicle();
        StartRideGeneration();
    }

    private void Update()
    {
        if (businessState == null) return;
        ProcessPendingRides();
        UpdateActiveRides();
    }

    private void InitializeTierConfigs()
    {
        tierConfigs = new TaxiTierConfig[]
        {
            new TaxiTierConfig
            {
                tier = 1, tierName = "Solo Driver",
                fleetSize = 1, maxDrivers = 0,
                dailyRideCap = 20, serviceDistricts = 1,
                features = new string[] { "basic_navigation" },
                requiredBP = 0, requiredCash = 0
            },
            new TaxiTierConfig
            {
                tier = 2, tierName = "Premium Driver",
                fleetSize = 1, maxDrivers = 0,
                dailyRideCap = 30, serviceDistricts = 2,
                features = new string[] { "premium_rides", "better_gps" },
                requiredBP = 75, requiredCash = 3000
            },
            new TaxiTierConfig
            {
                tier = 3, tierName = "Small Fleet",
                fleetSize = 3, maxDrivers = 2,
                dailyRideCap = 80, serviceDistricts = 5,
                features = new string[] { "dispatch_system", "city_wide" },
                requiredBP = 200, requiredCash = 15000
            },
            new TaxiTierConfig
            {
                tier = 4, tierName = "Taxi Company",
                fleetSize = 10, maxDrivers = 9,
                dailyRideCap = 200, serviceDistricts = 10,
                features = new string[] { "corporate_accounts", "airport_contract" },
                requiredBP = 400, requiredCash = 50000
            },
            new TaxiTierConfig
            {
                tier = 5, tierName = "Transportation Empire",
                fleetSize = 30, maxDrivers = 29,
                dailyRideCap = 500, serviceDistricts = 20,
                features = new string[] { "luxury_vehicles", "private_contracts", "regional" },
                requiredBP = 800, requiredCash = 150000
            }
        };
    }

    private void InitializeStarterVehicle()
    {
        fleet.Clear();
        fleet.Add(new TaxiVehicle
        {
            vehicleId = "vehicle_starter",
            vehicleName = "Personal Car",
            vehicleType = VehicleType.Economy,
            condition = 100f,
            fuelLevel = 100f,
            cleanliness = 100f,
            isAvailable = true
        });
    }

    /// <summary>
    /// Start generating ride requests periodically.
    /// </summary>
    private void StartRideGeneration()
    {
        InvokeRepeating(nameof(GenerateRideRequest), 30f, 60f);
    }

    /// <summary>
    /// Generate a random ride request.
    /// </summary>
    private void GenerateRideRequest()
    {
        if (!businessState.isOpen) return;

        var config = GetCurrentTierConfig();
        if (completedRides.Count >= config.dailyRideCap) return;

        var ride = new RideRequest
        {
            rideId = Guid.NewGuid().ToString(),
            passengerName = GeneratePassengerName(),
            pickupLocation = GenerateRandomLocation(),
            dropoffLocation = GenerateRandomLocation(),
            requestTime = DateTime.Now,
            estimatedFare = UnityEngine.Random.Range(8f, 50f),
            passengerRating = UnityEngine.Random.Range(3.5f, 5f),
            status = RideStatus.Pending
        };

        pendingRides.Enqueue(ride);
        OnRideRequested?.Invoke(ride);
    }

    private string GeneratePassengerName()
    {
        string[] names = { "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Riley", "Jamie", "Quinn" };
        return names[UnityEngine.Random.Range(0, names.Length)];
    }

    private Vector3 GenerateRandomLocation()
    {
        // Generate random location within service area
        return new Vector3(
            UnityEngine.Random.Range(-100f, 100f),
            0f,
            UnityEngine.Random.Range(-100f, 100f)
        );
    }

    /// <summary>
    /// Accept a ride as player driver.
    /// </summary>
    public bool AcceptRideAsPlayer(string rideId)
    {
        if (playerIsDriving) return false;

        var ride = FindRideInQueue(rideId);
        if (ride == null) return false;

        RemoveFromQueue(rideId);
        ride.status = RideStatus.Accepted;
        ride.acceptedTime = DateTime.Now;
        ride.assignedVehicleId = fleet[0].vehicleId; // Player's vehicle

        currentPlayerRide = ride;
        activeRides.Add(ride);
        playerIsDriving = true;

        OnRideAccepted?.Invoke(ride);
        return true;
    }

    /// <summary>
    /// Pick up the passenger.
    /// </summary>
    public void PickupPassenger()
    {
        if (currentPlayerRide == null) return;

        currentPlayerRide.status = RideStatus.InProgress;
        currentPlayerRide.pickupTime = DateTime.Now;
        Debug.Log($"[Taxi] Picked up {currentPlayerRide.passengerName}");
    }

    /// <summary>
    /// Complete the ride and drop off passenger.
    /// </summary>
    public void CompleteRide()
    {
        if (currentPlayerRide == null) return;

        currentPlayerRide.status = RideStatus.Completed;
        currentPlayerRide.dropoffTime = DateTime.Now;

        // Calculate fare and tip
        float baseFare = currentPlayerRide.estimatedFare;
        float tip = baseFare * UnityEngine.Random.Range(0.1f, 0.25f);
        float totalEarnings = baseFare + tip;

        BusinessManager.Instance?.RecordSale(businessId, totalEarnings);

        // Update vehicle
        var vehicle = fleet.Find(v => v.vehicleId == currentPlayerRide.assignedVehicleId);
        if (vehicle != null)
        {
            vehicle.fuelLevel -= UnityEngine.Random.Range(5f, 15f);
            vehicle.cleanliness -= UnityEngine.Random.Range(2f, 8f);
        }

        activeRides.Remove(currentPlayerRide);
        completedRides.Add(currentPlayerRide);

        OnRideCompleted?.Invoke(currentPlayerRide);
        
        currentPlayerRide = null;
        playerIsDriving = false;

        Debug.Log($"[Taxi] Ride completed. Earned: ${totalEarnings:F2}");
    }

    /// <summary>
    /// Start navigation mini-game.
    /// </summary>
    public void StartNavigationMiniGame()
    {
        Debug.Log("[Taxi] Starting navigation mini-game");
    }

    /// <summary>
    /// Start driving mini-game.
    /// </summary>
    public void StartDrivingMiniGame()
    {
        Debug.Log("[Taxi] Starting driving mini-game");
    }

    /// <summary>
    /// Perform vehicle maintenance.
    /// </summary>
    public void PerformMaintenance(string vehicleId, MaintenanceType type)
    {
        var vehicle = fleet.Find(v => v.vehicleId == vehicleId);
        if (vehicle == null) return;

        float cost = 0f;
        switch (type)
        {
            case MaintenanceType.Refuel:
                cost = (100f - vehicle.fuelLevel) * 0.5f;
                vehicle.fuelLevel = 100f;
                break;
            case MaintenanceType.Wash:
                cost = 15f;
                vehicle.cleanliness = 100f;
                break;
            case MaintenanceType.Repair:
                cost = (100f - vehicle.condition) * 2f;
                vehicle.condition = 100f;
                break;
        }

        BusinessManager.Instance?.RecordExpense(businessId, cost, $"Vehicle maintenance: {type}");
    }

    /// <summary>
    /// Purchase a new vehicle.
    /// </summary>
    public bool PurchaseVehicle(VehicleType type, float price)
    {
        var config = GetCurrentTierConfig();
        if (fleet.Count >= config.fleetSize) return false;

        if (businessState.cashBalance < price) return false;

        var newVehicle = new TaxiVehicle
        {
            vehicleId = Guid.NewGuid().ToString(),
            vehicleName = $"Vehicle {fleet.Count + 1}",
            vehicleType = type,
            condition = 100f,
            fuelLevel = 100f,
            cleanliness = 100f,
            isAvailable = true
        };

        fleet.Add(newVehicle);
        BusinessManager.Instance?.RecordExpense(businessId, price, "Vehicle purchase");

        OnVehicleAdded?.Invoke(newVehicle);
        return true;
    }

    private RideRequest FindRideInQueue(string rideId)
    {
        foreach (var ride in pendingRides)
        {
            if (ride.rideId == rideId) return ride;
        }
        return null;
    }

    private void RemoveFromQueue(string rideId)
    {
        var tempList = new List<RideRequest>(pendingRides);
        tempList.RemoveAll(r => r.rideId == rideId);
        pendingRides.Clear();
        foreach (var ride in tempList)
        {
            pendingRides.Enqueue(ride);
        }
    }

    private void ProcessPendingRides()
    {
        // Assign rides to available drivers
    }

    private void UpdateActiveRides()
    {
        // Update ride progress for AI drivers
    }

    public TaxiTierConfig GetCurrentTierConfig()
    {
        int tier = businessState?.tier ?? 1;
        return tierConfigs[Mathf.Clamp(tier - 1, 0, tierConfigs.Length - 1)];
    }

    public BusinessState GetBusinessState() => businessState;
    public RideRequest GetCurrentRide() => currentPlayerRide;
    public List<TaxiVehicle> GetFleet() => new List<TaxiVehicle>(fleet);
    public bool IsPlayerDriving() => playerIsDriving;
}

[System.Serializable]
public class TaxiVehicle
{
    public string vehicleId;
    public string vehicleName;
    public VehicleType vehicleType;
    public float condition;
    public float fuelLevel;
    public float cleanliness;
    public bool isAvailable;
    public string assignedDriverId;
}

public enum VehicleType
{
    Economy, Standard, Premium, Luxury, SUV, Van
}

public enum MaintenanceType
{
    Refuel, Wash, Repair, OilChange, TireRotation, FullService
}

[System.Serializable]
public class TaxiDriver
{
    public string driverId;
    public string driverName;
    public float rating;
    public int ridesCompleted;
    public bool isOnDuty;
    public string assignedVehicleId;
}

[System.Serializable]
public class RideRequest
{
    public string rideId;
    public string passengerName;
    public Vector3 pickupLocation;
    public Vector3 dropoffLocation;
    public string pickupAddress;
    public string dropoffAddress;
    public DateTime requestTime;
    public DateTime acceptedTime;
    public DateTime pickupTime;
    public DateTime dropoffTime;
    public float estimatedFare;
    public float actualFare;
    public float tip;
    public float passengerRating;
    public float driverRating;
    public RideStatus status;
    public string assignedDriverId;
    public string assignedVehicleId;
}

public enum RideStatus
{
    Pending, Accepted, EnRouteToPickup, InProgress, Completed, Cancelled
}

[System.Serializable]
public class TaxiTierConfig
{
    public int tier;
    public string tierName;
    public int fleetSize;
    public int maxDrivers;
    public int dailyRideCap;
    public int serviceDistricts;
    public string[] features;
    public int requiredBP;
    public float requiredCash;
}
