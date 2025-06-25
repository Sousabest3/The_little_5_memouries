using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemStack currentStack;

    public void Initialize(int index)
    {
        ClearSlot();
    }

    public void SetItem(ItemStack stack)
    {
        currentStack = stack;

        // Mostra o ícone
        if (stack.item != null)
        {
            icon.sprite = stack.item.icon;
            icon.color = stack.item.itemColor;
            icon.preserveAspect = true;
            icon.gameObject.SetActive(true);
        }

        // Mostra a quantidade, mesmo se for 1 (opcional)
        amountText.text = stack.amount > 1 ? stack.amount.ToString() : "";
        amountText.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentStack = null;

        icon.sprite = null;
        icon.gameObject.SetActive(false);

        amountText.text = "";
        amountText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentStack != null && InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ShowItemDetails(currentStack.item);
        }
    }
}
