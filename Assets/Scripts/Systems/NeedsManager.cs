using UnityEngine;
using System;

public class NeedsManager : MonoBehaviour, ISaveable
{
    public static NeedsManager Instance { get; private set; }

    [Header("Needs (0-100)")]
    [Range(0, 100)] public float hunger = 80f;
    [Range(0, 100)] public float energy = 80f;
    [Range(0, 100)] public float hygiene = 80f;
    [Range(0, 100)] public float stress = 10f;

    [Header("Decay Rates (per in-game minute)")]
    public float hungerDecayPerMinute = 0.08f;
    public float energyDecayPerMinute = 0.05f;
    public float hygieneDecayPerMinute = 0.01f;
    public float stressIncreasePerMinute = 0.02f;

    public event Action OnNeedsChanged;

    void Awake()
    {
        Instance = this;
        TimeSystem.Instance.OnTimeTick += OnTimeTick;
        TimeSystem.Instance.OnNewDay += OnNewDay;
        SaveManager.Instance.RegisterSaveable(this);
    }

    void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnTimeTick -= OnTimeTick;
            TimeSystem.Instance.OnNewDay -= OnNewDay;
        }
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnregisterSaveable(this);
    }

    void OnTimeTick(int hour, int minute)
    {
        // every minute decay/increase accordingly
        hunger = Mathf.Clamp(hunger - hungerDecayPerMinute, 0, 100);
        energy = Mathf.Clamp(energy - energyDecayPerMinute, 0, 100);
        hygiene = Mathf.Clamp(hygiene - hygieneDecayPerMinute, 0, 100);
        stress = Mathf.Clamp(stress + stressIncreasePerMinute, 0, 100);
        OnNeedsChanged?.Invoke();
    }

    void OnNewDay()
    {
        // small daily adjustments
        energy = Mathf.Clamp(energy + 10f, 0f, 100f);
        OnNeedsChanged?.Invoke();
    }

    public void ModifyHunger(float delta) { hunger = Mathf.Clamp(hunger + delta, 0, 100); OnNeedsChanged?.Invoke(); }
    public void ModifyEnergy(float delta) { energy = Mathf.Clamp(energy + delta, 0, 100); OnNeedsChanged?.Invoke(); }
    public void ModifyHygiene(float delta) { hygiene = Mathf.Clamp(hygiene + delta, 0, 100); OnNeedsChanged?.Invoke(); }
    public void ModifyStress(float delta) { stress = Mathf.Clamp(stress + delta, 0, 100); OnNeedsChanged?.Invoke(); }

    public string SaveData()
    {
        return JsonUtility.ToJson(Snapshot());
    }

    public void LoadData(string state)
    {
        var data = JsonUtility.FromJson<NeedsSnapshot>(state);
        hunger = data.hunger;
        energy = data.energy;
        hygiene = data.hygiene;
        stress = data.stress;
        OnNeedsChanged?.Invoke();
    }

    public NeedsSnapshot Snapshot() => new NeedsSnapshot { hunger = hunger, energy = energy, hygiene = hygiene, stress = stress };
}

[Serializable]
public struct NeedsSnapshot
{
    public float hunger; public float energy; public float hygiene; public float stress;
}
