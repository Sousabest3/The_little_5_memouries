using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite defaultNPCExpression;
    public Sprite defaultPlayerExpression;
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)] public string text;
    public Sprite npcExpression;
    public Sprite playerExpression;
    public bool isPlayerSpeaking;
    public float typingSpeed = 0.05f;
    public bool autoProgress = false;
    public float autoProgressDelay = 2f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
}