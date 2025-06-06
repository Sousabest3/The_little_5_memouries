using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Quest Dialogue")]
public class QuestDialogueData : ScriptableObject
{
    public QuestData quest;
    public string npcName;
    public Sprite npcPortrait;

    public DialogueLine[] dialogueLines; // Fala antes da missão
    public DialogueLine[] completeLines; // Fala ao completar
    public DialogueLine[] doneLines;     // Fala se já completou
}
