using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;

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

        saveFilePath = Application.persistentDataPath + "/save.json";
    }

    public void SaveGame()
    {
        PlayerSaveData saveData = new PlayerSaveData
        {
            hunger = NeedsManager.Instance.GetHunger(),
            energy = NeedsManager.Instance.GetEnergy(),
            money = MoneyManager.Instance.GetMoney(),
            stress = NeedsManager.Instance.GetStress(),
            experience = 0, // Add experience manager if needed
            gameTime = TimeManager.Instance.GetCurrentTime()
        };

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

            NeedsManager.Instance.EatFood(saveData.hunger - NeedsManager.Instance.GetHunger());
            NeedsManager.Instance.Sleep(saveData.energy - NeedsManager.Instance.GetEnergy());
            MoneyManager.Instance.AddMoney(saveData.money - MoneyManager.Instance.GetMoney());
            NeedsManager.Instance.ReduceStress(NeedsManager.Instance.GetStress() - saveData.stress);

            Debug.Log("Game loaded!");
        }
        else
        {
            Debug.Log("No save file found!");
        }
    }
}
