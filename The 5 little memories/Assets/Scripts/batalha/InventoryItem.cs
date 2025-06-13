using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool isHealing;
    public int amount;

    public void Use(CharacterCombatant target)
    {
        if (isHealing)
            target.Heal(amount);
        else
            target.TakeDamage(amount); // se quiser que itens possam dar dano
    }
}
