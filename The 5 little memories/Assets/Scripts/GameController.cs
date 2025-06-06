using UnityEngine;

public class GameController : MonoBehaviour
{
    public Item testItem;

    void Start()
    {
        InventorySystem.Instance.AddItem(testItem, 1);
    }
}

