using UnityEngine;

public class EnemyCombatant : CharacterCombatant
{
    private void OnMouseDown()
    {
        BattleUI.Instance.SetSelectedTarget(this);
    }
}
