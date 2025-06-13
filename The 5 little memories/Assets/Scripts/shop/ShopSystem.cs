using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [System.Serializable]
    public class ShopItem
    {
        public Item item;
        public int price;
    }

    [Header("Configuração da Loja")]
    public List<ShopItem> itemsForSale;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform itemListContainer;
    public GameObject shopItemPrefab;

    public Image itemImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemEffectText;
    public TMP_Text goldText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        RefreshShopUI();
        ClearItemDetails();
    }

    public void RefreshShopUI()
    {
        foreach (Transform child in itemListContainer)
            Destroy(child.gameObject);

        foreach (var shopItem in itemsForSale)
        {
            if (shopItem.item == null) continue; // evita nulos

            GameObject obj = Instantiate(shopItemPrefab, itemListContainer);
            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.Setup(shopItem, this);
        }

        UpdateGoldUI();
    }

    public void ShowItemDetails(Item item, int price)
    {
        itemImage.sprite = item.icon;
        itemImage.color = item.itemColor;
        itemImage.gameObject.SetActive(true);
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        itemEffectText.text = $"Efeito: {item.battleEffect}\nPreço: {price}G";
    }

    public void ClearItemDetails()
    {
        itemImage.gameObject.SetActive(false);
        itemNameText.text = "";
        itemDescriptionText.text = "";
        itemEffectText.text = "";
    }

    public void TryBuyItem(Item item, int price)
    {
        if (CurrencySystem.Instance.SpendGold(price))
        {
            InventorySystem.Instance.AddItem(item);
            RefreshShopUI();
        }
        else
        {
            Debug.Log("💰 Dinheiro insuficiente!");
        }
    }

    public void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {CurrencySystem.Instance.GetGold()}";
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        PauseController.SetPause(false);
    }
}
