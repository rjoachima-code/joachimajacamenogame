// Stub for Dialogue.cs
// This is a placeholder for dialogue data structures.
// Implement dialogue lines, choices, and branching conversations.

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string nextDialogueId;
}

public class Dialogue : ScriptableObject
{
    public string id;
    public List<DialogueLine> lines = new List<DialogueLine>();
    public List<DialogueChoice> choices = new List<DialogueChoice>();
}
