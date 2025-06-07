using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    public GameObject commandPanel;
    public GameObject skillPanel;
    public GameObject itemPanel;

    public Button attackButton, skillButton, guardButton, fleeButton;
    public Transform skillListContainer;
    public GameObject skillButtonPrefab;

    public PartyStatusUI[] partyStatusUIs;

    private PlayerCombatant currentPlayer;

    public void Setup(PlayerCombatant player, AllyCombatant ally, EnemyCombatant[] enemies)
    {
        partyStatusUIs[0].Setup(player);
        partyStatusUIs[1].Setup(ally);

        HideAllPanels();
    }

    public IEnumerator ShowPlayerCommand(PlayerCombatant player)
    {
        currentPlayer = player;
        bool waiting = true;

        commandPanel.SetActive(true);

        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(() =>
        {
            OnAttack();
            waiting = false;
        });

        skillButton.onClick.RemoveAllListeners();
        skillButton.onClick.AddListener(() =>
        {
            OnSkill();
            waiting = false;
        });

        guardButton.onClick.RemoveAllListeners();
        guardButton.onClick.AddListener(() =>
        {
            OnGuard();
            waiting = false;
        });

        fleeButton.onClick.RemoveAllListeners();
        fleeButton.onClick.AddListener(() =>
        {
            OnFlee();
            waiting = false;
        });

        while (waiting)
            yield return null;

        HideAllPanels();
    }

    void OnAttack()
    {
        EnemyCombatant target = BattleManager.Instance.GetRandomAliveEnemy();
        if (target != null)
        {
            target.TakeDamage(currentPlayer.attackPower);
            UpdateEnemyStatus();
        }
        UpdatePartyStatus();
    }

    void OnSkill()
    {
        skillPanel.SetActive(true);

        foreach (Transform child in skillListContainer)
            Destroy(child.gameObject);

        foreach (var skill in currentPlayer.skills)
        {
            GameObject btn = Instantiate(skillButtonPrefab, skillListContainer);
            btn.GetComponentInChildren<TMP_Text>().text = $"{skill.skillName} ({skill.manaCost} MP)";
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentPlayer.currentMP >= skill.manaCost)
                {
                    skill.Use(currentPlayer); // ← você pode passar um inimigo como alvo
                    UpdatePartyStatus();
                }
            });
        }
    }

    void OnGuard()
    {
        Debug.Log("Defendendo...");
        // Você pode ativar um flag como currentPlayer.isDefending = true;
    }

    void OnFlee()
    {
        if (Random.value < 0.2f)
        {
            Debug.Log("Fuga bem-sucedida!");
            // SceneManager.LoadScene(...);
        }
        else
        {
            Debug.Log("Fuga falhou! Tomou dano.");
            currentPlayer.TakeDamage(5);
            UpdatePartyStatus();
        }
    }

    public void UpdatePartyStatus()
    {
        foreach (var ui in partyStatusUIs)
        {
            ui.UpdateUI();
        }
    }

    public void UpdateEnemyStatus()
    {
        // TODO: implementar se houver UI de inimigos
    }

    void HideAllPanels()
    {
        commandPanel.SetActive(false);
        skillPanel.SetActive(false);
        itemPanel.SetActive(false);
    }
}
