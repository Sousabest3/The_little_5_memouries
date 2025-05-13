using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    [TextArea(3, 10)]
    public string description = "Item Description";
    
    // Effects
    public int healthRestore = 0;
    public int manaRestore = 0;
    public int attackBoost = 0;
    public int defenseBoost = 0;
    
    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
        // Add actual use logic in child classes
    }
}
