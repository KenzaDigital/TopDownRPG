using UnityEngine;
using UnityEngine.InputSystem;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class MovementInputHandler : InputHandler
{
    [SerializeField] private CharacterController characterController;

    private Vector2 moveInput;

    protected override void RegisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.actions["Move"].performed += OnMovePerformed;
            playerInput.actions["Move"].canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogError("PlayerInput is null in MovementInputHandler");
        }
    }

    protected override void UnregisterInputActions()
    {
        PlayerInput playerInput = GetPlayerInput();
        if (playerInput != null)
        {
            playerInput.actions["Move"].performed -= OnMovePerformed;
            playerInput.actions["Move"].canceled -= OnMoveCanceled;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (characterController != null)
        {
            characterController.SetMoveDirection(moveInput);
        }
        else
        {
            Debug.LogError("CharacterController non assigné dans MovementInputHandler");
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        if (characterController != null)
        {
            characterController.SetMoveDirection(moveInput);
        }
    }

    private string GetDebuggerDisplay() => ToString();
}

