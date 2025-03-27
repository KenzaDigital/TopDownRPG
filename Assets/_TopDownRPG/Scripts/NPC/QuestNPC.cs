using UnityEngine;


public class QuestNPC : NPC
{
    // ===== DONNÉES =====

    [Header("Quest Settings")]
    public QuestGiver questGiver; // Référence au composant QuestGiver

    // ===== INTERACTION =====

    // Surcharge de la méthode d'interaction pour ajouter le comportement de quête
    public override void Interact()
    {
        // Vérifier si le DialogueManager existe
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        // Vérifier si un QuestGiver est associé et a une quête
        if (questGiver == null || questGiver.quest == null)
        {
            // Si pas de quête, utiliser le comportement standard du NPC
            base.Interact();
            return;
        }

        // Obtenir le dialogue approprié pour l'état de la quête
        string[] dialogue = questGiver.GetCurrentDialogue();

        // Vérifier que le dialogue existe
        if (dialogue != null && dialogue.Length > 0)
        {
            // Démarrer le dialogue avec le joueur
            string speakerName = string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;
            DialogueManager.Instance.StartDialogue(speakerName, dialogue);

            // Configurer les actions à exécuter quand le dialogue se termine
            ConfigureDialogueEndActions();
        }
    }

    // ===== MÉTHODES PRIVÉES =====

    // Configure les actions qui se déclencheront à la fin du dialogue
    private void ConfigureDialogueEndActions()
    {
        // Obtenir l'état actuel de la quête
        QuestState state = questGiver.GetQuestState();

        // Nettoyer les écouteurs précédents pour éviter les duplications
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        // Configurer l'action appropriée selon l'état de la quête
        if (state == QuestState.NotStarted)
        {
            // Si la quête n'est pas commencée, la démarrer quand le dialogue se termine
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedStartQuest);
        }
        else if (state == QuestState.Complete)
        {
            // Si la quête est complétée, donner les récompenses quand le dialogue se termine
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedGiveRewards);
        }
        // Dans les autres cas, ne rien faire de spécial
    }

    // Méthode appelée quand le dialogue se termine et que la quête doit être démarrée
    private void OnDialogueEndedStartQuest()
    {
        // Démarrer la quête
        QuestManager.Instance.StartQuest(questGiver.quest);

        // Nettoyer l'écouteur pour ne pas qu'il reste actif
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest started: {questGiver.quest.questName}");
    }

    // Méthode appelée quand le dialogue se termine et que les récompenses doivent être données
    private void OnDialogueEndedGiveRewards()
    {
        // Donner les récompenses de la quête
        QuestManager.Instance.GiveQuestRewards(questGiver.quest);

        // Nettoyer l'écouteur pour ne pas qu'il reste actif
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest rewards given for: {questGiver.quest.questName}");
    }
}