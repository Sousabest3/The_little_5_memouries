using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestProgressUpdater : MonoBehaviour
{
    private static QuestProgressUpdater instance;

    // Lista de cenas onde o script deve ser destruído
    private readonly string[] excludedScenes = {
        "Battle",
        "VictoryScene",
        "GameOverScene",
        "Menu",
        "IntroScene"
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var name in excludedScenes)
        {
            if (scene.name == name)
            {
                Debug.Log($"🧹 QuestProgressUpdater removido na cena: {scene.name}");
                Destroy(gameObject);
                return;
            }
        }
    }

    private void Update()
    {
        if (MissionHUDUI.Instance != null && MissionHUDUI.Instance.isActiveAndEnabled)
        {
            MissionHUDUI.Instance.UpdateProgress();
        }
    }
}
