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
        icon.sprite = stack.item.icon;
        icon.color = stack.item.itemColor;
        icon.preserveAspect = true;
        icon.gameObject.SetActive(true);
        amountText.text = stack.amount > 1 ? stack.amount.ToString() : "";
    }

    public void ClearSlot()
    {
        currentStack = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        amountText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentStack != null)
        {
            InventorySystem.Instance.ShowItemDetails(currentStack.item);
        }
    }
}
