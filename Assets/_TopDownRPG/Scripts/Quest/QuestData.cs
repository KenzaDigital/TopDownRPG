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
    public int requiredAmount = 1;  // Nombre d'ennemis � tuer, d'objets � collecter, etc.
    public Item requiredItem;       // Pour les qu�tes de collecte

    [Header("Quest Rewards")]
    public bool isMainQuest = false;  // Est-ce une qu�te principale d�clenchant la victoire ?
    public Item[] rewardItems;        // Objets donn�s en r�compense
    public int experienceReward;      // R�compense d'exp�rience

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] questOfferDialogue;    // Dialogue lors de l'offre de qu�te
    [TextArea(2, 5)]
    public string[] questActiveDialogue;   // Dialogue quand la qu�te est active
    [TextArea(2, 5)]
    public string[] questCompletedDialogue; // Dialogue quand la qu�te est termin�e
}

// �num�ration pour d�finir diff�rents types de qu�tes
public enum QuestType
{
    KillEnemies,
    CollectItems,
    TalkToNPC
}