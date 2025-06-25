using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleInventory : MonoBehaviour
{
    public static BattleInventory Instance;

    public List<ItemStack> usableItems = new List<ItemStack>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 🔁 Aguarda o InventorySystem estar pronto antes de carregar os itens
        StartCoroutine(WaitForInventoryAndLoad());
    }

    private IEnumerator WaitForInventoryAndLoad()
    {
        // Espera até que o InventorySystem esteja carregado
        while (InventorySystem.Instance == null)
        {
            yield return null; // espera um frame
        }

        LoadUsableItems();
    }

    public void LoadUsableItems()
    {
        usableItems.Clear();

        List<ItemStack> allItems = InventorySystem.Instance.GetAllItems();

        foreach (var slot in allItems)
        {
            if (slot.item != null && slot.item.effectType != BattleEffectType.None)
            {
                usableItems.Add(new ItemStack(slot.item, slot.amount));
            }
        }

        // (Opcional) Debug para ver o que foi carregado
        Debug.Log($"Usable items carregados: {usableItems.Count}");
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

        // Atualiza a lista interna (se quiser)
        LoadUsableItems();
    }
}
