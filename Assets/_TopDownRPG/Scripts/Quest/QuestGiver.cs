using UnityEngine;


public class QuestGiver : MonoBehaviour
{
    public QuestData quest;
    private QuestStatus questStatus;

    private void Start()
    {
        questStatus = new QuestStatus(quest);
        Debug.Log("QuestGiver initialized with quest: " + quest.questName);
    }

    public QuestState GetQuestState()
    {
        Debug.Log("Quest state requested: " + questStatus.state);
        return questStatus.state;
    }

    public string[] GetCurrentDialogue()
    {
        Debug.Log("Getting current dialogue for quest state: " + questStatus.state);
        switch (questStatus.state)
        {
            case QuestState.NotStarted:
                return quest.questOfferDialogue;
            case QuestState.Active:
                return quest.questActiveDialogue;
            case QuestState.Complete:
                return quest.questCompletedDialogue;
            default:
                return null;
        }
    }

    public void StartQuest()
    {
        questStatus.state = QuestState.Active;
        Debug.Log("Quest started: " + quest.questName);
        if (QuestManager.Instance != null)
        {
            // Démarrer la quête
            QuestManager.Instance.StartQuest(quest);
        }
    }

    public void CompleteQuest()
    {
        questStatus.state = QuestState.Complete;
        Debug.Log("Quest completed: " + quest.questName);
        if (QuestManager.Instance != null)
        {
            // Mettre à jour la progression
            QuestManager.Instance.CompleteQuest(quest);
        }
    }

    public void GiveRewards()
    {
        if (questStatus.state == QuestState.Complete)
        {
            // Logique pour donner les récompenses
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.GiveQuestRewards(quest);
            }
            questStatus.state = QuestState.Rewarded;
            Debug.Log("Rewards given for quest: " + quest.questName);
        }
    }
}
