using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Events")]
    public UnityEvent<string> onDialogueStart;
    public UnityEvent onDialogueEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(string dialogueText)
    {
        onDialogueStart?.Invoke(dialogueText);
    }

    public void EndDialogue()
    {
        onDialogueEnd?.Invoke();
    }
}
