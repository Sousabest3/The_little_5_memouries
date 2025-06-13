using UnityEngine;

public abstract class CharacterCombatant : MonoBehaviour
{
    public CharacterBase data;
    public int currentHP;
    public int currentMP;
    public int level = 1;
    public int currentXP = 0;
    public int xpToLevelUp = 100;

    public bool IsAlive => currentHP > 0;

    public virtual void Init()
    {
        if (data == null)
        {
            Debug.LogError($"{name} está sem CharacterBase!");
            return;
        }

        currentHP = data.maxHP;
        currentMP = data.maxMP;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        BattleUI.Instance.ShowDamage(transform.position, amount, false);
        CameraShake.Instance?.Shake(0.1f, 0.2f);

        var ui = GetComponentInChildren<PartyStatusUI>();
        if (ui != null)
            ui.FlashPortrait(data.hurtPortrait);
    }

    public virtual void Heal(int amount)
    {
        currentHP = Mathf.Min(data.maxHP, currentHP + amount);
        BattleUI.Instance.ShowDamage(transform.position, amount, true);

        var ui = GetComponentInChildren<PartyStatusUI>();
        if (ui != null)
            ui.FlashPortrait(data.healPortrait);
    }

    public void UseMP(int cost)
    {
        currentMP = Mathf.Max(0, currentMP - cost);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToLevelUp)
        {
            level++;
            currentXP = 0;
            xpToLevelUp += 50;
            data.maxHP += 10;
            data.attack += 2;
            BattleUI.Instance.dialogueBox.text += $"\n{data.characterName} subiu para o nível {level}!";
        }
    }
}
