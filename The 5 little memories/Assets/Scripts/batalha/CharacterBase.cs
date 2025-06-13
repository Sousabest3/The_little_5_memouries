using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Character Base")]
public class CharacterBase : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public Sprite hurtPortrait;
    public Sprite healPortrait;

    public int maxHP = 100;
    public int maxMP = 30;
    public int attack = 10;

    public List<SkillData> skills;
}
