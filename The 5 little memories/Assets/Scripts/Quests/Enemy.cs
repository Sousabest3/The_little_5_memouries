using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyTag;

    public void Die()
    {
        QuestManager.Instance.RegisterEnemyKill(enemyTag);
        Destroy(gameObject);
    }
}
