using UnityEngine;

public class EnemyCombatant : CharacterCombatant
{
    private void OnMouseDown()
    {
        BattleUI.Instance.SetSelectedTarget(this);
    }

    protected override void Die()
    {
        Debug.Log($"{data.characterName} morreu!");
        BattleManager.Instance.CheckBattleEnd();
        gameObject.SetActive(false); // ou Destroy(gameObject);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (currentHP <= 0)
        {
            Die();
        }
    }
}
