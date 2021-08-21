using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TroopInfo : MonoBehaviour
{
    public GameObject troop;
    public GameObject troopModel;
    public GameObject blurredTroopModel;
    public GameObject shipModel;
    public GameObject blurredShipModel;
    public TroopActionsCS troopActions;
    public GameObject healthTextObject;
    public TextMeshPro healthText;
    public BoxCollider boxCollider;
    public string troopName;

    public int id;
    public int ownerId;
    public int xIndex, zIndex;
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

    public bool canAttack;
    public bool canMultyKill;
    public bool canMoveAfterKill;
    public bool isExposed = false;
    public bool isBoat = false;

    public int idOfPlayerThatSentInfo;

    // Attack/Animation variables
    public int attackRotation;
    public int lastTroopAttackedId;

    /// <summary>
    /// Init troop info for new troop and set values for troop to be used this round
    /// </summary>
    public void InitTroopInfo(string _troopName, TroopActionsCS _troopActions, 
                              int _id, int _ownerId, int _xIndex, int _zIndex)
    {
        troopName = _troopName;
        troopActions = _troopActions;
        troopActions.InitTroopActions(this);
        id = _id;
        ownerId = _ownerId;
        xIndex = _xIndex;
        zIndex = _zIndex;
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
        canAttack = true;
        canMultyKill = Constants.troopInfoBool[_troopName]["CanMultyKill"];
        canMoveAfterKill = Constants.troopInfoBool[_troopName]["CanMoveAfterKill"];

        if (GetComponent<BoxCollider>() != null)
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = true;
        }
    }

    /// <summary>
    /// Copy existing troop info
    /// </summary>
    /// <param name="_troopInfo"> Troop info </param>
    /// <param name="_troop"> troop gameobject </param>
    /// <param name="_troopActions"> troop actions component </param>
    public void CopyTroopInfo(TroopInfo _troopInfo, TroopActionsCS _troopActions)
    {
        troopName = _troopInfo.troopName;
        troopActions = _troopActions;
        troopActions.InitTroopActions(this);
        id = _troopInfo.id;
        ownerId = _troopInfo.ownerId;
        xIndex = _troopInfo.xIndex;
        zIndex = _troopInfo.zIndex;
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
        canMultyKill = _troopInfo.canMultyKill;
        canMoveAfterKill = _troopInfo.canMoveAfterKill;
    }

    public void CopyNecessaryTroopInfoToSendToServer(TroopInfo _troopInfo)
    {
        troopName = _troopInfo.troopName;
        id = _troopInfo.id;
        ownerId = _troopInfo.ownerId;
        xIndex = _troopInfo.xIndex;
        zIndex = _troopInfo.zIndex;
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
        canMultyKill = _troopInfo.canMultyKill;
        canMoveAfterKill = _troopInfo.canMoveAfterKill;

        lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        attackRotation = _troopInfo.attackRotation;
    }

    /// <summary>
    /// Update troop info with another troop info values (used for updating troops for data coming from server)
    /// </summary>
    /// <param name="_troopInfo"></param>
    public void UpdateTroopInfo(TroopInfo _troopInfo)
    {
        id = _troopInfo.id;
        ownerId = _troopInfo.ownerId;
        xIndex = _troopInfo.xIndex;
        zIndex = _troopInfo.zIndex;
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
        canMultyKill = _troopInfo.canMultyKill;

        lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        attackRotation = _troopInfo.attackRotation;
    }
}
