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
    public GameObject rotationIndicationModel;
    public TroopActionsCS troopActions;
    public GameObject healthTextObject;
    public TextMeshPro healthText;
    public BoxCollider boxCollider;
    public ParticleSystem exhaustedParicleSystem;
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

    // Ship variables
    public string shipName;
    public int shipAttack;
    public int shipStealthAttack;
    public int shipCounterAttack;
    public int shipBaseDefense;
    public int shipFacingDefense;
    public int shipMovementCost;
    public int shipAttackRange;
    public int shipSeeRange;
    public bool shipCanMultyKill;
    public bool shipCanMoveAfterKill;

    /// <summary>
    /// Init troop info for new troop and set values for troop to be used this round
    /// </summary>
    public void InitTroopInfo(string _troopName, TroopActionsCS _troopActions, int _id, int _ownerId, 
                              string _shipName, int _xIndex, int _zIndex)
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

        shipName = _shipName;

        shipAttack = Constants.shipInfoInt[_shipName]["BaseAttack"];
        shipStealthAttack = Constants.shipInfoInt[_shipName]["StealthAttack"];
        shipCounterAttack = Constants.shipInfoInt[_shipName]["CounterAttack"];
        shipBaseDefense = Constants.shipInfoInt[_shipName]["BaseDefense"];
        shipFacingDefense = Constants.shipInfoInt[_shipName]["FacingDefense"];
        shipMovementCost = Constants.shipInfoInt[_shipName]["MovementCost"];
        shipAttackRange = Constants.shipInfoInt[_shipName]["AttackRange"];
        shipSeeRange = Constants.shipInfoInt[_shipName]["SeeRange"];

        shipCanMultyKill = Constants.shipInfoBool[_shipName]["CanMultyKill"];
        shipCanMoveAfterKill = Constants.shipInfoBool[_shipName]["CanMoveAfterKill"];

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

        shipName = _troopInfo.shipName;
        shipAttack = _troopInfo.shipAttack;
        shipStealthAttack = _troopInfo.shipStealthAttack;
        shipCounterAttack = _troopInfo.shipCounterAttack;
        shipBaseDefense = _troopInfo.shipBaseDefense;
        shipFacingDefense = _troopInfo.shipFacingDefense;
        shipMovementCost = _troopInfo.shipMovementCost;
        shipAttackRange = _troopInfo.shipAttackRange;
        shipSeeRange = _troopInfo.shipSeeRange;

        shipCanMultyKill = _troopInfo.shipCanMultyKill;
        shipCanMoveAfterKill = _troopInfo.shipCanMoveAfterKill;
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
        isBoat = _troopInfo.isBoat;

        lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        attackRotation = _troopInfo.attackRotation;

        shipName = _troopInfo.shipName;
        shipAttack = _troopInfo.shipAttack;
        shipStealthAttack = _troopInfo.shipStealthAttack;
        shipCounterAttack = _troopInfo.shipCounterAttack;
        shipBaseDefense = _troopInfo.shipBaseDefense;
        shipFacingDefense = _troopInfo.shipFacingDefense;
        shipMovementCost = _troopInfo.shipMovementCost;
        shipAttackRange = _troopInfo.shipAttackRange;
        shipSeeRange = _troopInfo.shipSeeRange;

        shipCanMultyKill = _troopInfo.shipCanMultyKill;
        shipCanMoveAfterKill = _troopInfo.shipCanMoveAfterKill;
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

        shipName = _troopInfo.shipName;
        shipAttack = _troopInfo.shipAttack;
        shipStealthAttack = _troopInfo.shipStealthAttack;
        shipCounterAttack = _troopInfo.shipCounterAttack;
        shipBaseDefense = _troopInfo.shipBaseDefense;
        shipFacingDefense = _troopInfo.shipFacingDefense;
        shipMovementCost = _troopInfo.shipMovementCost;
        shipAttackRange = _troopInfo.shipAttackRange;
        shipSeeRange = _troopInfo.shipSeeRange;

        shipCanMultyKill = _troopInfo.shipCanMultyKill;
        shipCanMoveAfterKill = _troopInfo.shipCanMoveAfterKill;
    }
}
