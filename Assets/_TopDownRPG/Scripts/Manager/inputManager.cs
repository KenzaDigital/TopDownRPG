using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;
    private CharacterController characterController; // Utiliser le bon type

    // Propri�t� pour acc�der au PlayerInput depuis d'autres syst�mes
    // Cherche automatiquement le PlayerInput s'il n'a pas encore �t� d�fini
    public PlayerInput CurrentPlayerInput
    {
        get
        {
            // Si le playerInput n'est pas d�fini, on essaie de le trouver
            if (playerInput == null)
            {
                playerInput = FindFirstObjectByType<PlayerInput>();
                if (playerInput == null)
                {
                    Debug.LogError("Missing PlayerInput in the scene");
                }
                else
                {
                    Debug.Log("PlayerInput found in the scene");
                }
            }
            return playerInput;
        }
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // DontDestroyOnLoad pour qu'il persiste entre les sc�nes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (playerInput != null)
        {
            characterController = playerInput.GetComponent<CharacterController>();
            if (characterController != null)
            {
                playerInput.actions["Dash"].performed += ctx => characterController.Dash();
                Debug.Log("Dash action bound to CharacterController");
            }
            else
            {
                Debug.LogError("CharacterController is missing on the PlayerInput GameObject");
            }
        }
        else
        {
            Debug.LogError("PlayerInput is missing");
        }
    }

    // M�thode pour d�finir le PlayerInput quand un joueur est instanci�
    public void SetPlayerInput(PlayerInput newPlayerInput)
    {
        playerInput = newPlayerInput;
        characterController = playerInput.GetComponent<CharacterController>();
        if (characterController != null)
        {
            playerInput.actions["Dash"].performed += ctx => characterController.Dash();
            Debug.Log("Dash action bound to CharacterController");
        }
        else
        {
            Debug.LogError("CharacterController is missing on the PlayerInput GameObject");
        }
    }
}
