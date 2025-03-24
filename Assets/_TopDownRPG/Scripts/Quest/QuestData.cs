using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    [TextArea(3, 5)]
    public string questDescription;

    [Header("Quest Requirements")]
    public QuestType questType;
    public int requiredAmount = 1;  // Nombre d'ennemis à tuer, d'objets à collecter, etc.
    public Item requiredItem;       // Pour les quêtes de collecte

    [Header("Quest Rewards")]
    public bool isMainQuest = false;  // Est-ce une quête principale déclenchant la victoire ?
    public Item[] rewardItems;        // Objets donnés en récompense
    public int experienceReward;      // Récompense d'expérience

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] questOfferDialogue;    // Dialogue lors de l'offre de quête
    [TextArea(2, 5)]
    public string[] questActiveDialogue;   // Dialogue quand la quête est active
    [TextArea(2, 5)]
    public string[] questCompletedDialogue; // Dialogue quand la quête est terminée
}

// Énumération pour définir différents types de quêtes
public enum QuestType
{
    KillEnemies,
    CollectItems,
    TalkToNPC
}