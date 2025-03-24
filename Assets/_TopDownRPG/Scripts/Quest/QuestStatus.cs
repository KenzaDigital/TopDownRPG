using UnityEngine;

[System.Serializable]
public class QuestStatus
{
    public QuestData quest;
    public QuestState state;
    public int currentAmount; // Progression actuelle (ennemis tu�s, objets collect�s)

    public QuestStatus(QuestData quest)
    {
        this.quest = quest;
        state = QuestState.NotStarted;
        currentAmount = 0;
    }

    public bool IsCompleted()
    {
        return currentAmount >= quest.requiredAmount;
    }
}

// �num�ration pour d�finir l'�tat d'une qu�te
public enum QuestState
{
    NotStarted,
    Active,
    Complete,
    Rewarded
}