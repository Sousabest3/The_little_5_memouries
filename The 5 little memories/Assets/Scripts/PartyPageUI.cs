using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyPageUI : MonoBehaviour
{
    [Header("Player UI")]
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public TMP_Text playerHPText;
    public TMP_Text playerMPText;
    public Image playerPortrait;

    [Header("Ally UI")]
    public GameObject allyPanel; // Ativa/desativa tudo do ally
    public TMP_Text allyNameText;
    public TMP_Text allyLevelText;
    public TMP_Text allyHPText;
    public TMP_Text allyMPText;
    public Image allyPortrait;

    [Header("References")]
    public PlayerCombatant player;

    void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (player != null && player.data != null)
        {
            playerNameText.text = player.data.characterName;
            playerLevelText.text = $"Nv {player.level}";
            playerHPText.text = $"{player.currentHP} / {player.data.maxHP}";
            playerMPText.text = $"{player.currentMP} / {player.data.maxMP}";
            playerPortrait.sprite = player.data.portrait;
        }

        // Verifica se há NPC a seguir
        if (NPCController2D.FollowingNPC != null)
        {
            PlayerCombatant npcCombatant = NPCController2D.FollowingNPC.GetComponent<PlayerCombatant>();

            if (npcCombatant != null && npcCombatant.data != null)
            {
                allyPanel.SetActive(true);
                allyNameText.text = npcCombatant.data.characterName;
                allyLevelText.text = $"Nv {npcCombatant.level}";
                allyHPText.text = $"{npcCombatant.currentHP} / {npcCombatant.data.maxHP}";
                allyMPText.text = $"{npcCombatant.currentMP} / {npcCombatant.data.maxMP}";
                allyPortrait.sprite = npcCombatant.data.portrait;
            }
            else
            {
                allyPanel.SetActive(false);
            }
        }
        else
        {
            allyPanel.SetActive(false);
        }
    }
}
