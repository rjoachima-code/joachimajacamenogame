using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float timeSpeed = 1f; // How fast time passes (1 = real time)
    [SerializeField] private float dayLengthInMinutes = 24f; // Length of a day in game minutes

    [Header("Events")]
    public UnityEvent onDayStart;
    public UnityEvent onNightStart;
    public UnityEvent onHourPassed;

    private float currentTime = 0f; // Current time in hours (0-24)
    private int currentDay = 1;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            currentTime += Time.deltaTime * timeSpeed / 60f; // Convert to hours

            if (currentTime >= 24f)
            {
                currentTime -= 24f;
                currentDay++;
                onDayStart?.Invoke();
            }

            // Check for day/night transitions
            if (currentTime >= 6f && currentTime < 18f) // Day time
            {
                // Day logic
            }
            else // Night time
            {
                // Night logic
            }
        }
    }

    public void PauseTime()
    {
        isPaused = true;
    }

    public void ResumeTime()
    {
        isPaused = false;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }

    public bool IsDayTime()
    {
        return currentTime >= 6f && currentTime < 18f;
    }
}
