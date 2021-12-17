using UnityEngine;
using UnityEngine.EventSystems;

public class TroopToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string troopName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        int _baseDefense = Constants.troopInfoInt[troopName]["BaseDefense"];
        int _facingDefense = Constants.troopInfoInt[troopName]["FacingDefense"]; ;
        int _health = Constants.troopInfoInt[troopName]["Health"]; ;
        int _attack = Constants.troopInfoInt[troopName]["BaseDefense"]; ;
        int _movement = Constants.troopInfoInt[troopName]["MovementCost"]; ;
        int _attackRange = Constants.troopInfoInt[troopName]["AttackRange"]; ;
        int _seeRange = Constants.troopInfoInt[troopName]["SeeRange"]; ;

        string _text = string.Format("Health: {0}\nAttack: {1}\nMovement: {2}\nAttack Range: " +
            "{3}\nSee Range: {4}\nBase Defense: {5}\n", _health, _attack, _movement,_attackRange, _seeRange, _baseDefense);

        PlayerCS.instance.playerUI.ShowStatsToolTip();
        PlayerCS.instance.playerUI.ChangeStatsToolTipText(_text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerCS.instance.playerUI.HideStatsToolTip();
    }
}
