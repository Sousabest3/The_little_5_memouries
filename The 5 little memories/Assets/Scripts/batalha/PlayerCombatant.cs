using UnityEngine;
using System.Collections.Generic;

public class PlayerCombatant : MonoBehaviour
{
    public CharacterBase data;
    public List<SkillData> skills;

    public int currentHP;
    public int currentMP;

    public string displayName => data.characterName;
    public int maxHP => data.maxHP;
    public int maxMP => data.maxMP;
    public int attackPower => data.attack;
    public Sprite portrait;

    public bool IsAlive => currentHP > 0;

    public void Init()
    {
        currentHP = data.maxHP;
        currentMP = data.maxMP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
    }

    public bool UseMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            return true;
        }
        return false;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void RecoverMP(int amount)
    {
        currentMP = Mathf.Min(currentMP + amount, maxMP);
    }
}
