using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MissionHUDUI : MonoBehaviour
{
    public static MissionHUDUI Instance;

    [Header("Referências UI")]
    public TMP_Text missionNameText;
    public TMP_Text objectiveText;
    public TMP_Text rewardText;

    private QuestData currentQuest;

    // Cenas onde o HUD deve ficar invisível (mas não ser destruído)
    private readonly string[] hiddenScenes = {
        "Battle",
        "VictoryScene",
        "GameOverScene",
        "Menu",
        "IntroScene"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DontDestroyOnLoad(transform.root.gameObject);
                Debug.LogWarning("⚠️ MissionHUDUI não está na raiz. Usando root para DontDestroyOnLoad.");
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // evita duplicatas
            return;
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Oculta se estiver em uma cena da lista
        foreach (var sceneName in hiddenScenes)
        {
            if (scene.name == sceneName)
            {
                Hide();
                return;
            }
        }

        // Se voltamos para uma cena jogável e ainda há missão ativa, mostrar novamente
        if (currentQuest != null && !QuestManager.Instance.IsQuestCompleted(currentQuest.questId))
        {
            ShowMission(currentQuest);
        }
    }

    public void ShowMission(QuestData quest)
    {
        if (quest == null) return;
        if (missionNameText == null || objectiveText == null || rewardText == null)
        {
            Debug.LogError("❌ MissionHUDUI: Referências da UI não atribuídas.");
            return;
        }

        currentQuest = quest;
        gameObject.SetActive(true);

        missionNameText.text = $"📜 {quest.questName}";
        objectiveText.text = $"🎯 Item: {quest.requiredItem.itemName} ({InventorySystem.Instance.GetItemCount(quest.requiredItem)}/{quest.requiredAmount})";
        rewardText.text = $"💰 Recompensa: {quest.rewardMoney} ouro";
    }

    public void UpdateProgress()
    {
        if (currentQuest == null || !gameObject.activeSelf) return;

        objectiveText.text = $"🎯 Item: {currentQuest.requiredItem.itemName} ({InventorySystem.Instance.GetItemCount(currentQuest.requiredItem)}/{currentQuest.requiredAmount})";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        // Mantém currentQuest para reaparecer depois
    }
}
