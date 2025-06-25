using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSaver : MonoBehaviour
{
    public static GameSaver Instance;

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
        }
    }

    private string SavePath => Application.persistentDataPath + "/save.sav";

    public void SaveGame()
    {
        SaveData data = new SaveData();

        PlayerCombatant player = FindObjectOfType<PlayerCombatant>();
        if (player != null)
        {
            data.playerLevel = player.level;
            data.currentHP = player.currentHP;
            data.currentMP = player.currentMP;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            data.playerPosition[0] = playerTransform.position.x;
            data.playerPosition[1] = playerTransform.position.y;
            data.playerPosition[2] = playerTransform.position.z;
        }

        data.isNPCFollowing = NPCController2D.FollowingNPC != null;

        data.gold = CurrencySystem.Instance != null ? CurrencySystem.Instance.GetGold() : 0;

        foreach (var stack in InventorySystem.Instance.GetAllItems())
        {
            data.inventory.Add(new ItemSaveData
            {
                itemId = stack.item.itemName, // Use itemName se não tiver itemId
                amount = stack.amount
            });
        }

        var questManager = QuestManager.Instance;
        if (questManager != null)
        {
            data.activeQuests.AddRange(questManager.GetActiveQuestIds());
            data.completedQuests.AddRange(questManager.GetCompletedQuestIds());
        }

        FileStream file = File.Create(SavePath);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();

        Debug.Log("💾 Jogo salvo com sucesso!");
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath)) return;

        var bf = new BinaryFormatter();
        var file = File.Open(SavePath, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();

        PlayerCombatant player = FindObjectOfType<PlayerCombatant>();
        if (player != null)
        {
            player.level = data.playerLevel;
            player.currentHP = data.currentHP;
            player.currentMP = data.currentMP;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            playerTransform.position = new Vector3(
                data.playerPosition[0],
                data.playerPosition[1],
                data.playerPosition[2]
            );
        }

        CurrencySystem.Instance?.SetGold(data.gold);

        InventorySystem inventory = InventorySystem.Instance;
        if (inventory != null)
        {
            foreach (var itemData in data.inventory)
            {
                Item item = ItemDatabase.GetItemById(itemData.itemId); // ou busque por nome
                if (item != null)
                {
                    inventory.AddItem(item, itemData.amount);
                }
            }
        }

        QuestManager questManager = QuestManager.Instance;
        if (questManager != null)
        {
            foreach (string id in data.activeQuests)
                questManager.ForceAddActiveQuest(id);

            foreach (string id in data.completedQuests)
                questManager.ForceAddCompletedQuest(id);
        }

        if (data.isNPCFollowing)
        {
            var npcPrefab = Resources.Load<GameObject>("FollowingNPC");
            if (npcPrefab != null && playerTransform != null)
            {
                GameObject npcInstance = Instantiate(npcPrefab);
                npcInstance.transform.position = playerTransform.position + Vector3.left;
            }
        }

        Debug.Log("✅ Jogo carregado com sucesso!");
    }
}
