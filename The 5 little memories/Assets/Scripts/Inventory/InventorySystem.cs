using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Slots")]
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public int slotsCount = 24;

    [Header("Item Description")]
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI battleEffectsText;

    private List<ItemStack> inventoryItems = new List<ItemStack>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeSlots();
        ClearItemDetails();
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            Instantiate(slotPrefab, slotsContainer).GetComponent<SlotUI>().Initialize(i);
        }
    }

    public bool AddItem(Item item, int amount = 1)
    {
        foreach (var stack in inventoryItems)
        {
            if (stack.item == item && stack.amount < item.maxStack)
            {
                stack.amount += amount;
                UpdateUI();
                return true;
            }
        }

        if (inventoryItems.Count < slotsCount)
        {
            inventoryItems.Add(new ItemStack(item, amount));
            UpdateUI();
            return true;
        }

        Debug.Log("Inventário cheio.");
        return false;
    }

    private void UpdateUI()
    {
        var slots = slotsContainer.GetComponentsInChildren<SlotUI>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventoryItems.Count)
                slots[i].SetItem(inventoryItems[i]);
            else
                slots[i].ClearSlot();
        }

        ClearItemDetails(); // limpa descrição
    }

    public void ShowItemDetails(Item item)
    {
        itemImage.sprite = item.icon;
        itemImage.color = item.itemColor;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        battleEffectsText.text = $"Efeito em batalha: {item.battleEffect}";
    }

    public void ClearItemDetails()
    {
        itemImage.sprite = null;
        itemImage.color = new Color(1, 1, 1, 0);
        itemNameText.text = "";
        itemDescriptionText.text = "";
        battleEffectsText.text = "";
    }

    public bool HasItem(Item item, int amount = 1)
    {
        foreach (var stack in inventoryItems)
        {
            if (stack.item == item && stack.amount >= amount)
                return true;
        }
        return false;
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        foreach (var stack in inventoryItems)
        {
            if (stack.item == item)
            {
                stack.amount -= amount;
                if (stack.amount <= 0)
                    inventoryItems.Remove(stack);
                UpdateUI();
                return;
            }
        }
    }
}
