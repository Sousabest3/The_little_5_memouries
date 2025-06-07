using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Character Base")]
public class CharacterBase : ScriptableObject
{
    [Header("Identidade")]
    public string characterName;
    public Sprite portrait;

    [Header("Stats")]
    public int maxHP;
    public int maxMP;
    public int attack;

    [Header("Habilidades")]
    public SkillData[] skills;
}
