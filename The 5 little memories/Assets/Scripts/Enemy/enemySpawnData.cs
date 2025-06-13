using UnityEngine;

public static class EnemySpawnData
{
    public static CharacterBase SelectedEnemy;
    public static CharacterBase ExtraEnemy;

    public static CharacterBase[] allEnemies;

    public static CharacterBase GetRandomEnemyExcept(CharacterBase exclude)
    {
        CharacterBase[] candidates = System.Array.FindAll(allEnemies, e => e != exclude);
        if (candidates.Length == 0) return null;
        return candidates[Random.Range(0, candidates.Length)];
    }
}
