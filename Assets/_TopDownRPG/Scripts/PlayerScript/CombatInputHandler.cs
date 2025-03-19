using UnityEngine;
using UnityEngine.InputSystem;  

public class CombatInputHandler : MonoBehaviour
{
    private PlayerCombatSystem playerCombatSystem;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerCombatSystem = GetComponent<PlayerCombatSystem>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Attack"].performed += OnAttackPerformed;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Attack"].performed -= OnAttackPerformed;
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (playerCombatSystem != null)
        {
            playerCombatSystem.Attack();
        }
    }
}
