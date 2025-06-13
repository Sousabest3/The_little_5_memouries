using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private Item item;
    private int price;
    private ShopSystem shopSystem;

    public void Setup(ShopSystem.ShopItem shopItem, ShopSystem system)
    {
        item = shopItem.item;
        price = shopItem.price;
        shopSystem = system;

        icon.sprite = item.icon;
        icon.color = item.itemColor;
        nameText.text = item.itemName;
        priceText.text = $"{price}G";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => shopSystem.TryBuyItem(item, price));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            shopSystem.ShowItemDetails(item, price);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopSystem.ClearItemDetails();
    }
}
