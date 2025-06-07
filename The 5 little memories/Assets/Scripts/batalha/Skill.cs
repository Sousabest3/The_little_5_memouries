using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int manaCost;
    public bool isHeal;

    public void Use(PlayerCombatant user)
    {
        user.UseMP(manaCost);

        if (isHeal)
            user.Heal(10);
        else
        {
            EnemyCombatant enemy = BattleManager.Instance.GetRandomAliveEnemy();
            enemy.TakeDamage(15);
        }
    }
}
