using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Combatants")]
    public PlayerCombatant player;
    public AllyCombatant ally;
    public List<EnemyCombatant> enemies = new List<EnemyCombatant>();

    [Header("UI")]
    public BattleUI battleUI;

    private int currentTurn = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
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
        yield return PlayerTurn();
        yield return AllyTurn();
        yield return EnemyTurn();

        // Repetir ciclo
        StartCoroutine(BattleLoop());
    }

    IEnumerator PlayerTurn()
    {
        Debug.Log("Turno do jogador...");
        yield return battleUI.ShowPlayerCommand(player);
    }

    IEnumerator AllyTurn()
    {
        Debug.Log("Turno do aliado...");
        if (ally.IsAlive)
        {
            EnemyCombatant target = GetRandomAliveEnemy();
            if (target != null)
            {
                target.TakeDamage(10); // ataque básico
                Debug.Log("Aliado atacou!");
                battleUI.UpdateEnemyStatus();
            }
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Turno dos inimigos...");
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            int targetIndex = Random.Range(0, 2);
            if (targetIndex == 0 && player.IsAlive)
            {
                player.TakeDamage(enemy.attack);
            }
            else if (ally.IsAlive)
            {
                ally.TakeDamage(enemy.attack);
            }
        }

        battleUI.UpdatePartyStatus();
        yield return new WaitForSeconds(1f);
    }

    public EnemyCombatant GetRandomAliveEnemy()
    {
        List<EnemyCombatant> alive = enemies.FindAll(e => e.IsAlive);
        if (alive.Count == 0) return null;
        return alive[Random.Range(0, alive.Count)];
    }
}
