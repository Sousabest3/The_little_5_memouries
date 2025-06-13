using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public PlayerCombatant player;
    public PlayerCombatant ally;
    public List<EnemyCombatant> enemies = new List<EnemyCombatant>();
    public BattleUI battleUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player.Init();
        ally.Init();
        foreach (var enemy in enemies)
            enemy.Init();

        battleUI.Setup(player, ally, enemies.ToArray());
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        yield return PlayerTurn(player);
        yield return PlayerTurn(ally);
        yield return EnemyTurn();

        CheckBattleEnd();
        StartCoroutine(BattleLoop());
    }

    IEnumerator PlayerTurn(PlayerCombatant combatant)
    {
        if (!combatant.IsAlive) yield break;
        yield return battleUI.ShowPlayerCommand(combatant);
    }

    IEnumerator EnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            var targets = new List<PlayerCombatant> { player, ally };
            PlayerCombatant target = targets.Find(t => t.IsAlive);

            if (target != null)
                target.TakeDamage(enemy.data.attack);

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CheckBattleEnd()
    {
        bool playerDead = !player.IsAlive && !ally.IsAlive;
        bool allEnemiesDead = enemies.TrueForAll(e => !e.IsAlive);

        if (playerDead)
            SceneManager.LoadScene("GameOverScene");
        else if (allEnemiesDead)
        {
            player.AddXP(50);
            ally.AddXP(50);
            SceneManager.LoadScene("VictoryScene");
        }
    }

    public EnemyCombatant GetRandomAliveEnemy()
    {
        var alive = enemies.FindAll(e => e.IsAlive);
        if (alive.Count == 0) return null;
        return alive[Random.Range(0, alive.Count)];
    }

    public void SetTarget(EnemyCombatant target)
    {
        battleUI.SetSelectedTarget(target);
    }
}
