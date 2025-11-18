using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueButton;

    private NPCController npcController;
    private string[] dialogues;
    private int currentDialogueIndex = 0;

    private void Start()
    {
        npcController = GetComponent<NPCController>();
        dialogues = npcController.GetNPCData().dialogues;
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentDialogueIndex = 0;
        ShowCurrentDialogue();
        npcController.StartTalking();
    }

    public void ContinueDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            ShowCurrentDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowCurrentDialogue()
    {
        dialogueText.text = dialogues[currentDialogueIndex];
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        npcController.ChangeState(NPCState.Idle);
    }
}
