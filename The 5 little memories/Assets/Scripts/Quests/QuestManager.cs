using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> activeQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();
    private Dictionary<string, int> enemyKillCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest.questId) && !completedQuests.Contains(quest.questId))
        {
            activeQuests.Add(quest.questId);
            Debug.Log($"Missão iniciada: {quest.questName}");
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        if (!completedQuests.Contains(quest.questId))
        {
            completedQuests.Add(quest.questId);
            activeQuests.Remove(quest.questId);

            if (quest.questType == QuestType.CollectItem)
            {
                InventorySystem.Instance.RemoveItem(quest.requiredItem, quest.requiredAmount);
            }

            CurrencySystem.Instance?.AddGold(quest.rewardMoney);
            Debug.Log($"Missão completa: {quest.questName}");
        }
    }

    public bool IsQuestCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public bool IsQuestActive(string questId)
    {
        return activeQuests.Contains(questId);
    }

    public void RegisterEnemyKill(string enemyTag)
    {
        if (!enemyKillCounts.ContainsKey(enemyTag))
        {
            enemyKillCounts[enemyTag] = 0;
        }

        enemyKillCounts[enemyTag]++;
    }

    public int GetEnemyKillCount(string enemyTag)
    {
        return enemyKillCounts.TryGetValue(enemyTag, out var count) ? count : 0;
    }
}
