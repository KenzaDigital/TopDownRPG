using UnityEngine;


public class QuestGiver : MonoBehaviour
{
    // ===== DONN�ES =====

    // La qu�te que ce PNJ peut donner
    public QuestData quest;

    // ===== M�THODES PUBLIQUES =====

    // Obtient l'�tat actuel de la qu�te
    public QuestState GetQuestState()
    {
        // V�rifications de s�curit�
        if (quest == null || QuestManager.Instance == null)
            return QuestState.NotStarted;

        // Demander au QuestManager l'�tat de cette qu�te
        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
        return status != null ? status.state : QuestState.NotStarted;
    }

    // Obtient le dialogue appropri� selon l'�tat de la qu�te
    public string[] GetCurrentDialogue()
    {
        // Si pas de qu�te, pas de dialogue sp�cifique
        if (quest == null)
            return null;

        // Obtenir l'�tat actuel
        QuestState state = GetQuestState();

        // S�lectionner le dialogue appropri� selon l'�tat
        switch (state)
        {
            case QuestState.NotStarted:
                return quest.questOfferDialogue;

            case QuestState.Active:
                // Cas sp�cial: qu�te active mais objectifs d�j� atteints
                QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
                if (status != null && status.IsCompleted())
                {
                    return quest.questCompletedDialogue;
                }
                return quest.questActiveDialogue;

            case QuestState.Complete:
                return quest.questCompletedDialogue;

            case QuestState.Rewarded:
                return quest.questCompletedDialogue;

            default:
                return null;
        }
    }

    public void StartQuest()
    {
        Debug.Log("Quest started: " + quest.questName);
        if (QuestManager.Instance != null)
        {
            // D�marrer la qu�te
            QuestManager.Instance.StartQuest(quest);
        }
    }

    public void CompleteQuest()
    {
        Debug.Log("Quest completed: " + quest.questName);
        if (QuestManager.Instance != null)
        {
            // Mettre � jour la progression
            QuestManager.Instance.CompleteQuest(quest);
        }
    }

    public void GiveRewards()
    {
        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
        if (status != null && status.state == QuestState.Complete)
        {
            // Logique pour donner les r�compenses
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.GiveQuestRewards(quest);
            }
            status.state = QuestState.Rewarded;
            Debug.Log("Rewards given for quest: " + quest.questName);
        }
    }
}
