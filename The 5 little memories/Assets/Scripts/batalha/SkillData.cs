using UnityEngine;

public enum SkillType { Damage, Heal, Buff }

[CreateAssetMenu(menuName = "Battle/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public SkillType type;
    public int power;
    public int manaCost;

    // Este é o método que estava faltando:
    public void Use(PlayerCombatant user, EnemyCombatant target)
    {
        if (!user.UseMP(manaCost))
        {
            Debug.Log("MP insuficiente!");
            return;
        }

        switch (type)
        {
            case SkillType.Damage:
                target.TakeDamage(power);
                break;

            case SkillType.Heal:
                user.Heal(power);
                break;

            case SkillType.Buff:
                // Aqui você pode implementar buffs como defesa temporária, etc.
                Debug.Log("Buff aplicado (não implementado)");
                break;
        }
    }

    public void Use(PlayerCombatant user, PlayerCombatant allyTarget)
{
    if (!user.UseMP(manaCost))
    {
        Debug.Log("MP insuficiente!");
        return;
    }

    switch (type)
    {
        case SkillType.Heal:
            allyTarget.Heal(power);
            break;
            
        case SkillType.Buff:
                // Aqui você pode implementar buffs como defesa temporária, etc.
                Debug.Log("Buff aplicado (não implementado)");
                break;
        
    }
}

}
