using UnityEngine;

public class ItemWorld : MonoBehaviour, IInteractable
{
    public Item item;
    public WorldDialogueData pickupDialogue;

    private bool pickedUp = false;

    public bool CanInteract() => !pickedUp;

    public void Interact()
    {
        if (pickedUp) return;

        if (InventorySystem.Instance.AddItem(item))
        {
            pickedUp = true;

            if (pickupDialogue != null && WorldDialogueSystem.Instance != null)
            {
                WorldDialogueSystem.Instance.ShowDialogue(pickupDialogue);
            }

            Destroy(gameObject);
        }
    }
}
