using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class QuestManager : MonoBehaviour
{
    // ===== SINGLETON =====
    // Instance unique accessible de partout
    public static QuestManager Instance { get; private set; }

    // ===== DONN�ES =====
    // Liste de toutes les qu�tes actives du joueur
    private List<QuestStatus> playerQuests = new List<QuestStatus>();

    // ===== �V�NEMENTS =====
    // Ces �v�nements permettent � l'UI et autres syst�mes de r�agir aux changements
    public UnityEvent<QuestStatus> onQuestStarted;   // Quand une qu�te d�marre
    public UnityEvent<QuestStatus> onQuestUpdated;   // Quand une qu�te progresse
    public UnityEvent<QuestStatus> onQuestCompleted; // Quand une qu�te est termin�e

    // ===== INITIALISATION =====
    private void Awake()
    {
        // Application du pattern Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // D�truire les doublons
        }
        else
        {
            Instance = this; // D�finir l'instance unique
        }
    }

    // ===== FONCTIONS DE V�RIFICATION =====

    // V�rifie si le joueur a d�j� cette qu�te
    public bool HasQuest(QuestData quest)
    {
        return GetQuestStatus(quest) != null;
    }

    // Trouve l'�tat d'une qu�te sp�cifique
    public QuestStatus GetQuestStatus(QuestData quest)
    {
        return playerQuests.Find(q => q.quest == quest);
    }

    // ===== GESTION DES QU�TES =====

    // D�marre une nouvelle qu�te
    public void StartQuest(QuestData quest)
    {
        // �viter les doublons
        if (HasQuest(quest))
            return;

        // Cr�er et initialiser la nouvelle qu�te
        QuestStatus newQuest = new QuestStatus(quest);
        newQuest.state = QuestState.Active;
        playerQuests.Add(newQuest);

        // Notification
        onQuestStarted?.Invoke(newQuest);

        Debug.Log("Started quest: " + quest.questName);
    }

    // Met � jour la progression d'une qu�te
    public void UpdateQuestProgress(QuestData quest, int amount)
        
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        // V�rifier si la qu�te existe et est active
        if (questStatus == null || questStatus.state != QuestState.Active)
            return;

        // Mettre � jour la progression
        questStatus.currentAmount += amount;
        Debug.Log($"Quest {quest.questName} progress: {questStatus.currentAmount}/{quest.requiredAmount}");

        // Notification
        onQuestUpdated?.Invoke(questStatus);

        // V�rifier si la qu�te est compl�t�e
        if (questStatus.IsCompleted())
        {
            CompleteQuest(quest);
        }
    }

    // Appel�e quand un objet est collect�
    public void ItemCollected(Item item)
    {
        // Mettre � jour toutes les qu�tes de collection pour cet objet
        Debug.Log("Item collected: " + item.itemName);
        foreach (QuestStatus quest in playerQuests)
        {
            if (quest.state == QuestState.Active &&
                quest.quest.questType == QuestType.CollectItems &&
                quest.quest.requiredItem == item)
            {
                UpdateQuestProgress(quest.quest, 1);
                Debug.Log($"Item collected: {item.itemName}, current amount: {quest.currentAmount}/{quest.quest.requiredAmount}");
            }
        }
    }

    // ===== COMPL�TION ET R�COMPENSES =====

    // Marque une qu�te comme termin�e
    private void CompleteQuest(QuestData quest)
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        if (questStatus == null)
            return;

        // Changer l'�tat
        questStatus.state = QuestState.Complete;

        // Notification
        onQuestCompleted?.Invoke(questStatus);

        Debug.Log("Completed quest: " + quest.questName);
    }

    // Donne les r�compenses pour une qu�te termin�e
    public void GiveQuestRewards(QuestData quest)
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        // V�rifier l'�tat
        if (questStatus == null || questStatus.state != QuestState.Complete)
            return;

        // R�compenses d'objets
        if (quest.rewardItems != null && quest.rewardItems.Length > 0)
        {
            foreach (Item item in quest.rewardItems)
            {
                if (item != null && InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(item);
                }
            }
        }

        // R�compense d'exp�rience
        if (quest.experienceReward > 0)
        {
            // Si vous impl�mentez un syst�me d'exp�rience, ajoutez-le ici
            Debug.Log("Received " + quest.experienceReward + " experience");
        }

        // Marquer comme r�compens�e
        questStatus.state = QuestState.Rewarded;

        Debug.Log("Received rewards for quest: " + quest.questName);
    }
    public void EnemyKilled()
    {
        // Mettre � jour toutes les qu�tes de type KillEnemies
        foreach (QuestStatus quest in playerQuests)
        {
            if (quest.state == QuestState.Active && quest.quest.questType == QuestType.KillEnemies)
            {
                UpdateQuestProgress(quest.quest, 1);
            }
        }
    }
}

