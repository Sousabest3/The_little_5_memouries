using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class InventorySlot : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image icon;
    private Button button;
    
    private Item item;
    private InventoryController inventory;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(Item newItem, InventoryController newInventory)
    {
        item = newItem;
        inventory = newInventory;
        icon.sprite = item.icon;
        
        button.onClick.AddListener(() => {
            inventory.ShowItemDetails(item);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Optional: Add hover effect
        inventory.ShowItemDetails(item);
    }
}
