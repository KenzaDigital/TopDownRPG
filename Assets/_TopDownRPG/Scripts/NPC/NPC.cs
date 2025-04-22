using UnityEngine;


public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    [Header("Custom Settings (Optional)")]
    [Tooltip("Laissez vide pour utiliser le nom défini dans le DialogueData")]
    public string customName;

    public virtual void Interact()
    {
        if (dialogueData == null)
        {
            Debug.LogError("DialogueData not assigned to NPC: " + gameObject.name);
            return;
        }

        // Vérifier si le DialogueManager existe et démarrer le dialogue
        if (DialogueManager.Instance != null)
        {
            // Utiliser le nom personnalisé s'il est défini, sinon utiliser celui du DialogueData
            string speakerName = string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;
            DialogueManager.Instance.StartDialogue(speakerName, dialogueData.dialogueLines);
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene!");
        }
    }

    public string GetInteractionPrompt()
    {
        if (dialogueData == null)
        {
            return "Appuyez sur E pour interagir";
        }

        string name = string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;
        return "Appuyez sur E pour parler à " + name;
    }

    public string GetCustomName()
    {
        return string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;  
    }

}
