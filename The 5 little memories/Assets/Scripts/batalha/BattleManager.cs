using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public PlayerCombatant player;
    public PlayerCombatant ally;

    public EnemyCombatant enemyPrefab;
    public BattleEncounterManager encounterManager;
    public BattleUI battleUI;

    private List<EnemyCombatant> enemies = new List<EnemyCombatant>();
    private bool battleEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player.Init();
        ally.Init();

        if (encounterManager != null && encounterManager.chosenEnemy != null)
        {
            EnemyCombatant newEnemy = Instantiate(enemyPrefab, new Vector3(2f, 0, 0), Quaternion.identity);
            newEnemy.data = encounterManager.chosenEnemy;
            newEnemy.Init();
            enemies.Add(newEnemy);
        }

        battleUI.Setup(player, ally, enemies.ToArray());
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (!battleEnded)
        {
            yield return battleUI.ShowPlayerCommand(player);
            yield return battleUI.ShowPlayerCommand(ally);
            yield return EnemyTurn();
            CheckBattleEnd();
        }
    }

    IEnumerator EnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            List<PlayerCombatant> targets = new List<PlayerCombatant>();
            if (player.IsAlive) targets.Add(player);
            if (ally.IsAlive) targets.Add(ally);

            if (targets.Count > 0)
            {
                PlayerCombatant target = targets[Random.Range(0, targets.Count)];
                target.TakeDamage(enemy.data.attack);
                battleUI.dialogueBox.text = $"{enemy.data.characterName} atacou {target.data.characterName}!";
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void CheckBattleEnd()
    {
        bool allEnemiesDead = enemies.TrueForAll(e => !e.IsAlive);
        bool allPlayersDead = !player.IsAlive && !ally.IsAlive;

        if (allEnemiesDead)
        {
            SceneManager.LoadScene("VictoryScene");
            battleEnded = true;
        }
        else if (allPlayersDead)
        {
            SceneManager.LoadScene("GameOverScene");
            battleEnded = true;
        }
    }

    public EnemyCombatant GetRandomAliveEnemy()
    {
        return enemies.Where(e => e.IsAlive).OrderBy(e => Random.value).FirstOrDefault();
    }
}
