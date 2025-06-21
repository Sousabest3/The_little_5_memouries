using UnityEngine;
using UnityEngine.EventSystems;

public class EnemySelector : MonoBehaviour, IPointerClickHandler
{
    public EnemyCombatant enemy;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (enemy.IsAlive)
        {
            BattleUI.Instance.SetSelectedTarget(enemy);
        }
    }
}
