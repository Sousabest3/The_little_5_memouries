using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Status do jogador
    public int playerLevel;
    public int currentHP;
    public int currentMP;
    public float[] playerPosition = new float[3];

    // Ouro
    public int gold;

    // Inventário: lista de itens salvos
    public List<ItemSaveData> inventory = new List<ItemSaveData>();

    // Missões
    public List<string> activeQuests = new List<string>();
    public List<string> completedQuests = new List<string>();

    // NPC seguidor
    public bool isNPCFollowing;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemId;
    public int amount;
}
