using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public NPCController2D npc; // Referência ao script NPC

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) // Verifica se a tecla foi pressionada
        {
            Debug.Log("Tecla E pressionada!");
            npc.Interact();
        }
    }
}
