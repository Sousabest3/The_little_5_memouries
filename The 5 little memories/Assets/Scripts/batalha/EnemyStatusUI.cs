using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatusUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text hpText;
    public Image hpBar;
    public Image portraitImage;

    private EnemyCombatant enemy;

    public void Setup(EnemyCombatant combatant)
    {
        enemy = combatant;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (enemy == null || enemy.data == null) return;

        nameText.text = enemy.data.characterName;
        hpText.text = $"{enemy.currentHP} / {enemy.data.maxHP}";
        hpBar.fillAmount = (float)enemy.currentHP / enemy.data.maxHP;
        portraitImage.sprite = enemy.data.portrait;
    }
}
