using System;
using UnityEngine;
using System.Collections;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance { get; private set; }

    [Header("Time Settings")]
    public int startHour = 8;
    public int startMinute = 0;
    public float realSecondsPerInGameMinute = 1f; // change to speed up/down

    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public event Action<int,int> OnTimeTick; // hour, minute
    public event Action OnNewDay;

    private float accumulator = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        Hour = startHour;
        Minute = startMinute;
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
        if (Minute >= 60) { Minute = 0; Hour++; if (Hour >= 24) { Hour = 0; OnNewDay?.Invoke(); } }
        OnTimeTick?.Invoke(Hour, Minute);
    }

    public string GetTimeString() => $"{Hour:D2}:{Minute:D2}";

    public void MinutesSet(int minute)
    {
        Minute = minute;
    }
}
