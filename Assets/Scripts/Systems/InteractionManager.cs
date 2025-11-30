using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager for Sims-style object interactions.
/// Handles interaction discovery, radial menu, and execution.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Current State")]
    [SerializeField] private GameObject currentTarget;
    [SerializeField] private List<InteractionOption> availableInteractions = new List<InteractionOption>();

    public event Action<GameObject> OnTargetChanged;
    public event Action<List<InteractionOption>> OnInteractionsAvailable;
    public event Action<InteractionOption> OnInteractionStarted;
    public event Action<InteractionOption> OnInteractionCompleted;

    private Camera mainCamera;
    private bool isInteracting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isInteracting)
        {
            ScanForInteractables();
            HandleInteractionInput();
        }
    }

    /// <summary>
    /// Scans for interactable objects in range and line of sight.
    /// </summary>
    private void ScanForInteractables()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            if (hitObject != currentTarget)
            {
                SetCurrentTarget(hitObject);
            }
        }
        else if (currentTarget != null)
        {
            SetCurrentTarget(null);
        }
    }

    /// <summary>
    /// Sets the current interaction target and discovers available interactions.
    /// </summary>
    private void SetCurrentTarget(GameObject target)
    {
        currentTarget = target;
        availableInteractions.Clear();

        if (target != null)
        {
            // Get all interaction providers on the target
            var providers = target.GetComponents<IInteractionProvider>();
            foreach (var provider in providers)
            {
                availableInteractions.AddRange(provider.GetInteractions());
            }

            // Also check for basic IInteractable
            var interactable = target.GetComponent<IInteractable>();
            if (interactable != null && availableInteractions.Count == 0)
            {
                availableInteractions.Add(new InteractionOption
                {
                    name = "Interact",
                    icon = null,
                    action = () => interactable.Interact()
                });
            }
        }

        OnTargetChanged?.Invoke(currentTarget);
        OnInteractionsAvailable?.Invoke(availableInteractions);
    }

    /// <summary>
    /// Handles player input for interactions.
    /// </summary>
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentTarget != null && availableInteractions.Count > 0)
        {
            if (availableInteractions.Count == 1)
            {
                // Single interaction - execute immediately
                ExecuteInteraction(availableInteractions[0]);
            }
            else
            {
                // Multiple interactions - show radial menu
                ShowInteractionMenu();
            }
        }
    }

    /// <summary>
    /// Shows the radial interaction menu (Sims-style).
    /// </summary>
    private void ShowInteractionMenu()
    {
        // This would trigger the UI to display a radial menu
        // For now, we'll just execute the first interaction
        if (availableInteractions.Count > 0)
        {
            ExecuteInteraction(availableInteractions[0]);
        }
    }

    /// <summary>
    /// Executes a specific interaction.
    /// </summary>
    public void ExecuteInteraction(InteractionOption interaction)
    {
        if (interaction == null || interaction.action == null) return;

        isInteracting = true;
        OnInteractionStarted?.Invoke(interaction);

        try
        {
            interaction.action.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Interaction failed: {e.Message}");
        }

        OnInteractionCompleted?.Invoke(interaction);
        isInteracting = false;
    }

    /// <summary>
    /// Selects an interaction by index from the radial menu.
    /// </summary>
    public void SelectInteraction(int index)
    {
        if (index >= 0 && index < availableInteractions.Count)
        {
            ExecuteInteraction(availableInteractions[index]);
        }
    }

    /// <summary>
    /// Gets the current interaction target.
    /// </summary>
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// Gets available interactions for the current target.
    /// </summary>
    public List<InteractionOption> GetAvailableInteractions()
    {
        return availableInteractions;
    }

    /// <summary>
    /// Checks if player is currently interacting.
    /// </summary>
    public bool IsInteracting()
    {
        return isInteracting;
    }

    /// <summary>
    /// Gets the interaction range.
    /// </summary>
    public float GetInteractionRange()
    {
        return interactionRange;
    }
}

/// <summary>
/// Represents a single interaction option (like in The Sims).
/// </summary>
[Serializable]
public class InteractionOption
{
    public string name;
    public string description;
    public Sprite icon;
    public Action action;
    public float duration;
    public InteractionCategory category;
}

/// <summary>
/// Categories for grouping interactions in the radial menu.
/// </summary>
public enum InteractionCategory
{
    General,
    Social,
    Work,
    Fun,
    Needs,
    Romance,
    Special
}

/// <summary>
/// Interface for objects that provide multiple interaction options.
/// </summary>
public interface IInteractionProvider
{
    List<InteractionOption> GetInteractions();
}
