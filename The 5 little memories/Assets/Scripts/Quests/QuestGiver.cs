using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public QuestDialogueData dialogueData;
    private bool interactedOnce = false;

    public bool CanInteract() => !DialogueManager.Instance.IsDialoguePlaying;

    public void Interact()
    {
        var quest = dialogueData.quest;

        if (QuestManager.Instance.IsQuestCompleted(quest.questId))
        {
            DialogueManager.Instance.StartDialogue(dialogueData.doneLines, dialogueData.npcName, dialogueData.npcPortrait);
            return;
        }

        if (IsQuestReadyToComplete())
        {
            DialogueManager.Instance.StartDialogue(dialogueData.completeLines, dialogueData.npcName, dialogueData.npcPortrait, () =>
            {
                QuestManager.Instance.CompleteQuest(quest);
            });
        }
        else if (!interactedOnce)
        {
            interactedOnce = true;
            DialogueManager.Instance.StartDialogue(dialogueData.dialogueLines, dialogueData.npcName, dialogueData.npcPortrait);
            QuestManager.Instance.StartQuest(quest);
        }
        else
        {
            DialogueManager.Instance.StartDialogue(dialogueData.dialogueLines, dialogueData.npcName, dialogueData.npcPortrait);
        }
    }

    private bool IsQuestReadyToComplete()
    {
        var quest = dialogueData.quest;
        return InventorySystem.Instance.HasItem(quest.requiredItem, quest.requiredAmount);
    }
}
