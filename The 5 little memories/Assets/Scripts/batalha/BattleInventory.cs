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
            Destroy(gameObject);
    }

    /// <summary>
    /// Chamar antes da batalha para carregar os itens que têm efeito de batalha.
    /// </summary>
    public void LoadUsableItems()
    {
        usableItems.Clear();

        foreach (var slot in InventorySystem.Instance.GetAllItems())
        {
            if (slot.item.effectType != BattleEffectType.None)
            {
                usableItems.Add(new ItemStack(slot.item, slot.amount));
            }
        }
    }

    public void UseItem(Item item, CharacterCombatant target)
    {
        if (item.effectType == BattleEffectType.Heal)
        {
            target.Heal(item.effectPower);
        }
        else if (item.effectType == BattleEffectType.Damage)
        {
            target.TakeDamage(item.effectPower);
        }

        InventorySystem.Instance.RemoveItem(item, 1);
    }
}
