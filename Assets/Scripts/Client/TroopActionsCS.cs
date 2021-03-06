using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script handles troop quick menu and troop actions including movement and attack
/// </summary>
public class TroopActionsCS : MonoBehaviour
{
    public TileInfo[] objecstToBeReset;
    private int objectsToBeResetSize, objectsToBeResetCapacity = 100;

    public int whatIsInteractableValue, whatIsDefaultValue;
    public TroopInfo troopInfo;
    private readonly string moveableTileTag = "MoveableTile", attackableTileTag = "AttackableTile", defaultTileTag = "Tile", 
                            conquerableCityTag = "ConquerableCity", cityTag = "City", moveableCityTag = "MoveableCity", 
                            portTag = "Port";

    // Quick Menu
    public GameObject quickMenuContainer;
    public Button createCityButton, conquerCityButton, upgradeShipButton;

    private bool troopInAttackRange;

    private int curretMovementCost;

    #region Set Up

    /// <summary>
    /// Init troop action variables
    /// </summary>
    /// <param name="_troopInfo"> TroopInfo this script is attached to </param>
    public void InitTroopActions(TroopInfo _troopInfo)
    {
        whatIsInteractableValue = LayerMask.NameToLayer("Interactable");
        whatIsDefaultValue = LayerMask.NameToLayer("Default");
        troopInfo = _troopInfo;
        objecstToBeReset = new TileInfo[objectsToBeResetCapacity];
        objectsToBeResetSize = 0;
    }
    #endregion

    #region Interactable Tiles

    /// <summary>
    /// Sets tiles to iteractable if troop can reach them
    /// </summary>
    public void CreateInteractableTiles()
    {
        // Check if player can either commit actions or move
        if (troopInfo.movementCost <= 0 && troopInfo.canAttack == false) return;

        bool _conflictFound = false;
        int _attackRange = troopInfo.isBoat ? troopInfo.shipAttackRange: troopInfo.attackRange;
        curretMovementCost = troopInfo.isBoat ? troopInfo.shipMovementCost : troopInfo.movementCost;

        if (GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex].isRoad && troopInfo.potentialRoadMovementCost > 0)
        {
            curretMovementCost++;
            troopInfo.potentialRoadMovementCost--;
            troopInfo.roadMovementCostUsed++;
        }

