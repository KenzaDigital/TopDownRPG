using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Rpg/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("NPC Settings")]
    public string npcName;


    [Header("Dialogue")]
    [TextArea(3, 10)]
    public string[] dialogueLines;
}

