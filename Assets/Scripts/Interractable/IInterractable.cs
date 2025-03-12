using UnityEngine;


public interface IInteractable
{
    void Interact();
    string GetInteractionPrompt();
    string GetCustomName();
}
