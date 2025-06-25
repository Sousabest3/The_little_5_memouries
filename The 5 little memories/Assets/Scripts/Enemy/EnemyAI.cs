using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour, IInteractable
{
    public string enemyID; // ID único
    public CharacterBase enemyData;
    public BattleEncounterManager encounterManager;
    public string battleSceneName = "Battle";

    private void Start()
    {
        // Se esse inimigo já foi derrotado, destrói o GameObject
        if (EnemyStateManager.Instance != null && EnemyStateManager.Instance.IsDefeated(enemyID))
        {
            Destroy(gameObject);
        }
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        if (encounterManager != null)
        {
            encounterManager.chosenEnemy = enemyData;
        }

        // Guarda a cena atual e o ID do inimigo
        if (SceneMemory.Instance != null)
        {
            SceneMemory.Instance.lastSceneName = SceneManager.GetActiveScene().name;
            SceneMemory.Instance.lastEnemyID = enemyID;
        }

        SceneManager.LoadScene(battleSceneName);
    }
}
