using UnityEngine;

[System.Serializable]
public class QuestStatus
{
    public QuestData quest;
    public QuestState state;
    public int currentAmount; // Progression actuelle (ennemis tués, objets collectés)

    public QuestStatus(QuestData quest)
    {
        this.quest = quest;
        state = QuestState.NotStarted;
        currentAmount = 0;
    }

    public bool IsCompleted()
    {
        bool completed = currentAmount >= quest.requiredAmount;
        Debug.Log($"Checking if quest is completed: {completed} (currentAmount: {currentAmount}, requiredAmount: {quest.requiredAmount})");
        return completed;
    }
}

// Énumération pour définir l'état d'une quête
public enum QuestState
{
    NotStarted,
    Active,
    Complete,
    Rewarded
}
