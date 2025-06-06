using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldDialogue", menuName = "Dialogue/World Dialogue")]
public class WorldDialogueData : ScriptableObject
{
    public WorldDialogueLine[] lines;
}

