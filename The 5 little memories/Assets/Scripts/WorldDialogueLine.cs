using UnityEngine;

[System.Serializable]
public class WorldDialogueLine
{
    [TextArea(2, 4)] public string text;
    public float typingSpeed = 0.04f;
    public bool autoProgress = false;
    public float autoProgressDelay = 2f;
}
