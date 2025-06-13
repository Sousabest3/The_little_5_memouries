using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PartyStatusUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public Image hpBar;
    public Image mpBar;
    public Image portraitImage;

    private PlayerCombatant player;

    public void Setup(PlayerCombatant combatant)
    {
        player = combatant;
        UpdateUI();
    }

    private void Update()
    {
        if (player != null)
            UpdateUI();
    }

    public void UpdateUI()
    {
        nameText.text = player.data.characterName;
        hpText.text = $"{player.currentHP} / {player.data.maxHP}";
        mpText.text = $"{player.currentMP} / {player.data.maxMP}";
        hpBar.fillAmount = (float)player.currentHP / player.data.maxHP;
        mpBar.fillAmount = (float)player.currentMP / player.data.maxMP;
        portraitImage.sprite = player.data.portrait;
    }

    public void FlashPortrait(Sprite newPortrait)
    {
        StartCoroutine(SwapPortrait(newPortrait));
    }

    private IEnumerator SwapPortrait(Sprite sprite)
    {
        portraitImage.sprite = sprite;
        yield return new WaitForSeconds(1f);
        portraitImage.sprite = player.data.portrait;
    }
}
