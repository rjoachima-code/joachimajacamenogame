using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI companion for tutorials and navigation assistance.
/// </summary>
public class JocGuide : MonoBehaviour
{
    public static JocGuide Instance { get; private set; }

    [Header("Guide Settings")]
    [SerializeField] private string guideName = "Joc";
    [SerializeField] private bool isActive = true;

    [Header("Tutorial State")]
    [SerializeField] private bool hasCompletedIntro = false;
    [SerializeField] private List<string> completedTutorials = new List<string>();

    [Header("Navigation Hints")]
    [SerializeField] private float hintCooldownSeconds = 30f;

    public event Action<string> OnGuideMessage;
    public event Action<string, string[]> OnGuideChoice;
    public event Action<DistrictType> OnNavigationHint;

    private float lastHintTime;
    private Queue<string> messageQueue = new Queue<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (!hasCompletedIntro && isActive)
        {
            ShowIntroduction();
        }
    }

    /// <summary>
    /// Shows the introduction tutorial.
    /// </summary>
    public void ShowIntroduction()
    {
        ShowMessage($"Welcome to Jacameno! I'm {guideName}, your guide to adult life.");
        ShowMessage("I'll help you navigate the city and learn the ropes.");
        ShowMessage("You're starting at The Grid, the central hub connecting all districts.");
        hasCompletedIntro = true;
    }

    /// <summary>
    /// Shows a message from the guide.
    /// </summary>
    public void ShowMessage(string message)
    {
        if (!isActive) return;
        OnGuideMessage?.Invoke(message);
    }

    /// <summary>
    /// Queues a message to be shown later.
    /// </summary>
    public void QueueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    /// <summary>
    /// Shows the next queued message.
    /// </summary>
    public void ShowNextQueuedMessage()
    {
        if (messageQueue.Count > 0)
        {
            ShowMessage(messageQueue.Dequeue());
        }
    }

    /// <summary>
    /// Shows a choice dialog.
    /// </summary>
    public void ShowChoice(string question, string[] options)
    {
        if (!isActive) return;
        OnGuideChoice?.Invoke(question, options);
    }

    /// <summary>
    /// Provides a tutorial for a specific topic.
    /// </summary>
    public void StartTutorial(string tutorialId)
    {
        if (completedTutorials.Contains(tutorialId)) return;

        switch (tutorialId)
        {
            case "train_system":
                ShowTrainSystemTutorial();
                break;
            case "districts":
                ShowDistrictsTutorial();
                break;
            case "businesses":
                ShowBusinessTutorial();
                break;
            case "housing":
                ShowHousingTutorial();
                break;
            case "jobs":
                ShowJobsTutorial();
                break;
            default:
                ShowMessage($"Tutorial '{tutorialId}' is not available yet.");
                return;
        }

        completedTutorials.Add(tutorialId);
    }

    private void ShowTrainSystemTutorial()
    {
        ShowMessage("The train system connects all five districts.");
        ShowMessage("Each district has a color-coded train line.");
        ShowMessage("Travel costs time but allows you to reach any part of the city.");
    }

    private void ShowDistrictsTutorial()
    {
        ShowMessage("Jacameno has five districts:");
        ShowMessage("Fame - Entertainment and celebrity district");
        ShowMessage("Remi - Residential and family-oriented");
        ShowMessage("Kiyo - Technology and innovation");
        ShowMessage("Zenin - Business and financial");
        ShowMessage("Xero - Industrial and manufacturing");
    }

    private void ShowBusinessTutorial()
    {
        ShowMessage("Each district offers different business opportunities.");
        ShowMessage("Start small and grow your business empire!");
    }

    private void ShowHousingTutorial()
    {
        ShowMessage("You'll need a place to live. Rent varies by district.");
        ShowMessage("Cheaper housing is in Xero, premium in Fame and Zenin.");
    }

    private void ShowJobsTutorial()
    {
        ShowMessage("Jobs are essential for earning money.");
        ShowMessage("Different districts offer different job opportunities.");
    }

    /// <summary>
    /// Provides a navigation hint for reaching a district.
    /// </summary>
    public void ProvideNavigationHint(DistrictType targetDistrict)
    {
        if (!isActive) return;
        if (Time.time - lastHintTime < hintCooldownSeconds) return;

        lastHintTime = Time.time;
        OnNavigationHint?.Invoke(targetDistrict);

        var currentDistrict = DistrictManager.Instance?.GetCurrentDistrict() ?? DistrictType.Fame;
        if (currentDistrict == targetDistrict)
        {
            ShowMessage($"You're already in the {targetDistrict} district!");
        }
        else
        {
            ShowMessage($"To reach {targetDistrict}, head to the train station and take the {targetDistrict} line.");
        }
    }

    /// <summary>
    /// Enables or disables the guide.
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
    }

    /// <summary>
    /// Checks if a tutorial has been completed.
    /// </summary>
    public bool HasCompletedTutorial(string tutorialId)
    {
        return completedTutorials.Contains(tutorialId);
    }

    /// <summary>
    /// Resets all tutorials.
    /// </summary>
    public void ResetTutorials()
    {
        completedTutorials.Clear();
        hasCompletedIntro = false;
    }
}
