using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("UI References")]
    public Transform itemsContent;
    public GameObject itemSlotPrefab;
    public Image largeIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemEffectsText;
    public Button useButton;

    [Header("Configuration")]
    public List<Item> startingItems = new List<Item>();

    private List<Item> inventoryItems = new List<Item>();
    private Item selectedItem;

    private void Start()
    {
        InitializeInventory();
        useButton.onClick.AddListener(UseSelectedItem);
    }

    private void InitializeInventory()
    {
        foreach (Item item in startingItems)
        {
            AddItem(item);
        }
    }

    public void AddItem(Item newItem)
    {
        if (newItem == null) return;
        
        inventoryItems.Add(newItem);
        RefreshInventory();
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (itemToRemove == null) return;
        
        inventoryItems.Remove(itemToRemove);
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        if (itemsContent == null || itemSlotPrefab == null) return;

        // Clear existing slots
        foreach (Transform child in itemsContent)
        {
            if (child != null)
                Destroy(child.gameObject);
        }

        // Create new slots
        foreach (Item item in inventoryItems)
        {
            if (item == null) continue;
            
            GameObject slot = Instantiate(itemSlotPrefab, itemsContent);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            if (slotScript != null)
            {
                slotScript.Initialize(item, this);
            }
        }

        if (selectedItem != null && !inventoryItems.Contains(selectedItem))
        {
            ClearItemDetails();
        }
    }

    public void ShowItemDetails(Item item)
    {
        if (item == null || largeIcon == null || itemNameText == null || 
            itemDescriptionText == null || itemEffectsText == null) return;

        selectedItem = item;
        
        largeIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        
        string effects = "";
        if (item.healthRestore > 0) effects += $"Health +{item.healthRestore}\n";
        if (item.manaRestore > 0) effects += $"Mana +{item.manaRestore}\n";
        if (item.attackBoost > 0) effects += $"Attack +{item.attackBoost}\n";
        if (item.defenseBoost > 0) effects += $"Defense +{item.defenseBoost}\n";
        
        itemEffectsText.text = effects == "" ? "No special effects" : effects;
        
        if (useButton != null)
            useButton.interactable = true;
    }

    private void ClearItemDetails()
    {
        selectedItem = null;
        if (largeIcon != null) largeIcon.sprite = null;
        if (itemNameText != null) itemNameText.text = "Select an item";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
        if (itemEffectsText != null) itemEffectsText.text = "";
        if (useButton != null) useButton.interactable = false;
    }

    private void UseSelectedItem()
    {
        if (selectedItem != null)
        {
            selectedItem.Use();
        }
    }
}