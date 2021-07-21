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

    public bool canMoveNextTurn;
    public bool canMultyKill;

    public void FillTroopInfo(string _troopName, GameObject _troop, TroopActionsCS _troopActions, 
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

    public void FillScoutInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord, 
                              int _rotation)
    {
        troopName = "Scout";
        troop = _troop;
        troopActions = _troopActions;
        troopActions.InitTroopActions(this);
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 3;
        baseAttack = 1;
        stealthAttack = 2;
        counterAttack = 1;
        baseDefense = 0;
        facingDefense = 0;
        movementCost = 5;
        attackRange = 1;
        seeRange = 3;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(3, 1, 0, 0, 5, 3);
    }

    public void FillNormalMilitiaInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                              int _rotation)
    {
        troopName = "NormalMilitia";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 5;
        baseAttack = 3;
        stealthAttack = 4;
        counterAttack = 2;
        baseDefense = 0;
        facingDefense = 1;
        movementCost = 2;
        attackRange = 1;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(5, 3, 0, 1, 2, 3);
    }

    public void FillAdvancedMilitiaInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                              int _rotation)
    {
        troopName = "AdvancedMilitia";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 10;
        baseAttack = 5;
        stealthAttack = 6;
        counterAttack = 4;
        baseDefense = 1;
        facingDefense = 3;
        movementCost = 2;
        attackRange = 1;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(10, 5, 1, 3, 2, 3);
    }

    public void FillMissileInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                              int _rotation)
    {
        troopName = "Missle";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 5;
        baseAttack = 15;
        stealthAttack = 15;
        counterAttack = 10;
        baseDefense = 0;
        facingDefense = 0;
        movementCost = 1;
        attackRange = 5;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(5, 15, 0, 0, 1, 5);
    }

    public void FillDefenseInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                              int _rotation)
    {
        troopName = "Defense";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 15;
        baseAttack = 1;
        stealthAttack = 2;
        counterAttack = 5;
        baseDefense = 5;
        facingDefense = 10;
        movementCost = 1;
        attackRange = 1;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(15, 1, 0, 0, 1, 5);
    }

    public void FillStealthInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                              int _rotation)
    {
        troopName = "Stealth";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 1;
        baseAttack = 1;
        stealthAttack = 15;
        counterAttack = 5;
        baseDefense = 5;
        facingDefense = 10;
        movementCost = 4;
        attackRange = 1;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(15, 1, 0, 0, 1, 5);
    }

    public void FillSnipperInfo(GameObject _troop, TroopActionsCS _troopActions, int _id, int _ownerId, int _xCoord, int _zCoord,
                                int _rotation)
    {
        troopName = "Snipper";
        troop = _troop;
        id = _id;
        ownerId = _ownerId;
        xCoord = _xCoord;
        zCoord = _zCoord;
        rotation = _rotation;

        health = 1;
        baseAttack = 10;
        stealthAttack = 10;
        counterAttack = 0;
        baseDefense = 0;
        facingDefense = 0;
        movementCost = 3;
        attackRange = 4;
        seeRange = 2;
        canMoveNextTurn = true;
        canMultyKill = false;

        //FillGeneralTroopInfo(15, 1, 0, 0, 1, 5);
    }



    public void FillGeneralTroopInfo(int _health, int _baseAttack, int _counterAttack, int _baseDefense, int _facingDefense, int _movementCost, int _attackRange, int _seeRange)
    {
        health = _health;
        baseAttack = _baseAttack;
        counterAttack = _counterAttack;
        baseDefense = _baseDefense;
        facingDefense = _facingDefense;
        movementCost = _movementCost;
        attackRange = _attackRange;
        seeRange = _seeRange;
        canMoveNextTurn = true;
    }
}
