using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> activeQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest.questId) && !completedQuests.Contains(quest.questId))
        {
            activeQuests.Add(quest.questId);
            Debug.Log($"📜 Missão iniciada: {quest.questName}");

            MissionHUDUI.Instance?.ShowMission(quest);
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        if (!completedQuests.Contains(quest.questId))
        {
            completedQuests.Add(quest.questId);
            activeQuests.Remove(quest.questId);

            InventorySystem.Instance.RemoveItem(quest.requiredItem, quest.requiredAmount);
            CurrencySystem.Instance?.AddGold(quest.rewardMoney);

            Debug.Log($"✅ Missão concluída: {quest.questName} (+{quest.rewardMoney} Gold)");
            MissionHUDUI.Instance?.Hide();
        }
    }

    public List<string> GetActiveQuestIds() => new List<string>(activeQuests);
    public List<string> GetCompletedQuestIds() => new List<string>(completedQuests);

    public void ForceAddActiveQuest(string questId)
    {
        if (!activeQuests.Contains(questId))
            activeQuests.Add(questId);
    }

    public void ForceAddCompletedQuest(string questId)
    {
        if (!completedQuests.Contains(questId))
            completedQuests.Add(questId);
    }

    public bool IsQuestActive(string questId) => activeQuests.Contains(questId);
    public bool IsQuestCompleted(string questId) => completedQuests.Contains(questId);
}
