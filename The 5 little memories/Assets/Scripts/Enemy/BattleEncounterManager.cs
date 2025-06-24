using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Battle Encounter Manager")]
public class BattleEncounterManager : ScriptableObject
{
    public CharacterBase[] enemyPool;
    [HideInInspector] public CharacterBase chosenEnemy;

    public void ChooseRandomEnemy()
    {
        if (enemyPool.Length > 0)
        {
            chosenEnemy = enemyPool[Random.Range(0, enemyPool.Length)];
        }
        else
        {
            Debug.LogWarning("enemyPool está vazio!");
        }
    }
}
