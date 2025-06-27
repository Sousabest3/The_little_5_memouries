using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public enum SpeakerType { Player, NPC, Ally }

    [System.Serializable]
    public class Line
    {
        [TextArea(3, 5)] public string text;
        public SpeakerType speaker;
        public Sprite portrait;
        [Range(0.01f, 0.1f)] public float typeSpeed = 0.05f;
        public bool autoAdvance = false;
        public float advanceDelay = 2f;
        public AudioClip voiceSound;
    }

    public string npcName = "NPC";
    public string allyName = "Ally";
    public string playerName = "Player";
    public Sprite defaultNPCPortrait;
    public Sprite defaultAllyPortrait;
    public Sprite defaultPlayerPortrait;
    public Line[] lines;
}
