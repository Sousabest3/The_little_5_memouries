using UnityEngine;

public class EnemyCombatant : MonoBehaviour
{
    public CharacterBase data;
    public int currentHP;

    public int attack => data.attack;
    public bool IsAlive => currentHP > 0;

    public void Init()
    {
        currentHP = data.maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
    }
}
