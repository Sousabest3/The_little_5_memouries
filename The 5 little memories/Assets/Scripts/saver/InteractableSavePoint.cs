using UnityEngine;

public class InteractableSavePoint : MonoBehaviour, IInteractable
{
    public bool CanInteract() => true;

    public void Interact()
    {
        GameSaver.Instance?.SaveGame();
        Debug.Log("🔥 Salvando jogo na fogueira...");
    }
}
