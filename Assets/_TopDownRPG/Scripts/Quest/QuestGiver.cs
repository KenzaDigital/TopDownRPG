using UnityEngine;


public class QuestGiver : MonoBehaviour
{
    // ===== DONNÉES =====

    // La quête que ce PNJ peut donner
    public QuestData quest;

    // ===== MÉTHODES PUBLIQUES =====

    // Obtient l'état actuel de la quête
    public QuestState GetQuestState()
    {
        // Vérifications de sécurité
        if (quest == null || QuestManager.Instance == null)
            return QuestState.NotStarted;

        // Demander au QuestManager l'état de cette quête
        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
        return status != null ? status.state : QuestState.NotStarted;
    }

    // Obtient le dialogue approprié selon l'état de la quête
    public string[] GetCurrentDialogue()
    {
        // Si pas de quête, pas de dialogue spécifique
        if (quest == null)
            return null;

        // Obtenir l'état actuel
        QuestState state = GetQuestState();

        // Sélectionner le dialogue approprié selon l'état
        switch (state)
        {
            case QuestState.NotStarted:
                return quest.questOfferDialogue;

            case QuestState.Active:
                // Cas spécial: quête active mais objectifs déjà atteints
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
            // Démarrer la quête
            QuestManager.Instance.StartQuest(quest);
        }
    }

    public void CompleteQuest()
    {
        Debug.Log("Quest completed: " + quest.questName);
        if (QuestManager.Instance != null)
        {
            // Mettre à jour la progression
            QuestManager.Instance.CompleteQuest(quest);
        }
    }

    public void GiveRewards()
    {
        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
        if (status != null && status.state == QuestState.Complete)
        {
            // Logique pour donner les récompenses
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.GiveQuestRewards(quest);
            }
            status.state = QuestState.Rewarded;
            Debug.Log("Rewards given for quest: " + quest.questName);
        }
    }
}
