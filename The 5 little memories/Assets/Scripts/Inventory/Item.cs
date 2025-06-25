using UnityEngine;

public enum BattleEffectType { None, Heal, Damage }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    public Sprite icon;
    [TextArea(3, 5)] public string description;
    [TextArea(2, 4)] public string battleEffect;
    public int maxStack = 5;
    public Color itemColor = Color.white;

    [Header("Batalha")]
    public BattleEffectType effectType;  // ✅ Agora será reconhecido
    public int effectPower;

    [Header("Chave")]
    public bool isKey = false;
    public string keyID;
}
