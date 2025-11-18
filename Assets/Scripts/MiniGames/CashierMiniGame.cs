using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CashierMiniGame : MonoBehaviour
{
    public static CashierMiniGame Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private int itemsToScan = 10;
    [SerializeField] private float scanTime = 1f;

    [Header("UI Elements")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text currentItemText;
    [SerializeField] private Button[] scanButtons;

    [Header("Items")]
    [SerializeField] private string[] itemNames = { "Apple", "Bread", "Milk", "Eggs", "Cheese" };

    private bool isPlaying = false;
    private float currentTime;
    private int currentScore = 0;
    private int itemsScanned = 0;
    private string currentItem;
    private int correctButtonIndex;

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
    }

    public void StartMiniGame()
    {
        if (isPlaying) return;

        isPlaying = true;
        currentTime = gameDuration;
        currentScore = 0;
        itemsScanned = 0;

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (isPlaying && currentTime > 0 && itemsScanned < itemsToScan)
        {
            GenerateNewItem();
            yield return new WaitForSeconds(scanTime);

            currentTime -= scanTime;
            UpdateUI();
        }

        EndGame();
    }

    private void GenerateNewItem()
    {
        currentItem = itemNames[Random.Range(0, itemNames.Length)];
        correctButtonIndex = Random.Range(0, scanButtons.Length);

        if (currentItemText != null)
        {
            currentItemText.text = $"Scan: {currentItem}";
        }

        // Set button texts (one correct, others random)
        for (int i = 0; i < scanButtons.Length; i++)
        {
            TMP_Text buttonText = scanButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (i == correctButtonIndex)
                {
                    buttonText.text = currentItem;
                    scanButtons[i].onClick.RemoveAllListeners();
                    scanButtons[i].onClick.AddListener(() => OnCorrectScan());
                }
                else
                {
                    string wrongItem = itemNames[Random.Range(0, itemNames.Length)];
                    while (wrongItem == currentItem)
                    {
                        wrongItem = itemNames[Random.Range(0, itemNames.Length)];
                    }
                    buttonText.text = wrongItem;
                    scanButtons[i].onClick.RemoveAllListeners();
                    scanButtons[i].onClick.AddListener(() => OnWrongScan());
                }
            }
        }
    }

    private void OnCorrectScan()
    {
        currentScore += 10;
        itemsScanned++;
        UpdateUI();
    }

    private void OnWrongScan()
    {
        currentScore -= 5;
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
            string performance = currentScore >= 80 ? "Excellent" : currentScore >= 60 ? "Good" : "Poor";
            JobResultsUI.Instance.ShowResults("Cashier", earnings, performance);
        }
    }
}
