using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
    public string displayName;
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int attackPower;

    public bool IsAlive => currentHP > 0;

    public virtual void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
    }

    public virtual void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    public virtual void UseMP(int amount)
    {
        currentMP = Mathf.Max(0, currentMP - amount);
    }
}
