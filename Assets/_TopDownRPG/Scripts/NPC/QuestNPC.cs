using UnityEngine;


public class QuestNPC : NPC
{
    [Header("Quest Settings")]
    public QuestGiver questGiver;
    public DialogueData questDialogueData; // Nouveau DialogueData pour les quêtes

    public override void Interact()
    {
        Debug.Log("Interact called on QuestNPC");

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        if (questGiver != null && questGiver.quest != null)
        {
            string[] dialogue = questGiver.GetCurrentDialogue();

            if (dialogue != null && dialogue.Length > 0)
            {
                string speakerName = string.IsNullOrEmpty(customName) ? questDialogueData.npcName : customName;
                DialogueManager.Instance.StartDialogue(speakerName, dialogue);
                ConfigureDialogueEndActions();
                return;
            }
        }

        // Si pas de quête ou pas de dialogue de quête, utiliser le comportement standard du NPC
        base.Interact();
    }

    private void ConfigureDialogueEndActions()
    {
        Debug.Log("Configuring dialogue end actions");

        QuestState state = questGiver.GetQuestState();
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        if (state == QuestState.NotStarted)
        {
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedStartQuest);
        }
        else if (state == QuestState.Complete)
        {
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedGiveRewards);
        }
    }

    private void OnDialogueEndedStartQuest()
    {
        Debug.Log("Starting quest");
        questGiver.StartQuest();
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();
        Debug.Log($"Quest started: {questGiver.quest.questName}");
    }

    private void OnDialogueEndedGiveRewards()
    {
        Debug.Log("Giving quest rewards");
        questGiver.GiveRewards();
        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();
        Debug.Log($"Quest rewards given for: {questGiver.quest.questName}");
    }
}
