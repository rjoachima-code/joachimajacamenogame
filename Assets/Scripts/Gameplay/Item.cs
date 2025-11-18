using UnityEngine;

[CreateAssetMenu(menuName = "Jacameno/Item")]
public class Item : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;
    public int price;
    public Sprite icon;
    public bool consumable;
    public int hungerRestore;
    public int energyRestore;
}
