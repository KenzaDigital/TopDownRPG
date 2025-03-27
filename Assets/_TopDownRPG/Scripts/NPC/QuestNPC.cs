using UnityEngine;


public class QuestNPC : NPC
{
    // ===== DONN�ES =====

    [Header("Quest Settings")]
    public QuestGiver questGiver; // R�f�rence au composant QuestGiver

    // ===== INTERACTION =====

    // Surcharge de la m�thode d'interaction pour ajouter le comportement de qu�te
    public override void Interact()
    {
        // V�rifier si le DialogueManager existe
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        // V�rifier si un QuestGiver est associ� et a une qu�te
        if (questGiver == null || questGiver.quest == null)
        {
            // Si pas de qu�te, utiliser le comportement standard du NPC
            base.Interact();
            return;
        }

        // Obtenir le dialogue appropri� pour l'�tat de la qu�te
        string[] dialogue = questGiver.GetCurrentDialogue();

        // V�rifier que le dialogue existe
        if (dialogue != null && dialogue.Length > 0)
        {
            // D�marrer le dialogue avec le joueur
            string speakerName = string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;
            DialogueManager.Instance.StartDialogue(speakerName, dialogue);

            // Configurer les actions � ex�cuter quand le dialogue se termine
            ConfigureDialogueEndActions();
        }
    }

    // ===== M�THODES PRIV�ES =====

    // Configure les actions qui se d�clencheront � la fin du dialogue
    private void ConfigureDialogueEndActions()
    {
        // Obtenir l'�tat actuel de la qu�te
        QuestState state = questGiver.GetQuestState();

        // Nettoyer les �couteurs pr�c�dents pour �viter les duplications
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        // Configurer l'action appropri�e selon l'�tat de la qu�te
        if (state == QuestState.NotStarted)
        {
            // Si la qu�te n'est pas commenc�e, la d�marrer quand le dialogue se termine
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedStartQuest);
        }
        else if (state == QuestState.Complete)
        {
            // Si la qu�te est compl�t�e, donner les r�compenses quand le dialogue se termine
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedGiveRewards);
        }
        // Dans les autres cas, ne rien faire de sp�cial
    }

    // M�thode appel�e quand le dialogue se termine et que la qu�te doit �tre d�marr�e
    private void OnDialogueEndedStartQuest()
    {
        // D�marrer la qu�te
        QuestManager.Instance.StartQuest(questGiver.quest);

        // Nettoyer l'�couteur pour ne pas qu'il reste actif
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest started: {questGiver.quest.questName}");
    }

    // M�thode appel�e quand le dialogue se termine et que les r�compenses doivent �tre donn�es
    private void OnDialogueEndedGiveRewards()
    {
        // Donner les r�compenses de la qu�te
        QuestManager.Instance.GiveQuestRewards(questGiver.quest);

        // Nettoyer l'�couteur pour ne pas qu'il reste actif
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest rewards given for: {questGiver.quest.questName}");
    }
}