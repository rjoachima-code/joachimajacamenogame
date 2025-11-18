using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    [Header("Phone UI Elements")]
    [SerializeField] private GameObject phonePanel;
    [SerializeField] private GameObject questTab;
    [SerializeField] private GameObject appsTab;
    [SerializeField] private GameObject messagesTab;
    [SerializeField] private Transform questContainer;
    [SerializeField] private GameObject questSlotPrefab;

    public void ShowPhone()
    {
        phonePanel.SetActive(true);
        ShowQuestTab();
    }

    public void HidePhone()
    {
        phonePanel.SetActive(false);
    }

    public void ShowQuestTab()
    {
        questTab.SetActive(true);
        appsTab.SetActive(false);
        messagesTab.SetActive(false);
        UpdateQuestDisplay();
    }

    public void ShowAppsTab()
    {
        questTab.SetActive(false);
        appsTab.SetActive(true);
        messagesTab.SetActive(false);
    }

    public void ShowMessagesTab()
    {
        questTab.SetActive(false);
        appsTab.SetActive(false);
        messagesTab.SetActive(true);
    }

    private void UpdateQuestDisplay()
    {
        // Clear existing quests
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }

        // Add active quests
        foreach (Quest quest in QuestManager.Instance.GetActiveQuests())
        {
            GameObject questSlot = Instantiate(questSlotPrefab, questContainer);
            questSlot.GetComponentInChildren<Text>().text = quest.title + ": " + quest.description;
        }
    }
}
