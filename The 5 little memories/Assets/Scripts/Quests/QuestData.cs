using UnityEngine;

public enum QuestType { CollectItem }

[CreateAssetMenu(menuName = "Quest/Nova Missão")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    [TextArea] public string description;

    public QuestType questType = QuestType.CollectItem;

    public int requiredAmount = 1;
    public Item requiredItem;

    public int rewardMoney;
}
