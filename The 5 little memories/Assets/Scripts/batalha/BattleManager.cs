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
        foreach (var enemy in enemies)
            enemy.Init();

        battleUI.Setup(player, ally, enemies.ToArray());
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (!battleEnded)
        {
            yield return PlayerTurn(player);
            yield return PlayerTurn(ally);
            yield return EnemyTurn();

            CheckBattleEnd();
        }
    }

    IEnumerator PlayerTurn(PlayerCombatant combatant)
    {
        if (!combatant.IsAlive) yield break;

        yield return battleUI.ShowPlayerCommand(combatant);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator EnemyTurn()
    {
        var alivePlayers = new List<PlayerCombatant>();
        if (player.IsAlive) alivePlayers.Add(player);
        if (ally.IsAlive) alivePlayers.Add(ally);

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive || alivePlayers.Count == 0) continue;

            PlayerCombatant target = alivePlayers[Random.Range(0, alivePlayers.Count)];
            target.TakeDamage(enemy.data.attack);

            BattleUI.Instance.dialogueBox.text = $"{enemy.data.characterName} atacou {target.data.characterName}!";
            yield return new WaitForSeconds(1f);
        }
    }

    public void CheckBattleEnd()
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
