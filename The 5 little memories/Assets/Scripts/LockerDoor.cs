using UnityEngine;
using System.Collections;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Chave")]
    public string requiredKeyID = "porta_frente";

    [Header("Mensagens")]
    public string lockedMessage = "Está trancada. Precisas de uma chave.";
    public string unlockedMessage = "Usaste a chave.";

    [Header("Cena de destino")]
    public string nextSceneName;
    public Vector2 spawnPosition;

    [Header("Animação")]
    public Animator doorAnimator;
    public string openTriggerName = "Open";
    public float waitBeforeSceneLoad = 1f;

    private bool isUnlocked = false;

    public bool CanInteract() => !isUnlocked;

    public void Interact()
    {
        if (isUnlocked) return;

        foreach (var stack in InventorySystem.Instance.GetAllItems())
        {
            if (stack.item != null && stack.item.isKey && stack.item.keyID == requiredKeyID)
            {
                isUnlocked = true;
                InventorySystem.Instance.RemoveItem(stack.item); // Remove chave opcionalmente
                WorldDialogueSystem.Instance.ShowDialogue(unlockedMessage);
                StartCoroutine(OpenAndTransition());
                return;
            }
        }

        WorldDialogueSystem.Instance.ShowDialogue(lockedMessage);
    }

    private IEnumerator OpenAndTransition()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger(openTriggerName);

        yield return new WaitForSeconds(waitBeforeSceneLoad);

        SceneTransitionManager.Instance.ChangeScene(nextSceneName, spawnPosition);
    }
}
