using System;
using UnityEngine;

public class TimeSystem : MonoBehaviour, ISaveable
{
    public static TimeSystem Instance { get; private set; }

    [Header("Time Settings")]
    public int startDay = 1;
    public int startHour = 8;
    public int startMinute = 0;
    public float realSecondsPerInGameMinute = 1f; // change to speed up/down

    public int Day { get; private set; }
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public event Action<int,int> OnTimeTick; // hour, minute
    public event Action OnNewDay;

    private float accumulator = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        Day = startDay;
        Hour = startHour;
        Minute = startMinute;

        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if(SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    void Update()
    {
        accumulator += Time.deltaTime;
        while (accumulator >= realSecondsPerInGameMinute)
        {
            accumulator -= realSecondsPerInGameMinute;
            TickMinute();
        }
    }

    void TickMinute()
    {
        Minute++;
        if (Minute >= 60)
        {
            Minute = 0;
            Hour++;
            if (Hour >= 24)
            {
                Hour = 0;
                Day++;
                OnNewDay?.Invoke();
            }
        }
        OnTimeTick?.Invoke(Hour, Minute);
    }

    public string GetTimeString() => $"{Hour:D2}:{Minute:D2}";
    public string GetDateString() => $"Day {Day}, {GetTimeString()}";

    public void MinutesSet(int minute)
    {
        Minute = minute;
    }

    [System.Serializable]
    private struct TimeData
    {
        public int Day;
        public int Hour;
        public int Minute;
    }

    public string SaveData()
    {
        var data = new TimeData
        {
            Day = Day,
            Hour = Hour,
            Minute = Minute
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<TimeData>(state);
        Day = data.Day;
        Hour = data.Hour;
        Minute = data.Minute;
    }
}
