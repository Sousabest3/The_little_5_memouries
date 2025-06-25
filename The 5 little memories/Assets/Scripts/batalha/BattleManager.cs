using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Combatants")]
    public PlayerCombatant player;
    public PlayerCombatant ally;

    [Header("Enemy")]
    public EnemyCombatant enemyPrefab;
    public BattleEncounterManager encounterManager;

    [Header("UI")]
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
        // Inicializa os combatentes
        player.Init();
        ally.Init();

        // Spawna o inimigo baseado na escolha feita antes
        if (encounterManager != null && encounterManager.chosenEnemy != null)
        {
            EnemyCombatant newEnemy = Instantiate(enemyPrefab, new Vector3(2f, 0, 0), Quaternion.identity);
            newEnemy.data = encounterManager.chosenEnemy;
            newEnemy.Init();
            enemies.Add(newEnemy);
        }

        // Configura a UI e inicia o loop da batalha
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

    if (allEnemiesDead && !battleEnded)
    {
        battleEnded = true;

        // ✅ Dá XP à party (podes ajustar o valor depois)
        GiveXPToParty(50);

        // ✅ Marca o inimigo como derrotado
        if (SceneMemory.Instance != null &&
            EnemyStateManager.Instance != null &&
            !string.IsNullOrEmpty(SceneMemory.Instance.lastEnemyID))
        {
            EnemyStateManager.Instance.MarkDefeated(SceneMemory.Instance.lastEnemyID);
        }

        // ✅ Vai para a cena de vitória (VictoryScreenUI mostra os detalhes)
        SceneManager.LoadScene("VictoryScene");
    }
    else if (allPlayersDead && !battleEnded)
    {
        battleEnded = true;
        SceneManager.LoadScene("GameOverScene");
    }
}

    public EnemyCombatant GetRandomAliveEnemy()
    {
        return enemies.Where(e => e.IsAlive).OrderBy(e => Random.value).FirstOrDefault();
    }

    public void GiveXPToParty(int xpAmount)
    {
        if (VictoryTracker.Instance == null) return;

        VictoryTracker.Instance.Clear();
        var party = new List<PlayerCombatant> { player, ally };

        foreach (var member in party)
        {
            if (!member.IsAlive) continue;

            var stats = new VictoryStats
            {
                characterName = member.data.characterName,
                xpGained = xpAmount,
                levelBefore = member.level,
                hpBefore = member.data.maxHP,
                atkBefore = member.data.attack
            };

            member.AddXP(xpAmount);

            stats.levelAfter = member.level;
            stats.hpAfter = member.data.maxHP;
            stats.atkAfter = member.data.attack;

            VictoryTracker.Instance.results.Add(stats);
        }
    }
}
