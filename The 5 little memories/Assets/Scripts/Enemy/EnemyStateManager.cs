using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance;

    private HashSet<string> defeatedEnemies = new HashSet<string>();

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

    public void MarkDefeated(string enemyID)
    {
        defeatedEnemies.Add(enemyID);
    }

    public bool IsDefeated(string enemyID)
    {
        return defeatedEnemies.Contains(enemyID);
    }
}
