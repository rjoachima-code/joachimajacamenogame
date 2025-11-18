using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeliveryMiniGame : MonoBehaviour
{
    [Header("Mini-Game Settings")]
    [SerializeField] private GameObject miniGamePanel;
    [SerializeField] private Text destinationText;
    [SerializeField] private Button deliverButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private float timeLimit = 60f;

    private string[] destinations = { "House A", "House B", "House C", "Store", "Office" };
    private string currentDestination;
    private int score = 0;
    private float timeRemaining;
    private bool isActive = false;
    private int deliveriesCompleted = 0;

    public void StartMiniGame()
    {
        miniGamePanel.SetActive(true);
        score = 0;
        timeRemaining = timeLimit;
        deliveriesCompleted = 0;
        isActive = true;
        StartCoroutine(MiniGameLoop());
    }

    private IEnumerator MiniGameLoop()
    {
        while (timeRemaining > 0 && isActive && deliveriesCompleted < 5)
        {
            GenerateDestination();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || timeRemaining <= 0);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CompleteDelivery();
            }
            timeRemaining -= Random.Range(5f, 15f);
            UpdateUI();
        }

        EndMiniGame();
    }

    private void GenerateDestination()
    {
        currentDestination = destinations[Random.Range(0, destinations.Length)];
        destinationText.text = "Deliver to: " + currentDestination;
    }

    private void CompleteDelivery()
    {
        score += 20;
        deliveriesCompleted++;
        PlayerSkills.Instance.AddSkillExperience("Work", 10);
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score + " Time: " + Mathf.Ceil(timeRemaining) + " Deliveries: " + deliveriesCompleted;
    }

    private void EndMiniGame()
    {
        isActive = false;
        miniGamePanel.SetActive(false);
        MoneyManager.Instance.AddMoney(score);
        Debug.Log("Delivery mini-game ended. Earned $" + score);
    }
}
