using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemSlotPrefab;

    private List<GameObject> itemSlots = new List<GameObject>();

    public void ShowInventory(List<string> items)
    {
        inventoryPanel.SetActive(true);
        UpdateInventoryDisplay(items);
    }

    public void HideInventory()
    {
        inventoryPanel.SetActive(false);
    }

    private void UpdateInventoryDisplay(List<string> items)
    {
        // Clear existing slots
        foreach (GameObject slot in itemSlots)
        {
            Destroy(slot);
        }
        itemSlots.Clear();

        // Create new slots
        foreach (string item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
            slot.GetComponentInChildren<Text>().text = item;
            itemSlots.Add(slot);
        }
    }
}
