// DialogueData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        [TextArea(3, 5)] public string text;
        public Sprite npcExpression;
        public Sprite playerExpression;
        public bool isPlayerSpeaking;
        [Range(0.01f, 0.1f)] public float typeSpeed = 0.05f;
        public bool autoAdvance = false;
        public float advanceDelay = 2f;
        public AudioClip voiceSound;
        [Range(0.5f, 2f)] public float pitchVariation = 1f;
    }

    public string characterName;
    public Sprite defaultNPCExpression;
    public Sprite defaultPlayerExpression;
    public Line[] lines;
}