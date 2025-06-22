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
        itemNameText.text = item.itemName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            foreach (var member in party)
            {
                if (member.IsAlive)
                {
                    BattleInventory.Instance.UseItem(itemData, member);
                    BattleUI.Instance.enemyStatusUI.UpdateUI();
                    onItemUsed?.Invoke(); // ✅ aciona finalização da ação
                    break;
                }
            }

            BattleUI.Instance.HideAllPanels();
        });
    }
}
