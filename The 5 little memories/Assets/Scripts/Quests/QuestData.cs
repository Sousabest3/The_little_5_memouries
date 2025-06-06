using UnityEngine;

public enum QuestType { KillEnemies, CollectItem }

[CreateAssetMenu(menuName = "Quest/New Quest")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    [TextArea] public string description;

    public QuestType questType;
    public int requiredAmount = 1;
    public Item requiredItem;

    public int rewardMoney;
    public string targetEnemyTag; // Para quests de matar inimigos
}