        // Create moveable tiles
        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                int z = troopInfo.zIndex + 1;
                while (!_conflictFound && z < troopInfo.zIndex + curretMovementCost + 1)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xIndex, z]))
                        {
                            _conflictFound = true;
                        }
                    }
                    z++;
                }
                break;
            case 90:
                int x = troopInfo.xIndex + 1;
                while (!_conflictFound && x < troopInfo.xIndex + curretMovementCost + 1)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zIndex]))
                        {
                            _conflictFound = true;
                        }
                    }
                    x++;
                }
                break;
            case 180:
                z = troopInfo.zIndex - 1;
                while (!_conflictFound && z > troopInfo.zIndex - curretMovementCost - 1)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xIndex, z]))
                        {
                            _conflictFound = true;
                        }
                    }
                    z--;
                }
                break;
            case 270:
                x = troopInfo.xIndex - 1;
                while (!_conflictFound && x > troopInfo.xIndex - curretMovementCost - 1)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zIndex]))
                        {
                            _conflictFound = true;
                        }
                    }
                    x--;
                }
                break;
            default:
                Debug.LogError("Troop " + troopInfo.id + " rotation is not compatible");
                break;
        }

        // Create attackable tiles
        if (!troopInfo.canAttack) return;
        troopInAttackRange = false;
        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                int z = troopInfo.zIndex + 1;
                while (!_conflictFound && z < troopInfo.zIndex + _attackRange + 1)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if(CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xIndex, z]))
                        {
                            _conflictFound = true;
                        }
                    }
                    z++;
                }
                break;
            case 90:
                int x = troopInfo.xIndex + 1;
                while (!_conflictFound && x < troopInfo.xIndex + _attackRange + 1)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zIndex]))
                        {
                            _conflictFound = true;
                        }
                    }
                    x++;
                }
                break;
            case 180:
                z = troopInfo.zIndex - 1;
                while (!_conflictFound && z > troopInfo.zIndex - _attackRange - 1)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if (CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xIndex, z]))
                        {
                            _conflictFound = true;
                        }
                    }
                    z--;
                }
                break;
            case 270:
                x = troopInfo.xIndex - 1;
                while (!_conflictFound && x > troopInfo.xIndex - _attackRange - 1)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zIndex]))
                        {
                            _conflictFound = true;
                        }
                    }
                    x--;
                }
                break;

            default:
                Debug.LogError("Troop " + troopInfo.id + " rotation is not compatible");
                break;
        }

        // Check if can standing on and can conquer acity
        TileInfo _tileInfo = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        if (_tileInfo.isCity)
        {
            CityInfo _city = GameManagerCS.instance.cities[_tileInfo.cityId];
            if (_city.ownerId != troopInfo.ownerId)
            {
                if (_city.isAbleToBeConquered)
                {
                    _tileInfo.tile.layer = whatIsInteractableValue;
                    _tileInfo.tile.tag = conquerableCityTag;
                }
            }
        }
    }

    /// <summary>
    /// set tile tags and layer for troop to be able to move onto them
    /// returns true if the tile resolves to interactable
    /// </summary>
    /// <param name="_tile"> tile to be modified </param>
    /// <param name="_index"> current index of objectsToBeReset array </param>
    public bool CreateInteractableTilesHelperMovement(TileInfo _tile)
    {
        if (_tile.isOccupied || _tile.isObstacle) return false;

        if(_tile.isRoad && troopInfo.potentialRoadMovementCost > 0)
        {
            curretMovementCost++;
            troopInfo.potentialRoadMovementCost--;
            troopInfo.roadMovementCostUsed++;
        }

        if (_tile.isCity)
        {
            if (GameManagerCS.instance.cities[_tile.cityId].isTrainingTroops &&
                GameManagerCS.instance.cities[_tile.cityId].ownerId == ClientCS.instance.myId)
                return false;

            _tile.tile.layer = whatIsInteractableValue;
            // If city is an enemy set it to moveable city tag otherwise set it to a normal 
            if(GameManagerCS.instance.cities[_tile.cityId].ownerId != ClientCS.instance.myId)
                _tile.tile.tag = moveableCityTag;
            else    
                _tile.tile.tag = moveableTileTag;
            _tile.moveUI.SetActive(true);
            _tile.boxCollider.enabled = true;
        }
        else if(_tile.isBuilding && _tile.buildingName == "Port")
        {
            _tile.tile.layer = whatIsInteractableValue;
            if(!troopInfo.isBoat)
                _tile.tile.tag = portTag;
            else    
                _tile.tile.tag = moveableTileTag;
            _tile.moveUI.SetActive(true);
            _tile.boxCollider.enabled = true;
        }
        else if (_tile.isWater)
        {
            if (troopInfo.isBoat)
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = moveableTileTag;
                _tile.moveUI.SetActive(true);
                _tile.boxCollider.enabled = true;
                // Only allow boat to move onto coast line
                if (!_tile.isWater && troopInfo.isBoat)
                {
                    objecstToBeReset[objectsToBeResetSize] = _tile;
                    objectsToBeResetSize++;
                    return false;
                }
            }
            else
                return false;
        }
        else if (!_tile.isWater)
        {
            _tile.tile.layer = whatIsInteractableValue;
            _tile.tile.tag = moveableTileTag;
            _tile.moveUI.SetActive(true);
            _tile.boxCollider.enabled = true;
            // Only allow boat to move onto coast line
            if (troopInfo.isBoat)
            {
                objecstToBeReset[objectsToBeResetSize] = _tile;
                objectsToBeResetSize++;
                return false;
            }
        }
        
        objecstToBeReset[objectsToBeResetSize] = _tile;
        objectsToBeResetSize++;

        return true;
    }

    // set tile tags and layer for troop to be able to attack troop on said tile
    public bool CreateInteractableTilesHelperAttack(TileInfo _tile)
    {
        if (_tile.isObstacle) return false;
        if (_tile.isOccupied)
        {
            if (GameManagerCS.instance.troops[_tile.occupyingObjectId].ownerId != troopInfo.ownerId)
            {
                troopInAttackRange = true;
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = attackableTileTag;
                _tile.attackUI.SetActive(true);
                _tile.boxCollider.enabled = true;
                objecstToBeReset[objectsToBeResetSize] = _tile;
                objectsToBeResetSize++;

                return true;
            }
        }
        return true;
    }


    /// <summary>
    /// Returns true if the provided coords of represent a real tile
    /// </summary>
    private bool CheckTileExists(int _x, int _z)
    {
        return (_x >= 0 && _x < GameManagerCS.instance.tiles.GetLength(0)
                && _z >= 0 && _z < GameManagerCS.instance.tiles.GetLength(1));
    }

    /// <summary>
    /// Reset tile tags and layermasks to default values
    /// </summary>
    public void ResetAlteredTiles()
    {
        if (objecstToBeReset == null) return;

        TileInfo _tile;

        for(int index = 0; index < objectsToBeResetSize; index++)
        {
            _tile = objecstToBeReset[index];

            if (_tile.isCity)
                _tile.tile.tag = cityTag;
            else
                _tile.tile.tag = defaultTileTag;

            _tile.tile.layer = whatIsDefaultValue;
            _tile.moveUI.SetActive(false);
            _tile.attackUI.SetActive(false);
            if (!_tile.fixedCell)
                _tile.boxCollider.enabled = false;
        }
        objectsToBeResetSize = 0;
    }

    #endregion

    #region Movement

    /// <summary>
    /// Move troop to new tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_newTile"></param>
    public void MoveToNewTileLocal(TileInfo _newTile)
    {
        ResetAlteredTiles();
        HideQuickMenu();
        // If player that was a boat moves onto land then change troop to land troop
        if (!_newTile.isWater && troopInfo.isBoat)
        {
            troopInfo.isBoat = false;
            GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "SwitchModel");
        }
        // Update old tile
        TileInfo _oldTile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        _oldTile.isOccupied = false;
        _oldTile.occupyingObjectId = -1;
        if (_oldTile.isCity)
        {
            CityInfo _city = GameManagerCS.instance.cities[_oldTile.cityId];
            _city.isBeingConquered = false;
            _city.isAbleToBeConquered = false;
        }

        // Move troop while doing the move animation
        PlayerCS.instance.isAnimInProgress = true;
        PlayerCS.instance.animationQueue.Enqueue(DescendTroopMoveAnim(_newTile.xIndex, _newTile.zIndex, false));

        troopInfo.movementCost -= Mathf.Abs(_newTile.xIndex - _oldTile.xIndex) + Mathf.Abs(_newTile.zIndex - _oldTile.zIndex) - troopInfo.roadMovementCostUsed;
        troopInfo.roadMovementCostUsed = 0;
        
        // Update new tile
        _newTile.isOccupied = true;
        _newTile.occupyingObjectId = troopInfo.id;
        // Add tile data to send to server
        GameManagerCS.instance.StoreModifiedTileInfo(_oldTile, "OccupyChange");
        GameManagerCS.instance.StoreModifiedTileInfo(_newTile, "OccupyChange");
    }

    /// <summary>
    /// Move troop to new tile and update modified troop and tile dicts
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"></param>
    public IEnumerator MoveToNewTileRemote(int _newXINdex, int _newZIndex)
    {
        PlayerCS.instance.isAnimInProgress = true;
        StartCoroutine(DescendTroopMoveAnim(_newXINdex, _newZIndex, true));
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator DescendTroopMoveAnim(int _newXIndex, int _newYIndex, bool _remoteTroop)
    {
        while(transform.position.y > -.5)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - .1f, transform.position.z);
            troopInfo.healthTextObject.transform.position = new Vector3(troopInfo.transform.position.x, 
                                                                        troopInfo.healthTextObject.transform.position.y -.1f,
                                                                        troopInfo.transform.position.z);
            // Using overlay texture so turn off health text when troop is under a tile
            if(troopInfo.healthTextObject.transform.position.y < -1 && troopInfo.healthTextObject.activeSelf)
                troopInfo.healthTextObject.SetActive(false);
            yield return new WaitForSeconds(.0001f);
        }
        troopInfo.xIndex = _newXIndex;
        troopInfo.zIndex = _newYIndex;
        troopInfo.troop.transform.position = new Vector3(troopInfo.xIndex, troopInfo.transform.position.y, troopInfo.zIndex);
        troopInfo.healthTextObject.transform.position = new Vector3(troopInfo.transform.position.x, Constants.troopHealthYPositionDescend,
                                                                    troopInfo.transform.position.z);
        // Change to ship model if player moved onto a port
        if(troopInfo.isBoat && !troopInfo.shipModel.gameObject.activeInHierarchy)
        {
            if (troopInfo.isExposed)
            {
                troopInfo.troopModel.SetActive(false);
                troopInfo.shipModel.SetActive(true);
            }
            else
            {
                troopInfo.blurredTroopModel.SetActive(false);
                troopInfo.blurredShipModel.SetActive(true);
            }
        }
        // Change to troop model if player moved onto a land and was a boat before moving
        else if(!troopInfo.isBoat && troopInfo.shipModel.gameObject.activeInHierarchy)
        {
            if (troopInfo.isExposed)
            {
                troopInfo.troopModel.SetActive(true);
                troopInfo.shipModel.SetActive(false);
            }
            else
            {
                troopInfo.blurredTroopModel.SetActive(true);
                troopInfo.blurredShipModel.SetActive(false);
            }
        }
        StartCoroutine(RiseTroopMoveAnim(_remoteTroop));
    }

    public IEnumerator RiseTroopMoveAnim(bool _remoteTroop)
    {
        while(transform.position.y < Constants.troopYPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
            if(troopInfo.healthTextObject.transform.position.y < 0)
            {
                // Using overlay texture so turn on health text when troop is above a tile
                if (troopInfo.healthTextObject.transform.position.y > -1 && !troopInfo.healthTextObject.activeSelf 
                    && troopInfo.isExposed)
                    troopInfo.healthTextObject.SetActive(true);
                troopInfo.healthTextObject.transform.position = new Vector3(troopInfo.transform.position.x,
                                                                troopInfo.healthTextObject.transform.position.y + .1f,
                                                                troopInfo.transform.position.z);
            }
            yield return new WaitForSeconds(.0001f);
        }
        transform.position = new Vector3(transform.position.x, Constants.troopYPosition, transform.position.z);
        troopInfo.healthTextObject.transform.position = new Vector3(troopInfo.transform.position.x,
                                                                    Constants.troopHealthYPositionAscend,
                                                                    troopInfo.transform.position.z);
        if(!_remoteTroop)
        {
            // Add Troopdata to send to server
            GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Move");
            CheckTroopSeeingRange();
            CreateInteractableTiles();
            CheckCanCommitAnyActions();
        }
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    // Move onto enemy city
    public void MoveOntoCity(TileInfo _tile, CityInfo _city)
    {
        _city.isBeingConquered = true;

        GameManagerCS.instance.StoreModifiedCityInfo(_city, "Update");
        MoveToNewTileLocal(_tile);
    }

    /// <summary>
    /// Switches player to canoe stats if player is not a boat when it move to port
    /// </summary>
    /// <param name="_tile"></param>
    public void MoveOntoPort(TileInfo _tile)
    {
        if(!troopInfo.isBoat)
            troopInfo.isBoat = true;

        // Assign canoe stats if canoe stats are not already used
        if(troopInfo.shipName != "Canoe")
        {
            troopInfo.shipName = "Canoe";
            troopInfo.shipAttack = Constants.shipInfoInt["Canoe"]["BaseAttack"];
            troopInfo.shipStealthAttack = Constants.shipInfoInt["Canoe"]["StealthAttack"];
            troopInfo.shipCounterAttack = Constants.shipInfoInt["Canoe"]["CounterAttack"];
            troopInfo.shipBaseDefense = Constants.shipInfoInt["Canoe"]["BaseDefense"];
            troopInfo.shipFacingDefense = Constants.shipInfoInt["Canoe"]["FacingDefense"];
            troopInfo.shipMovementCost = Constants.shipInfoInt["Canoe"]["MovementCost"];
            troopInfo.shipAttackRange = Constants.shipInfoInt["Canoe"]["AttackRange"];
            troopInfo.shipSeeRange = Constants.shipInfoInt["Canoe"]["SeeRange"];

            troopInfo.shipCanMultyKill = Constants.shipInfoBool["Canoe"]["CanMultyKill"];
            troopInfo.shipCanMoveAfterKill = Constants.shipInfoBool["Canoe"]["CanMoveAfterKill"];
        }

        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "SwitchModel");
        MoveToNewTileLocal(_tile);
    }

    /// <summary>
    /// pass in -1 for troop to rotate counter clockwise and 1 to rotate clockwise and update modified troop and tile dicts.
    /// </summary>
    public void RotateLocal(int _rotateValue)
    {
        if (troopInfo.movementCost <= 0 && !troopInAttackRange) return;
        // rotate counter clockwise
        if (_rotateValue == -1)
        {
            if (troopInfo.rotation == 0)
                troopInfo.rotation = 270;
            else
                troopInfo.rotation -= 90;
        }
        // rotate clockwise
        if (_rotateValue == 1)
        {
            if (troopInfo.rotation == 270)
                troopInfo.rotation = 0;
            else
                troopInfo.rotation += 90;
        }
        troopInfo.potentialRoadMovementCost += troopInfo.roadMovementCostUsed;
        troopInfo.roadMovementCostUsed = 0;

        troopInfo.troop.transform.localRotation = Quaternion.Euler(troopInfo.troop.transform.localEulerAngles.x, troopInfo.rotation,
                                                                   troopInfo.troop.transform.localEulerAngles.z);
        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Rotate");
        ResetAlteredTiles();
        CreateInteractableTiles();
    }

    public IEnumerator RotateRemote(int _rotation)
    {
        yield return new WaitForEndOfFrame();
        troopInfo.rotation = _rotation;
        troopInfo.troop.transform.localRotation = Quaternion.Euler(troopInfo.troop.transform.localEulerAngles.x,
                                                                   _rotation,
                                                                   troopInfo.troop.transform.localEulerAngles.z);
        PlayerCS.instance.runningCoroutine = null;
    }

    #endregion

    #region Actions

    /// <summary>
    /// Called from troop quick menu
    /// </summary>
    public void ConquerCity()
    {
        HideQuickMenu();
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        CityInfo _city = GameManagerCS.instance.cities[_tile.cityId];
        ConquerCityHelper(_tile, _city);
    }

    /// <summary>
    /// Conquers city that troop is standing on
    /// </summary>
    /// <param name="_tile"> Tile troop is standing on </param>
    /// <param name="_city"> City troop wants to conquer </param>
    public void ConquerCityHelper(TileInfo _tile, CityInfo _city)
    {
        int _previousCityOwnerId = _city.ownerId;
        _tile.tile.tag = cityTag;
        _tile.tile.layer = whatIsInteractableValue;
        _city.isAbleToBeConquered = false;
        _city.isBeingConquered = false;
        troopInfo.movementCost = 0;
        troopInfo.canAttack = false;
        TroopCanNotCommitAnyMoreActions();

        // Add resources and update resource text   
        PlayerCS.instance.food += (int)Constants.biomeInfo[_tile.biome]["Food"];
        PlayerCS.instance.wood += (int)Constants.biomeInfo[_tile.biome]["Wood"];
        PlayerCS.instance.metal += (int)Constants.biomeInfo[_tile.biome]["Metal"];
        PlayerCS.instance.money += (int)Constants.biomeInfo[_tile.biome]["Money"];
        PlayerCS.instance.population += (int)Constants.biomeInfo[_tile.biome]["Population"];
        PlayerCS.instance.playerUI.SetAllIntResourceUI();

        _city.ownerId = troopInfo.ownerId;
        _city.InitConqueredCity();
        GameManagerCS.instance.StoreModifiedCityInfo(_city, "Conquer");

        for (int x = _city.xIndex - _city.ownerShipRange; x < _city.xIndex + _city.ownerShipRange + 1; x++)
        {
            for (int z = _city.zIndex - _city.ownerShipRange; z < _city.zIndex + _city.ownerShipRange + 1; z++)
            {
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                 && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1))
                {
                    _tile = GameManagerCS.instance.tiles[x, z];
                    // Only make this tile owned by this player if this tile was owned by previous city owner
                    if(_tile.ownerId == _previousCityOwnerId)
                    {
                        _tile.ownerId = _city.ownerId;
                        _tile.ownerShipVisualObject.SetActive(true);
                        // Change color of ownership visual object
                        _tile.ownerShipVisualObject.GetComponent<MeshRenderer>().material.color =
                            Constants.tribeBodyColors[ClientCS.allClients[_tile.ownerId].tribe];
                        GameManagerCS.instance.StoreModifiedTileInfo(_tile, "OwnershipChange");
                    }
                }
            }
        }
        // Update morale and education
        PlayerCS.instance.ResetMoraleAndEducation();
        PlayerCS.instance.citiesOwned++;
        ResetAlteredTiles();
    }

    /// <summary>
    /// Called from troop quick menu
    /// </summary>
    /// <param name="_shipNameToUpgradeTo"></param>
    public void UpgradeShip(string _shipNameToUpgradeTo)
    {
        troopInfo.shipName = _shipNameToUpgradeTo;

        troopInfo.shipAttack = Constants.shipInfoInt[_shipNameToUpgradeTo]["BaseAttack"];
        troopInfo.shipStealthAttack = Constants.shipInfoInt[_shipNameToUpgradeTo]["StealthAttack"];
        troopInfo.shipCounterAttack = Constants.shipInfoInt[_shipNameToUpgradeTo]["CounterAttack"];
        troopInfo.shipBaseDefense = Constants.shipInfoInt[_shipNameToUpgradeTo]["BaseDefense"];
        troopInfo.shipFacingDefense = Constants.shipInfoInt[_shipNameToUpgradeTo]["FacingDefense"];
        troopInfo.shipMovementCost = Constants.shipInfoInt[_shipNameToUpgradeTo]["MovementCost"];
        troopInfo.shipAttackRange = Constants.shipInfoInt[_shipNameToUpgradeTo]["AttackRange"];
        troopInfo.shipSeeRange = Constants.shipInfoInt[_shipNameToUpgradeTo]["SeeRange"];

        troopInfo.shipCanMultyKill = Constants.shipInfoBool[_shipNameToUpgradeTo]["CanMultyKill"];
        troopInfo.shipCanMoveAfterKill = Constants.shipInfoBool[_shipNameToUpgradeTo]["CanMoveAfterKill"];

        // Change Ship model
        Destroy(troopInfo.shipModel);
        troopInfo.shipModel = Instantiate(GameManagerCS.instance.shipModels[_shipNameToUpgradeTo], troopInfo.troop.transform.position,
                                       GameManagerCS.instance.shipModels[_shipNameToUpgradeTo].transform.localRotation);
        troopInfo.shipModel.transform.parent = troopInfo.troop.transform;
        // Fix local rotation since setting parent transform might cause ship model to look in a wrong direction
        troopInfo.shipModel.transform.localRotation = GameManagerCS.instance.shipModels[_shipNameToUpgradeTo].transform.localRotation;

        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Update");
        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "ChangeShipModel");
    }

    /// <summary>
    /// Attack troop on tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_tile"></param>
    public void Attack(TileInfo _tile)
    {
        TroopInfo _troop = GameManagerCS.instance.troops[_tile.occupyingObjectId];
        bool _attackedFromTheBack = _troop.rotation == troopInfo.rotation;
        int _distance = Mathf.Abs(_troop.xIndex - troopInfo.xIndex) + Mathf.Abs(_troop.zIndex - troopInfo.zIndex);
        int _troopAttackedEnvironmentBuff = _tile.biome
                                            == Constants.tribeNativeEnvironment[ClientCS.allClients[_troop.ownerId].tribe]
                                            ? 1 : 0;
        int _troopAttackingEnvironmentBuff = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex].biome 
                                             == PlayerCS.instance.tribe ? 1 : 0;
        int _attackValue;

        HideQuickMenu();
        ResetAlteredTiles();
        AttackAnimLocal(_troop, _distance);

        // Expose troop identity
        if (!_troop.isExposed)
        {
            _troop.isExposed = true;
            if(_troop.isBoat)
            {
                _troop.shipModel.SetActive(true);
                if (_troop.blurredShipModel.activeSelf)
                    _troop.blurredShipModel.SetActive(false);
            }
            else
            {
                _troop.troopModel.SetActive(true);
                if (_troop.blurredTroopModel.activeSelf)
                    _troop.blurredTroopModel.SetActive(false);
            }
            _troop.healthTextObject.SetActive(true);
        }
        troopInfo.canAttack = false;

        // Check if attacking troop back
        if (_attackedFromTheBack)
        {
            if(troopInfo.isBoat)
                _attackValue = troopInfo.shipStealthAttack + _troopAttackingEnvironmentBuff
                                 - _troop.baseDefense - _troopAttackedEnvironmentBuff;
            else
                _attackValue = troopInfo.stealthAttack + _troopAttackingEnvironmentBuff 
                                 - _troop.baseDefense - _troopAttackedEnvironmentBuff;
        }
        else
        {
            if(troopInfo.isBoat)
                _attackValue = troopInfo.shipAttack + _troopAttackingEnvironmentBuff 
                                 - _troop.facingDefense - _troopAttackedEnvironmentBuff;
            else
                _attackValue = troopInfo.baseAttack + _troopAttackingEnvironmentBuff 
                                 - _troop.facingDefense - _troopAttackedEnvironmentBuff;
        }

        // Subtract attack value if it is more than 0. Else subtract 1 health point
        _troop.health -= _attackValue > 0 ? _attackValue : 1;

        // this troop killed the other troop
        if (_troop.health <= 0)
        {
            PlayerCS.instance.troopsKilled++;
            _tile.isOccupied = false;
            GameManagerCS.instance.troops.Remove(_troop.id);
            GameManagerCS.instance.objectsToDestroy.Add(_troop.troop);
            _troop.troopActions.DieAnim();

            if (troopInfo.health > 0 && troopInfo.canMoveAfterKill)
            {
                if (_tile.isCity)
                    MoveOntoCity(_tile, GameManagerCS.instance.cities[_tile.cityId]);
                else
                    MoveToNewTileLocal(_tile);
            }
            if (troopInfo.canMultyKill)
                troopInfo.canAttack = true;
            // Change cities and tiles to neutral if the troop killed was a king
            if(_troop.troopName == "King")
                GameManagerCS.instance.ResetOwnedCitiesAndTiles(_troop.ownerId);
        }
        else
        {
            // Check if can counter attack (only can counter attack if troop remains alive)
            if(!_attackedFromTheBack)
            {
                // If attack was a melee and not a ranged attack then counter attack (counter attack can not be a ranged attack)
                if(_distance == 1)
                    _attackValue = _troop.counterAttack + _troopAttackedEnvironmentBuff 
                                        - troopInfo.baseDefense - _troopAttackingEnvironmentBuff;
                troopInfo.health -= _attackValue > 0 ? _attackValue : 1;
            }
            // Play hurt animation
            _troop.troopActions.HurtAnimLocal();
        }

        // This troop was killed
        if (troopInfo.health <= 0)
        {
            PlayerCS.instance.ownedTroopsKilled++;
            _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
            _tile.isOccupied = false;
            DieAnim();
            if (troopInfo.troopName == "King")
                GameManagerCS.instance.KingIsDead();
            GameManagerCS.instance.troops.Remove(troopInfo.id);
            GameManagerCS.instance.objectsToDestroy.Add(troopInfo.troop);
        }
        // This if statment is here and not in the one with the condition !_attackedFromTheBack for the sake of animations going in order
        else if (!_attackedFromTheBack && _troop.health > 0)
        {
            // If attack was a melee and not a ranged attack then counter attack (counter attack can not be a ranged attack)
            if (_distance == 1)
            {
                _troop.troopActions.AttackAnimLocal(troopInfo, _distance);
                HurtAnimLocal();
            }
        }

        CreateInteractableTiles();
        if(troopInfo.health > 0)
            CheckCanCommitAnyActions();
    }


    /// <summary>
    /// Checks around this troop to see if there are any enemy troops on the tiles within seeing range.
    /// </summary>
    public void CheckTroopSeeingRange()
    {
        TileInfo _tile;
        TroopInfo _troop;

        for (int x = troopInfo.xIndex - troopInfo.seeRange; x < troopInfo.xIndex + troopInfo.seeRange + 1; x++)
        {
            for (int z = troopInfo.zIndex - troopInfo.seeRange; z < troopInfo.zIndex + troopInfo.seeRange + 1; z++)
            {
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                    && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1))
                {

                    _tile = GameManagerCS.instance.tiles[x, z];
                    if (_tile.isOccupied)
                    {
                        _troop = GameManagerCS.instance.troops[_tile.occupyingObjectId];
                        if (_troop.ownerId != troopInfo.ownerId
                            && !IsVisionBlocked(x, z))
                        {
                            // Enemy troop within seeing range
                            if (_troop.isExposed)
                            {
                                _troop.healthTextObject.SetActive(true);
                                if(_troop.isBoat)
                                    _troop.shipModel.SetActive(true);
                                else
                                    _troop.troopModel.SetActive(true);

                                if (_troop.blurredTroopModel.activeSelf)
                                    _troop.blurredTroopModel.SetActive(false);
                                else if (_troop.blurredShipModel.activeSelf)
                                    _troop.blurredShipModel.SetActive(false);
                            }
                            else
                            {
                                if(_troop.isBoat)
                                    _troop.blurredShipModel.SetActive(true);
                                else 
                                    _troop.blurredTroopModel.SetActive(true);
                            }
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// Returns true if there is a troop within 1 tiles from a wall  
    /// and this troop is not withing the 1 tile radius
    /// </summary>
    /// <returns></returns>
    private bool IsVisionBlocked(int _troopXIndex, int _troopZIndex)
    {
        int _xIndex, _zIndex, _distanceFromVisionBlockX, _distanceFromVisionBlockZ, _visionBlockRadius = 1;

        for(_xIndex = _troopXIndex - _visionBlockRadius; _xIndex <= _troopXIndex + _visionBlockRadius; _xIndex++)
        {
            for (_zIndex = _troopZIndex - _visionBlockRadius; _zIndex <= _troopZIndex + _visionBlockRadius; _zIndex++)
            {
                // First check if x index and z index is a valid index of a tile
                // Next check if current tile is a vision blocker
                if ( _xIndex >= 0 && _xIndex < GameManagerCS.instance.tiles.GetLength(0)
                    && _zIndex >= 0 && _zIndex < GameManagerCS.instance.tiles.GetLength(1)
                    && GameManagerCS.instance.tiles[_xIndex, _zIndex].isWall )
                {
                    _distanceFromVisionBlockX = Mathf.Abs(troopInfo.xIndex - _xIndex);
                    _distanceFromVisionBlockZ = Mathf.Abs(troopInfo.zIndex - _zIndex);

                    if(Mathf.Max(_distanceFromVisionBlockX, _distanceFromVisionBlockZ) > _visionBlockRadius)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if troop can commit anymore actions
    /// If troop can't then it calls TroopCanNotCommitAnyMoreActions function
    /// </summary>
    private void CheckCanCommitAnyActions()
    {
        if (troopInfo.movementCost <= 0 && (!troopInAttackRange || !troopInfo.canAttack))
            TroopCanNotCommitAnyMoreActions();
    }

    public void TroopCanNotCommitAnyMoreActions()
    {
        troopInfo.exhaustedParicleSystem.Play();
    }

    public void TroopCanCommitMoreActions()
    {
        troopInfo.exhaustedParicleSystem.Stop();
    }

    #endregion

    #region Troop Animations

    /// <summary>
    /// Sets up attack animations and adds attack information to modified troop dict to send to server
    /// </summary>
    /// <param name="_troopToAttack"></param>
    public void AttackAnimLocal(TroopInfo _troopToAttack, int _distance)
    {
        PlayerCS.instance.isAnimInProgress = true;
        int _attackDirection = 0;
        if (_troopToAttack.xIndex > troopInfo.xIndex)
            _attackDirection = 90;
        else if (_troopToAttack.xIndex < troopInfo.xIndex)
            _attackDirection = 270;
        else if (_troopToAttack.zIndex > troopInfo.zIndex)
            _attackDirection = 0;
        else if (_troopToAttack.zIndex < troopInfo.zIndex)
            _attackDirection = 180;

        if (_distance == 1)
            PlayerCS.instance.animationQueue.Enqueue(SwordAnim(_attackDirection));
        else
            PlayerCS.instance.animationQueue.Enqueue(ArrowAnim(_troopToAttack, _attackDirection));

        troopInfo.attackRotation = _attackDirection;
        troopInfo.lastTroopAttackedId = _troopToAttack.id;
        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Attack");
    }

    /// <summary>
    /// Sets up attack animation coming from server and updates troop info 
    /// </summary>
    /// <param name="_troopInfo"> Troop that is attack </param>
    /// <returns></returns>
    public IEnumerator AttackTroopRemote(TroopInfo _troopInfo)
    {
        yield return new WaitForEndOfFrame();
        TroopInfo _troopToAttack = GameManagerCS.instance.troops[_troopInfo.lastTroopAttackedId];
        troopInfo.lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        troopInfo.attackRotation = _troopInfo.attackRotation;
        int _distance = Mathf.Abs(_troopToAttack.xIndex - troopInfo.xIndex) + Mathf.Abs(_troopToAttack.zIndex - troopInfo.zIndex);
        if (_distance == 1)
            StartCoroutine(SwordAnim(troopInfo.attackRotation));
        else
            StartCoroutine(ArrowAnim(_troopToAttack, troopInfo.attackRotation));
    }

    /// <summary>
    /// Sets up sword animation 
    /// </summary>
    /// <param name="_localXRotation"> attack rotation </param>
    /// <returns></returns>
    public IEnumerator SwordAnim(int _localXRotation)
    {
        GameManagerCS.instance.sword.transform.position = new Vector3(troopInfo.transform.position.x,
                                                             2,
                                                             troopInfo.transform.position.z);
        GameManagerCS.instance.sword.transform.localRotation = Quaternion.Euler(-90, _localXRotation, 0);
        GameManagerCS.instance.sword.SetActive(true);
        StartCoroutine(SwordAnimHelper());
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Plays sword animation (swings sword up and down)
    /// </summary>
    /// <returns></returns>
    public IEnumerator SwordAnimHelper()
    {
        while (GameManagerCS.instance.sword.transform.localEulerAngles.x < .1f 
              || GameManagerCS.instance.sword.transform.localEulerAngles.x > 10.1f)
        {
            GameManagerCS.instance.sword.transform.localRotation *= Quaternion.Euler(10, 0, 0);
            yield return new WaitForSeconds(.01f);
        }
        GameManagerCS.instance.sword.SetActive(false);
        GameManagerCS.instance.sword.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Places arrow one tile in front of troop and adjusts the arrow's angle according to troop's current rotation.
    /// Then the arrow is enabled starts the ArrowAnimHelper function
    /// </summary>
    /// <param name="_troopToAttack"> Troop to attack </param>
    /// <returns></returns>
    public IEnumerator ArrowAnim(TroopInfo _troopToAttack, int _attackRotation)
    {
        PlayerCS.instance.isAnimInProgress = true;
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        switch (_attackRotation)
        {
            case 0:
                GameManagerCS.instance.arrow.transform.position = new Vector3(_tile.transform.position.x,
                                                                              1,
                                                                              _tile.transform.position.z + 1);
                break;
            case 90:
                GameManagerCS.instance.arrow.transform.position = new Vector3(_tile.transform.position.x + 1,
                                                                              1,
                                                                              _tile.transform.position.z);
                break;
            case 180:
                GameManagerCS.instance.arrow.transform.position = new Vector3(_tile.transform.position.x,
                                                                              1,
                                                                              _tile.transform.position.z - 1);
                break;
            case 270:
                GameManagerCS.instance.arrow.transform.position = new Vector3(_tile.transform.position.x - 1,
                                                                              1,
                                                                              _tile.transform.position.z);
                break;
            default:
                Debug.LogError("Could not accomplish task with rotation " + troopInfo.rotation);
                break;
        }
        GameManagerCS.instance.arrow.transform.localRotation = Quaternion.Euler(GameManagerCS.instance.arrow.transform.localEulerAngles.x, 
                                                                                troopInfo.rotation, 0);
        GameManagerCS.instance.arrow.SetActive(true);

        StartCoroutine(ArrowAnimHelper(_troopToAttack));
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Add force to the arrow object from GamaManagerCS to go towards troop that is supposed to be attacked.
    /// The arrow is then disabled when it has reached its target
    /// </summary>
    /// <param name="_troopToAttack"> Troop to attack </param>
    public IEnumerator ArrowAnimHelper(TroopInfo _troopToAttack)
    {
        TileInfo _tileToGoTo = GameManagerCS.instance.tiles[_troopToAttack.xIndex, _troopToAttack.zIndex];

        GameManagerCS.instance.arrowRB.AddForce(troopInfo.troopModel.transform.forward * 200 * Time.deltaTime, ForceMode.Impulse);

        int errorCatcher = 0;
        while(Mathf.Abs(GameManagerCS.instance.arrow.transform.position.x - _tileToGoTo.transform.position.x) > .5f )
        {
            yield return new WaitForSeconds(.01f);
        }
        while (Mathf.Abs(GameManagerCS.instance.arrow.transform.position.z - _tileToGoTo.transform.position.z) > .5f)
        {
            if (errorCatcher > 10000) break;
            errorCatcher++; 
            yield return new WaitForSeconds(.01f);
        }
        GameManagerCS.instance.arrow.SetActive(false);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;

    }

    /// <summary>
    /// Call when troop takes damaage. Places hurt animation in animation queue
    /// </summary>
    public void HurtAnimLocal()
    {
        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Hurt");
        PlayerCS.instance.animationQueue.Enqueue(HurtAnimHelper());
    }

    /// <summary>
    /// Rotate troop 360 degrees on the x axis by rotating 20 degree every .01 seconds of scaled time
    /// </summary>
    public IEnumerator HurtAnimHelper()
    {
        AudioManager.instance.Play(Constants.hitAudio);
        PlayerCS.instance.isAnimInProgress = true;
        troopInfo.healthText.text = troopInfo.health.ToString();
        for (int i = 0; i < 18; i++)
        {
            yield return new WaitForSeconds(.01f);
            troopInfo.troop.transform.localRotation *= Quaternion.Euler(0, 20, 0);
        }
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
        troopInfo.troop.transform.rotation = Quaternion.Euler(0, troopInfo.rotation, 0);
    }

    /// <summary>
    /// Plays troop hurt animation and updates troop info 
    /// </summary>
    /// <param name="_troopToCopy"> Troop info class containing information to copy </param>
    /// <returns></returns>
    public IEnumerator HurtAnimRemote(TroopInfo _troopToCopy)
    {
        troopInfo.health = _troopToCopy.health;
        StartCoroutine(HurtAnimHelper());
        yield return null;
    }

    /// <summary>
    /// Call when troop dies. Resets altered tiles, hides quick menu, and places die animation in queue
    /// </summary>
    public void DieAnim()
    {
        GameManagerCS.instance.StoreModifiedTroopInfo(troopInfo, "Die");
        troopInfo.healthTextObject.SetActive(false);

        // Reset altered tiles and hide troop quick menu if this is a local rather than remote troop
        if(troopInfo.ownerId == ClientCS.instance.myId)
        {
            ResetAlteredTiles();
            HideQuickMenu();
        }
        PlayerCS.instance.animationQueue.Enqueue(DieAnimHelper(true));
    }

    /// <summary>
    /// Rotates troop on the x and z axis while decreasing the troops y position until it is out of view.
    /// When the troop is out of view it is disabled.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DieAnimHelper(bool _setAnimToNull)
    {
        if(_setAnimToNull)
            PlayerCS.instance.isAnimInProgress = true;
        AudioManager.instance.Play(Constants.deathAudio);
        while (troopInfo.troop.transform.localPosition.y > -1.5f)
        {
            troopInfo.troop.transform.localRotation *= Quaternion.Euler(5, 0, 5);
            troopInfo.troop.transform.localPosition = new Vector3(troopInfo.troop.transform.localPosition.x,
                                                                       troopInfo.troop.transform.localPosition.y - .05f,
                                                                       troopInfo.troop.transform.localPosition.z);
            yield return new WaitForSeconds(.001f);
        }
        troopInfo.troop.SetActive(false);
        if(_setAnimToNull)
        {
            PlayerCS.instance.isAnimInProgress = false;
            PlayerCS.instance.runningCoroutine = null;
        }
    }

    #endregion

    #region Troop Quick Menu

    public void ToggleQuickMenu()
    {
        if (quickMenuContainer.activeInHierarchy)
            HideQuickMenu();
        else
            ShowQuickMenu();
    }

    public void HideQuickMenu()
    {
        PlayerCS.instance.playerUI.menuButton.SetActive(true);
        ResetQuickMenu();
    }

    public void ShowQuickMenu()
    {
        PlayerCS.instance.playerUI.menuButton.SetActive(false);
        PlayerCS.instance.HideQuckMenus();
        quickMenuContainer.SetActive(true);

        DisplayerPossibleActions();
    }

    public void DisplayerPossibleActions()
    {
        int _currentXCoord = -700, _xCoordIncrement = 500, _currentYCoord = -400;

        // Deactivate all action buttons beside close 
        createCityButton.gameObject.SetActive(false);
        conquerCityButton.gameObject.SetActive(false);
        upgradeShipButton.gameObject.SetActive(false);

        // Check if troop can conquer a city
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        if(_tile.isCity )
        {
            CityInfo _city = GameManagerCS.instance.cities[_tile.cityId];
            if(_city.isAbleToBeConquered)
            {
                if (troopInfo.troopName == "King")
                {
                    conquerCityButton.gameObject.SetActive(true);
                    conquerCityButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
                    _currentXCoord += _xCoordIncrement;
                }
                else
                {
                    conquerCityButton.gameObject.SetActive(true);
                    conquerCityButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
                    _currentXCoord += _xCoordIncrement;
                }
            }
        }
        if(troopInfo.troopName == "King")
        {
            if (CanCreateCity())
            {
                createCityButton.gameObject.SetActive(true);
                createCityButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
                _currentXCoord += _xCoordIncrement;
            }
        }
        if(troopInfo.isBoat && troopInfo.shipName == "Canoe")
        {
            upgradeShipButton.gameObject.SetActive(true);
            upgradeShipButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
            _currentXCoord += _xCoordIncrement;
        }

        // If no actions possible then close quick menu
        if (!conquerCityButton.gameObject.activeInHierarchy && !upgradeShipButton.gameObject.activeInHierarchy && 
            !createCityButton.gameObject.activeInHierarchy)
        {
            ResetQuickMenu();
        }
    }

    public void ResetQuickMenu()
    {
        quickMenuContainer.SetActive(false);
        PlayerCS.instance.isAbleToCommitActions = true;
        PlayerCS.instance.playerUI.menuButton.SetActive(true);
    }

    public void SetCurrentTroopId()
    {
        PlayerCS.instance.currentSelectedTroopId = troopInfo.id;
    }

    #endregion

    #region King Actions

    /// <summary>
    /// Returns true is the conditions to create a city resolve to true
    /// </summary>
    public bool CanCreateCity()
    {
        bool _tooFarFromNearestOwnedCity = true, _tooFarFromNearestResource = true;
        bool _playerOwnsCities = false;

        // Check if there is an owned city in a certain max distance and return if there are none
        for (int x = troopInfo.xIndex - GameManagerCS.instance.maxDistanceFromResource;
            x <= troopInfo.xIndex + GameManagerCS.instance.maxDistanceFromResource;
            x++)
        {
            for (int z = troopInfo.zIndex - GameManagerCS.instance.maxDistanceFromResource;
                z <= troopInfo.zIndex + GameManagerCS.instance.maxDistanceFromResource;
                z++)
            {
                if (CheckTileExists(x, z))
                {
                    TileInfo _tile = GameManagerCS.instance.tiles[x, z];
                    if (_tile.isFood || _tile.isWood || _tile.isMetal)
                        _tooFarFromNearestResource = false;
                }
            }
        }
        if (_tooFarFromNearestResource) return false;


        // Check if a city is too close and return if there is one
        for (int x = troopInfo.xIndex - GameManagerCS.instance.minDistanceBetweenCities;
            x <= troopInfo.xIndex + GameManagerCS.instance.minDistanceBetweenCities;
            x++)
        {
            for (int z = troopInfo.zIndex - GameManagerCS.instance.minDistanceBetweenCities;
                z <= troopInfo.zIndex + GameManagerCS.instance.minDistanceBetweenCities;
                z++)
            {
                if(CheckTileExists(x, z))
                {
                    if (GameManagerCS.instance.tiles[x, z].isCity)
                        return false;
                }
            }
        }

        // If player does not own city then return true only after checking if resources are close enough and other cities are far enough away
        foreach(CityInfo _city in GameManagerCS.instance.cities.Values)
        {
            if (_city.ownerId == troopInfo.ownerId)
                _playerOwnsCities = true;
        }
        if (!_playerOwnsCities) return true;

        // Check if there is an owned city in a certain max distance and return if there are none
        for (int x = troopInfo.xIndex - GameManagerCS.instance.maxDistanceBetweenCities;
            x <= troopInfo.xIndex + GameManagerCS.instance.maxDistanceBetweenCities;
            x++)
        {
            for (int z = troopInfo.zIndex - GameManagerCS.instance.maxDistanceBetweenCities;
                z <= troopInfo.zIndex + GameManagerCS.instance.maxDistanceBetweenCities;
                z++)
            {
                if (CheckTileExists(x, z))
                {
                    TileInfo _tile = GameManagerCS.instance.tiles[x, z];
                    if (_tile.isCity)
                    {
                        if(GameManagerCS.instance.cities[_tile.cityId].ownerId == troopInfo.ownerId)
                            _tooFarFromNearestOwnedCity = false;
                    }
                }
            }
        }
        if (_tooFarFromNearestOwnedCity) return false;

        return true;
    }

    /// <summary>
    /// Create city 
    /// This function is called through troop quick menu
    /// </summary>
    public void CreateCity()
    {
        if (!CanCreateCity()) return;

        GameManagerCS.instance.SpawnCity(troopInfo.xIndex, troopInfo.zIndex);
    }

    #endregion
}