using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Text moneyText;

    private Dictionary<string, int> shopItems = new Dictionary<string, int>
    {
        { "Food", 10 },
        { "Drink", 5 },
        { "Clothes", 50 }
    };

    public void ShowShop()
    {
        shopPanel.SetActive(true);
        UpdateShopDisplay();
        UpdateMoneyDisplay();
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    private void UpdateShopDisplay()
    {
        // Clear existing items
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // Add shop items
        foreach (KeyValuePair<string, int> item in shopItems)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, itemContainer);
            Text[] texts = itemSlot.GetComponentsInChildren<Text>();
            texts[0].text = item.Key;
            texts[1].text = "$" + item.Value;

            Button buyButton = itemSlot.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item.Key, item.Value));
        }
    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = "$" + MoneyManager.Instance.GetMoney();
    }

    private void BuyItem(string itemName, int price)
    {
        if (MoneyManager.Instance.SpendMoney(price))
        {
            // Add item to inventory (placeholder)
            Debug.Log("Bought " + itemName);
            UpdateMoneyDisplay();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
