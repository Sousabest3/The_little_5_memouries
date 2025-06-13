using UnityEngine;
using UnityEngine.UI;
using TMPro;

// BattleItemUI.cs já está pronto
public class BattleItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public Button button;

    private Item itemData;

    public void Setup(Item item, PlayerCombatant[] party)
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
                    BattleUI.Instance.dialogueBox.text = $"{member.data.characterName} usou {itemData.itemName}!";
                    break;
                }
            }

            BattleUI.Instance.HideAllPanels();
        });
    }
}
