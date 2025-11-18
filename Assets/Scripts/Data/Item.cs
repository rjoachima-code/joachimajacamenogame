using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public string description;
    public ItemType type;
    public int value;
    public Dictionary<string, int> effects; // e.g., "hunger": 10, "energy": 5

    public enum ItemType
    {
        Food,
        Tool,
        Clothing,
        Miscellaneous
    }

    public Item(string id, string name, string description, ItemType type, int value)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
        this.effects = new Dictionary<string, int>();
    }

    public void AddEffect(string stat, int amount)
    {
        effects[stat] = amount;
    }

    public int GetEffect(string stat)
    {
        return effects.ContainsKey(stat) ? effects[stat] : 0;
    }
}
