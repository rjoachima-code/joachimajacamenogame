using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobResultsUI : MonoBehaviour
{
    public static JobResultsUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text jobTitleText;
    [SerializeField] private TMP_Text earningsText;
    [SerializeField] private TMP_Text performanceText;
    [SerializeField] private Button continueButton;

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

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(HideResults);
        }
    }

    public void ShowResults(string jobTitle, int earnings, string performance)
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        if (jobTitleText != null)
        {
            jobTitleText.text = jobTitle;
        }

        if (earningsText != null)
        {
            earningsText.text = $"Earnings: ${earnings}";
        }

        if (performanceText != null)
        {
            performanceText.text = $"Performance: {performance}";
        }
    }

    private void HideResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
    }
}
