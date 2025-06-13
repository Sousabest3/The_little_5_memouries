using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public GameObject shopPanel;

    public bool CanInteract() => !shopPanel.activeSelf;

    public void Interact()
    {
        shopPanel.SetActive(true);
        PauseController.SetPause(true);
    }
}
