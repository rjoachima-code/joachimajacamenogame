using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WarehouseMiniGame : MonoBehaviour
{
    public static WarehouseMiniGame Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 45f;
    [SerializeField] private int itemsToSort = 15;
    [SerializeField] private float sortTime = 2f;

    [Header("UI Elements")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text currentItemText;
    [SerializeField] private Button[] binButtons;

    [Header("Bins")]
    [SerializeField] private string[] binNames = { "Electronics", "Clothing", "Food", "Tools" };

    private bool isPlaying = false;
    private float currentTime;
    private int currentScore = 0;
    private int itemsSorted = 0;
    private string currentItem;
    private string correctBin;
    private Dictionary<string, string> itemToBinMapping;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        InitializeItemMapping();
    }

    private void InitializeItemMapping()
    {
        itemToBinMapping = new Dictionary<string, string>
        {
            { "Laptop", "Electronics" },
            { "Phone", "Electronics" },
            { "TV", "Electronics" },
            { "Shirt", "Clothing" },
            { "Pants", "Clothing" },
            { "Hat", "Clothing" },
            { "Apple", "Food" },
            { "Bread", "Food" },
            { "Milk", "Food" },
            { "Hammer", "Tools" },
            { "Screwdriver", "Tools" },
            { "Wrench", "Tools" }
        };
    }

    public void StartMiniGame()
    {
        if (isPlaying) return;

        isPlaying = true;
        currentTime = gameDuration;
        currentScore = 0;
        itemsSorted = 0;

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (isPlaying && currentTime > 0 && itemsSorted < itemsToSort)
        {
            GenerateNewItem();
            yield return new WaitForSeconds(sortTime);

            currentTime -= sortTime;
            UpdateUI();
        }

        EndGame();
    }

    private void GenerateNewItem()
    {
        List<string> items = new List<string>(itemToBinMapping.Keys);
        currentItem = items[Random.Range(0, items.Count)];
        correctBin = itemToBinMapping[currentItem];

        if (currentItemText != null)
        {
            currentItemText.text = $"Sort: {currentItem}";
        }

        // Set button texts to bin names
        for (int i = 0; i < binButtons.Length && i < binNames.Length; i++)
        {
            TMP_Text buttonText = binButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = binNames[i];
                string binName = binNames[i];
                binButtons[i].onClick.RemoveAllListeners();
                binButtons[i].onClick.AddListener(() => OnBinSelected(binName));
            }
        }
    }

    private void OnBinSelected(string selectedBin)
    {
        if (selectedBin == correctBin)
        {
            currentScore += 10;
        }
        else
        {
            currentScore -= 5;
        }

        itemsSorted++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.Ceil(currentTime)}s";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    private void EndGame()
    {
        isPlaying = false;

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        // Calculate earnings based on score
        int earnings = Mathf.Max(0, currentScore / 2);

        // Add money to player
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ModifyMoney(earnings);
        }

        // Show results
        if (JobResultsUI.Instance != null)
        {
            string performance = currentScore >= 120 ? "Excellent" : currentScore >= 90 ? "Good" : "Poor";
            JobResultsUI.Instance.ShowResults("Warehouse", earnings, performance);
        }
    }
}
