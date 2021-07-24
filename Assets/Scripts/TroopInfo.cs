using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopInfo : MonoBehaviour
{
    public GameObject troop;
    public TroopActionsCS troopActions;
    public string troopName;

    public int id;
    public int ownerId;
    public int xCoord, zCoord;
    public int rotation = 0;
    public int baseDefense;
    public int facingDefense;
    public int health;
    public int baseAttack;
    public int stealthAttack;
    public int counterAttack;
    public int movementCost;
    public int attackRange;
    public int seeRange;
    public int lastTroopAttackedId;
    public int lastHurtById;

    public bool canMoveNextTurn;
    public bool canMultyKill;

    public int idOfPlayerThatSentInfo;

    /// <summary>
    /// Init troop info for new troop
    /// </summary>
    public void InitTroopInfo(string _troopName, GameObject _troop, TroopActionsCS _troopActions, 
                              int _id, int _ownerId, int _xCoord, int _zCoord)
    {
        troopName = _troopName;
        troop = _troop;
        troopActions = _troopActions;
        troopActions.InitTroopActions(this);
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = 0;

        health = Constants.troopInfoInt[_troopName]["Health"];
        baseAttack = Constants.troopInfoInt[_troopName]["BaseAttack"];
        stealthAttack = Constants.troopInfoInt[_troopName]["StealthAttack"];
        counterAttack = Constants.troopInfoInt[_troopName]["CounterAttack"];
        baseDefense = Constants.troopInfoInt[_troopName]["BaseDefense"];
        facingDefense = Constants.troopInfoInt[_troopName]["FacingDefense"];
        movementCost = Constants.troopInfoInt[_troopName]["MovementCost"];
        attackRange = Constants.troopInfoInt[_troopName]["AttackRange"];
        seeRange = Constants.troopInfoInt[_troopName]["SeeRange"];
        canMoveNextTurn = true;
        canMultyKill = Constants.troopInfoBool[_troopName]["CanMultyKill"];
    }
    
    /// <summary>
    /// Copy existing troop info
    /// </summary>
    /// <param name="_troopInfo"> Troop info </param>
    /// <param name="_troop"> troop gameobject </param>
    /// <param name="_troopActions"> troop actions component </param>
    public void InitTroopInfo(TroopInfo _troopInfo, GameObject _troop, TroopActionsCS _troopActions)
    {
        troopName = _troopInfo.name;
        troop = _troop;
        troopActions = _troopActions;
        troopActions.InitTroopActions(this);
        id = _troopInfo.id;
        ownerId = _troopInfo.ownerId;
        xCoord = _troopInfo.xCoord;
        zCoord = _troopInfo.zCoord;
        rotation = _troopInfo.rotation;

        health = _troopInfo.health;
        baseAttack = _troopInfo.baseAttack;
        stealthAttack = _troopInfo.stealthAttack;
        counterAttack = _troopInfo.counterAttack;
        baseDefense = _troopInfo.baseDefense;
        facingDefense = _troopInfo.facingDefense;
        movementCost = _troopInfo.movementCost;
        attackRange = _troopInfo.attackRange;
        seeRange = _troopInfo.seeRange;
        lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        lastHurtById = _troopInfo.lastHurtById;
        canMoveNextTurn = _troopInfo.canMoveNextTurn;
        canMultyKill = _troopInfo.canMultyKill;
    }

    public void CopyTroopInfo(TroopInfo _troopInfo)
    {
        id = _troopInfo.id;
        ownerId = _troopInfo.ownerId;
        xCoord = _troopInfo.xCoord;
        zCoord = _troopInfo.zCoord;
        rotation = _troopInfo.rotation;
        health = _troopInfo.health;
        baseAttack = _troopInfo.baseAttack;
        stealthAttack = _troopInfo.stealthAttack;
        counterAttack = _troopInfo.counterAttack;
        baseDefense = _troopInfo.baseDefense;
        facingDefense = _troopInfo.facingDefense;
        movementCost = _troopInfo.movementCost;
        attackRange = _troopInfo.attackRange;
        seeRange = _troopInfo.seeRange;
        lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        lastHurtById = _troopInfo.lastHurtById;
        canMoveNextTurn = _troopInfo.canMoveNextTurn;
        canMultyKill = _troopInfo.canMultyKill;
    }
}
