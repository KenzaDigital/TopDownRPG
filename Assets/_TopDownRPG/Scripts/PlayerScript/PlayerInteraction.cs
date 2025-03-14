using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class PlayerInteraction : InputHandler
{
    [Header("UI References")]
    public GameObject interactionPromptPanel;
    public TextMeshProUGUI interactionPromptText;

    private bool isInteracting = false;
    private IInteractable currentInteractable;

    private void Awake()
    {
        // S'assurer que le prompt est désactivé au démarrage
        if (interactionPromptPanel != null)
        {
            interactionPromptPanel.SetActive(false);
        }
    }

    protected override void RegisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.actions["Interact"].started += OnInteract;
        }
        else
        {
            Debug.LogError("PlayerInput is null in PlayerInteraction");
        }
    }

    protected override void UnregisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.actions["Interact"].started -= OnInteract;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && !isInteracting)
        {
            isInteracting = true;
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
        {
            Debug.LogError("Collider2D is null in OnTriggerEnter2D");
            return;
        }

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;

            if (interactionPromptPanel != null && interactionPromptText != null)
            {
                interactionPromptPanel.SetActive(true);
                interactionPromptText.text = currentInteractable.GetInteractionPrompt();
            }
            else
            {
                Debug.LogError("Interaction prompt panel or text is null");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null)
        {
            Debug.LogError("Collider2D is null in OnTriggerExit2D");
            return;
        }

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (currentInteractable == interactable)
        {
            currentInteractable = null;

            if (interactionPromptPanel != null)
            {
                isInteracting = false;
                interactionPromptPanel.SetActive(false);
            }
        }
    }
}
