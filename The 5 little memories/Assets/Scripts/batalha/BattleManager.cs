public class BattleManager : MonoBehaviour
{
    public List<CharacterBase> turnOrder = new List<CharacterBase>();
    public int currentTurnIndex = 0;

    public List<AllyCombatant> allies;
    public List<EnemyCombatant> enemies;

    public static BattleManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BeginBattle();
    }

    public void BeginBattle()
    {
        turnOrder.Clear();
        turnOrder.AddRange(allies);
        turnOrder.AddRange(enemies);
        currentTurnIndex = 0;
        NextTurn();
    }

    public void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

        while (turnOrder[currentTurnIndex].IsDead)
        {
            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        }

        var current = turnOrder[currentTurnIndex];
        if (current.isPlayer)
            BattleUI.Instance.ShowCommands(current as AllyCombatant);
        else
            StartCoroutine(EnemyAction(current as EnemyCombatant));
    }

    IEnumerator EnemyAction(EnemyCombatant enemy)
    {
        yield return new WaitForSeconds(1f);
        enemy.AttackRandomAlly();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void TryRun()
    {
        if (Random.value <= 0.2ff)
        {
            BattleUI.Instance.ShowMessage("Conseguiste fugir!");
            SceneManager.LoadScene("WorldScene"); // ou cena anterior
        }
        else
        {
            BattleUI.Instance.ShowMessage("Fuga falhou!");
            StartCoroutine(EnemyAction(enemies[0])); // contra-ataque
        }
    }
}
