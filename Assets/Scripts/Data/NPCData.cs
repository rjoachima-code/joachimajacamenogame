using System;

[Serializable]
public class NPCData
{
    public string id;
    public string name;
    public string[] dialogues;
    public string[] quests;

    public NPCData(string id, string name, string[] dialogues, string[] quests)
    {
        this.id = id;
        this.name = name;
        this.dialogues = dialogues;
        this.quests = quests;
    }
}
