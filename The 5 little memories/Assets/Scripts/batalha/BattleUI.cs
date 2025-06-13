using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public GameObject commandPanel;
    public GameObject skillPanel;
    public GameObject itemPanel;

    public Transform skillListContainer;
    public Transform itemListContainer;
    public GameObject skillButtonPrefab;
    public GameObject itemButtonPrefab;

    public Button attackButton, skillButton, itemButton, fleeButton;

    public PartyStatusUI[] partyStatusUIs;
    public TMP_Text dialogueBox;

    public GameObject damagePopupPrefab;

    private PlayerCombatant currentPlayer;
    private EnemyCombatant selectedEnemy;

    private void Awake() => Instance = this;

    public void Setup(PlayerCombatant player, PlayerCombatant ally, EnemyCombatant[] enemies)
    {
        partyStatusUIs[0].Setup(player);
        partyStatusUIs[1].Setup(ally);
        HideAllPanels();
    }

    public IEnumerator ShowPlayerCommand(PlayerCombatant player)
    {
        currentPlayer = player;
        selectedEnemy = null;

        dialogueBox.text = $"{player.data.characterName}, escolha sua ação...";
        commandPanel.SetActive(true);

        bool actionChosen = false;

        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(() =>
        {
            if (selectedEnemy != null)
            {
                selectedEnemy.TakeDamage(currentPlayer.data.attack);
                dialogueBox.text = $"{player.data.characterName} atacou {selectedEnemy.data.characterName}!";
                actionChosen = true;
            }
        });

        skillButton.onClick.RemoveAllListeners();
        skillButton.onClick.AddListener(() =>
        {
            ShowSkillPanel(player);
        });

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            ShowItemPanel(player);
        });

        fleeButton.onClick.RemoveAllListeners();
        fleeButton.onClick.AddListener(() =>
        {
            if (Random.value < 0.2f)
            {
                dialogueBox.text = "Fuga bem-sucedida!";
                SceneManager.LoadScene("Florest2");
            }
            else
            {
                player.TakeDamage(5);
                dialogueBox.text = "Fuga falhou! Sofreu dano.";
                actionChosen = true;
            }
        });

        while (!actionChosen)
            yield return null;

        HideAllPanels();
    }

    void ShowSkillPanel(PlayerCombatant player)
    {
        skillPanel.SetActive(true);

        foreach (Transform child in skillListContainer)
            Destroy(child.gameObject);

        foreach (var skill in player.data.skills)
        {
            var btn = Instantiate(skillButtonPrefab, skillListContainer);
            btn.GetComponentInChildren<TMP_Text>().text = skill.skillName;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (skill.isHealing)
                {
                    skill.Use(player, player); // Curar a si mesmo (ou abrir para seleção)
                    dialogueBox.text = $"{player.data.characterName} usou {skill.skillName}!";
                }
                else if (selectedEnemy != null)
                {
                    skill.Use(player, selectedEnemy);
                    dialogueBox.text = $"{player.data.characterName} usou {skill.skillName} em {selectedEnemy.data.characterName}!";
                }

                HideAllPanels();
            });
        }
    }

    void ShowItemPanel(PlayerCombatant player)
{
    itemPanel.SetActive(true);

    foreach (Transform child in itemListContainer)
        Destroy(child.gameObject);

    foreach (var stack in BattleInventory.Instance.usableItems)
    {
        var btn = Instantiate(itemButtonPrefab, itemListContainer);
        btn.GetComponent<BattleItemUI>().Setup(stack.item, new PlayerCombatant[] { player });
    }
}


    public void SetSelectedTarget(EnemyCombatant enemy)
    {
        selectedEnemy = enemy;
        dialogueBox.text = $"{enemy.data.characterName} foi selecionado!";
    }

    public void ShowDamage(Vector3 pos, int value, bool isHeal)
    {
        GameObject popup = Instantiate(damagePopupPrefab, pos, Quaternion.identity);
        TMP_Text txt = popup.GetComponentInChildren<TMP_Text>();
        txt.text = value.ToString();
        txt.color = isHeal ? Color.green : Color.red;
    }

    public void HideAllPanels()
    {
        commandPanel.SetActive(false);
        skillPanel.SetActive(false);
        itemPanel.SetActive(false);
    }
}
