using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class QuestManager : MonoBehaviour
{
    // ===== SINGLETON =====
    // Instance unique accessible de partout
    public static QuestManager Instance { get; private set; }

    // ===== DONNÉES =====
    // Liste de toutes les quêtes actives du joueur
    private List<QuestStatus> playerQuests = new List<QuestStatus>();

    // ===== ÉVÉNEMENTS =====
    // Ces événements permettent à l'UI et autres systèmes de réagir aux changements
    public UnityEvent<QuestStatus> onQuestStarted;   // Quand une quête démarre
    public UnityEvent<QuestStatus> onQuestUpdated;   // Quand une quête progresse
    public UnityEvent<QuestStatus> onQuestCompleted; // Quand une quête est terminée

    // ===== INITIALISATION =====
    private void Awake()
    {
        // Application du pattern Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Détruire les doublons
        }
        else
        {
            Instance = this; // Définir l'instance unique
        }
    }

    // ===== FONCTIONS DE VÉRIFICATION =====

    // Vérifie si le joueur a déjà cette quête
    public bool HasQuest(QuestData quest)
    {
        return GetQuestStatus(quest) != null;
    }

    // Trouve l'état d'une quête spécifique
    public QuestStatus GetQuestStatus(QuestData quest)
    {
        return playerQuests.Find(q => q.quest == quest);
    }

    // ===== GESTION DES QUÊTES =====

    // Démarre une nouvelle quête
    public void StartQuest(QuestData quest)
    {
        // Éviter les doublons
        if (HasQuest(quest))
            return;

        // Créer et initialiser la nouvelle quête
        QuestStatus newQuest = new QuestStatus(quest);
        newQuest.state = QuestState.Active;
        playerQuests.Add(newQuest);

        // Notification
        onQuestStarted?.Invoke(newQuest);

        Debug.Log("Started quest: " + quest.questName);
    }

    // Met à jour la progression d'une quête
    public void UpdateQuestProgress(QuestData quest, int amount)
        
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        // Vérifier si la quête existe et est active
        if (questStatus == null || questStatus.state != QuestState.Active)
            return;

        // Mettre à jour la progression
        questStatus.currentAmount += amount;
        Debug.Log($"Quest {quest.questName} progress: {questStatus.currentAmount}/{quest.requiredAmount}");

        // Notification
        onQuestUpdated?.Invoke(questStatus);

        // Vérifier si la quête est complétée
        if (questStatus.IsCompleted())
        {
            CompleteQuest(quest);
        }
    }

    // Appelée quand un objet est collecté
    public void ItemCollected(Item item)
    {
        // Mettre à jour toutes les quêtes de collection pour cet objet
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

    // ===== COMPLÉTION ET RÉCOMPENSES =====

    // Marque une quête comme terminée
    private void CompleteQuest(QuestData quest)
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        if (questStatus == null)
            return;

        // Changer l'état
        questStatus.state = QuestState.Complete;

        // Notification
        onQuestCompleted?.Invoke(questStatus);

        Debug.Log("Completed quest: " + quest.questName);
    }

    // Donne les récompenses pour une quête terminée
    public void GiveQuestRewards(QuestData quest)
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        // Vérifier l'état
        if (questStatus == null || questStatus.state != QuestState.Complete)
            return;

        // Récompenses d'objets
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

        // Récompense d'expérience
        if (quest.experienceReward > 0)
        {
            // Si vous implémentez un système d'expérience, ajoutez-le ici
            Debug.Log("Received " + quest.experienceReward + " experience");
        }

        // Marquer comme récompensée
        questStatus.state = QuestState.Rewarded;

        Debug.Log("Received rewards for quest: " + quest.questName);
    }
    public void EnemyKilled()
    {
        // Mettre à jour toutes les quêtes de type KillEnemies
        foreach (QuestStatus quest in playerQuests)
        {
            if (quest.state == QuestState.Active && quest.quest.questType == QuestType.KillEnemies)
            {
                UpdateQuestProgress(quest.quest, 1);
            }
        }
    }
}

