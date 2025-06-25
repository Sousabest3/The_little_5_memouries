using UnityEngine;

public class SceneMemory : MonoBehaviour
{
    public static SceneMemory Instance;

    public string lastSceneName;
    public string lastEnemyID;

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
}
