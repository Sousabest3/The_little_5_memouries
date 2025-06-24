using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    private List<SlotUI> currentSlots = new List<SlotUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        TryReconnectUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryReconnectUI();
        UpdateUI(); // Redesenha os itens após cena nova
    }

    private void TryReconnectUI()
    {
        // Procura pelo novo ItemsPanel (slotsContainer)
        GameObject panel = GameObject.Find("ItemsPanel");
        if (panel != null)
        {
            slotsContainer = panel.transform;

            // Limpa slots antigos e recria
            foreach (Transform child in slotsContainer)
                Destroy(child.gameObject);

            currentSlots.Clear();

            for (int i = 0; i < slotsCount; i++)
            {
                GameObject slotGO = Instantiate(slotPrefab, slotsContainer);
                SlotUI slot = slotGO.GetComponent<SlotUI>();
                slot.Initialize(i);
                currentSlots.Add(slot);
            }
        }

        // Também tenta achar e reconectar os campos de descrição se eles estiverem nulos
        if (itemImage == null) itemImage = GameObject.Find("ItemIcon")?.GetComponent<Image>();
        if (itemNameText == null) itemNameText = GameObject.Find("ItemNameText")?.GetComponent<TextMeshProUGUI>();
        if (itemDescriptionText == null) itemDescriptionText = GameObject.Find("ItemDescriptionText")?.GetComponent<TextMeshProUGUI>();
        if (battleEffectsText == null) battleEffectsText = GameObject.Find("BattleEffectsText")?.GetComponent<TextMeshProUGUI>();
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
        if (slotsContainer == null || currentSlots.Count == 0)
            return;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (i < inventoryItems.Count)
                currentSlots[i].SetItem(inventoryItems[i]);
            else
                currentSlots[i].ClearSlot();
        }

        ClearItemDetails();
    }

    public void ShowItemDetails(Item item)
    {
        if (itemImage == null) return;

        itemImage.sprite = item.icon;
        itemImage.color = item.itemColor;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        battleEffectsText.text = $"Efeito em batalha: {item.battleEffect}";
    }

    public void ClearItemDetails()
    {
        if (itemImage == null) return;

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
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].item == item)
            {
                inventoryItems[i].amount -= amount;
                if (inventoryItems[i].amount <= 0)
                    inventoryItems.RemoveAt(i);
                break;
            }
        }

        UpdateUI();
    }

    public List<ItemStack> GetAllItems()
    {
        return new List<ItemStack>(inventoryItems);
    }
}
