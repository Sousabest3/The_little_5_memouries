using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int manaCost;
    public int power;
    public bool isHealing;

    public void Use(CharacterCombatant user, CharacterCombatant target)
    {
        if (isHealing)
            target.Heal(power);
        else
            target.TakeDamage(power);

        user.UseMP(manaCost);
    }
}
