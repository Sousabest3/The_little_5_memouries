using UnityEngine;
using System.Collections.Generic;

public class BattleInventory : MonoBehaviour
{
    public static BattleInventory Instance;

    public List<ItemStack> usableItems = new List<ItemStack>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Carrega os itens assim que possível
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadUsableItems();
    }

    /// <summary>
    /// Substitua este método para usar seu próprio sistema de inventário!
    /// </summary>
    public void LoadUsableItems()
    {
        usableItems.Clear();

        // ⚠️ Substitua por seu próprio inventário aqui:
        List<ItemStack> allItems = InventorySystem.Instance.GetAllItems();

        foreach (var slot in allItems)
        {
            if (slot.item != null && slot.item.effectType != BattleEffectType.None)
            {
                usableItems.Add(new ItemStack(slot.item, slot.amount));
            }
        }
    }

    public void UseItem(Item item, CharacterCombatant target)
    {
        if (item == null || target == null) return;

        // Aplica o efeito
        if (item.effectType == BattleEffectType.Heal)
        {
            target.Heal(item.effectPower);
        }
        else if (item.effectType == BattleEffectType.Damage)
        {
            target.TakeDamage(item.effectPower);
        }

        // Remove do seu inventário
        InventorySystem.Instance.RemoveItem(item, 1);

    }
}
