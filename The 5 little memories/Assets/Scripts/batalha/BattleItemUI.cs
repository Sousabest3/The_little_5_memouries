using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public Button button;

    private Item itemData;

    public void Setup(Item item, PlayerCombatant[] party, System.Action onItemUsed)
    {
        itemData = item;

        // Proteção contra item nulo
        if (itemData == null)
        {
            Debug.LogError("❌ Item está nulo ao configurar o botão!");
            return;
        }

        itemNameText.text = itemData.itemName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            // Proteção contra BattleInventory não inicializado
            if (BattleInventory.Instance == null)
            {
                Debug.LogError("❌ BattleInventory.Instance está nulo.");
                return;
            }

            // Proteção contra party nula ou vazia
            if (party == null || party.Length == 0)
            {
                Debug.LogError("❌ Nenhum membro da party foi fornecido.");
                return;
            }

            // Aplica item no primeiro membro vivo
            foreach (var member in party)
            {
                if (member != null && member.IsAlive)
                {
                    BattleInventory.Instance.UseItem(itemData, member);

                    // Atualiza a UI de inimigos (se existir)
                    if (BattleUI.Instance?.enemyStatusUI != null)
                        BattleUI.Instance.enemyStatusUI.UpdateUI();

                    // Executa a ação de finalização (ex: passar turno)
                    onItemUsed?.Invoke();
                    break;
                }
            }

            // Oculta os painéis após uso
            if (BattleUI.Instance != null)
                BattleUI.Instance.HideAllPanels();
        });
    }
}
