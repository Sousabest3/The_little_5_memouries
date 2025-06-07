using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyStatusUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public Image portraitImage; // 🖼️ Retrato do personagem

    private PlayerCombatant player;

    public void Setup(PlayerCombatant p)
    {
        player = p;
        UpdateUI();
    }

    public void UpdateUI()
    {
        nameText.text = player.displayName;
        hpText.text = $"HP: {player.currentHP}/{player.maxHP}";
        mpText.text = $"MP: {player.currentMP}/{player.maxMP}";

        if (portraitImage != null)
            portraitImage.sprite = player.portrait;
    }
}
