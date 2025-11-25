using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    string saveFile => Path.Combine(Application.persistentDataPath, "save.json");

    private List<ISaveable> saveables = new List<ISaveable>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
        {
            saveables.Add(saveable);
        }
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        saveables.Remove(saveable);
    }

    public void SaveGame()
    {
        var dataToSave = new Dictionary<string, string>();
        foreach (var saveable in saveables)
        {
            dataToSave[saveable.GetType().ToString()] = saveable.SaveData();
        }

        string json = JsonUtility.ToJson(new SerializableDictionary<string, string>(dataToSave), true);
        File.WriteAllText(saveFile, json);
        Debug.Log($"Game saved to {saveFile}");
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFile)) { Debug.Log("No save file"); return; }
        string json = File.ReadAllText(saveFile);
        var loadedData = JsonUtility.FromJson<SerializableDictionary<string, string>>(json);
        
        if (loadedData == null) {
            Debug.LogError("Failed to deserialize save data.");
            return;
        }

        var savedData = loadedData.ToDictionary();

        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().ToString();
            if (savedData.ContainsKey(key) && !string.IsNullOrEmpty(savedData[key]))
            {
                saveable.LoadData(savedData[key]);
            }
        }
        Debug.Log("Game loaded");
    }
}
