using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopActionsCS : MonoBehaviour
{
    public TileInfo[] objecstToBeReset;
    public int whatIsInteractableValue, whatIsDefaultValue;
    private string moveableTileTag = "MoveableTile", attackableTileTag = "AttackableTile", defaultTileTag = "Tile";
    private string conquerableCityTag = "ConquerableCity", cityTag = "City", moveableCityTag = "MoveableCity";
    public TroopInfo troopInfo;

    // Quick Menu
    public GameObject quickMenuContainer, mainContainer, mainKingContainer;
    public Button createCityButton, conquerMainCityButton, conquerKingCityButton;

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
    }
    #endregion

    #region Interactable Tiles

    /// <summary>
    /// Sets tiles to iteractable if troop can reach them
    /// </summary>
    public void CreateInteractableTiles()
    {
        if (troopInfo.movementCost <= 0 && troopInfo.canAttack == false) return;
        int _index = 0;
        objecstToBeReset = new TileInfo[troopInfo.movementCost + troopInfo.attackRange];

        // Create moveable tiles
        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                for (int z = troopInfo.zIndex + 1; z < troopInfo.zIndex + troopInfo.movementCost + 1; z++)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xIndex, z], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xIndex + 1; x < troopInfo.xIndex + troopInfo.movementCost + 1; x++)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zIndex], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zIndex - 1; z > troopInfo.zIndex - troopInfo.movementCost - 1; z--)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xIndex, z], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xIndex - 1; x > troopInfo.xIndex - troopInfo.movementCost - 1; x--)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zIndex], _index))
                            break;
                        _index++;
                    }
                }
                break;

            default:
                Debug.LogError("Troop " + troopInfo.id + " rotation is not compatible");
                break;
        }

        // Create attackable tiles
        if (!troopInfo.canAttack) return;
        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                for (int z = troopInfo.zIndex + 1; z < troopInfo.zIndex + troopInfo.attackRange + 1; z++)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xIndex, z], _index);
                        _index++;
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xIndex + 1; x < troopInfo.xIndex + troopInfo.attackRange + 1; x++)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zIndex], _index);
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zIndex - 1; z > troopInfo.zIndex - troopInfo.attackRange - 1; z--)
                {
                    if (CheckTileExists(troopInfo.xIndex, z))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xIndex, z], _index);
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xIndex - 1; x > troopInfo.xIndex - troopInfo.attackRange - 1; x--)
                {
                    if (CheckTileExists(x, troopInfo.zIndex))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zIndex], _index);
                        _index++;
                    }
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
    public bool CreateInteractableTilesHelperMovement(TileInfo _tile, int _index)
    {
        if (_tile.isWater || _tile.isOccupied || _tile.isObstacle) return false;
        if (_tile.isCity)
        {
            if (GameManagerCS.instance.cities[_tile.cityId].isTrainingTroops &&
                GameManagerCS.instance.cities[_tile.cityId].ownerId != ClientCS.instance.myId)
                return false;
            _tile.tile.layer = whatIsInteractableValue;
            _tile.tile.tag = moveableCityTag;
            _tile.moveUI.SetActive(true);
        }
        else
        {
            _tile.tile.layer = whatIsInteractableValue;
            _tile.tile.tag = moveableTileTag;
            _tile.moveUI.SetActive(true);
        }
        objecstToBeReset[_index] = _tile;
        return true;
    }

    // set tile tags and layer for troop to be able to attack troop on said tile
    public void CreateInteractableTilesHelperAttack(TileInfo _tile, int _index)
    {
        if (_tile.isObstacle) return;
        if (_tile.isOccupied)
        {
            if (GameManagerCS.instance.troops[_tile.occupyingObjectId].ownerId != troopInfo.ownerId)
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = attackableTileTag;
                _tile.attackUI.SetActive(true);
            }
        }
        objecstToBeReset[_index] = _tile;
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
        foreach (TileInfo _tile in objecstToBeReset)
        {
            if (_tile != null)
            {
                if (_tile.isCity)
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = cityTag;
                    _tile.moveUI.SetActive(false);
                    _tile.attackUI.SetActive(false);
                }
                else
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = defaultTileTag;
                    _tile.moveUI.SetActive(false);
                    _tile.attackUI.SetActive(false);
                }
                _tile.moveUI.SetActive(false);
            }
        }
        objecstToBeReset = null;
    }

    #endregion

    #region Movement

    /// <summary>
    /// Move troop to new tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_newTile"></param>
    public void MoveToNewTile(TileInfo _newTile)
    {
        ResetAlteredTiles();
        HideQuickMenu();
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
        //StartCoroutine(DescendTroopMoveAnim(_oldTile, _newTile));
        PlayerCS.instance.isAnimInProgress = true;
        PlayerCS.instance.animationQueue.Enqueue(DescendTroopMoveAnim(_oldTile, _newTile));

        // Update new tile
        troopInfo.movementCost -= Mathf.Abs(_newTile.xIndex - _oldTile.xIndex) + Mathf.Abs(_newTile.zIndex - _oldTile.zIndex);
        if (troopInfo.movementCost < 0) troopInfo.movementCost = 0;
        _newTile.isOccupied = true;
        _newTile.occupyingObjectId = troopInfo.id;
        // Add tile data to send to server
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_oldTile, "Update"} };
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
        _tileData = new Dictionary<TileInfo, string>()
            { {_newTile, "Update"} };
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    public IEnumerator DescendTroopMoveAnim(TileInfo _oldTile, TileInfo _newTile)
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
        troopInfo.xIndex = (int)_newTile.position.x;
        troopInfo.zIndex = (int)_newTile.position.y;
        troopInfo.troop.transform.position = new Vector3(troopInfo.xIndex, troopInfo.transform.position.y, troopInfo.zIndex);
        troopInfo.healthTextObject.transform.position = new Vector3(troopInfo.transform.position.x, Constants.troopHealthYPositionDescend,
                                                                    troopInfo.transform.position.z);
        StartCoroutine(RiseTroopMoveAnim(_oldTile, _newTile));
    }

    public IEnumerator RiseTroopMoveAnim(TileInfo _oldTile, TileInfo _newTile)
    {
        while(transform.position.y < Constants.troopYPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
            if(troopInfo.healthTextObject.transform.position.y < 0)
            {
                // Using overlay texture so turn on health text when troop is above a tile
                if (troopInfo.healthTextObject.transform.position.y > -1 && !troopInfo.healthTextObject.activeSelf)
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
        // Add Troopdata to send to server
        TroopInfo _copiedTroop = GameManagerCS.instance.dataStoringObject.AddComponent<TroopInfo>();
        _copiedTroop.CopyNecessaryTroopInfoToSendToServer(troopInfo);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
        { {_copiedTroop, "Move"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        CheckTroopSeeingRange();
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    public void MoveOntoCity(TileInfo _tile, CityInfo _city)
    {
        _city.isBeingConquered = true;

        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { _city, "Update" } };
        MoveToNewTile(_tile);
    }

    /// <summary>
    /// pass in -1 for troop to rotate counter clockwise and 1 to rotate clockwise and update modified troop and tile dicts.
    /// </summary>
    public void Rotate(int _rotateValue)
    {
        if (troopInfo.movementCost <= 0 && troopInfo.canAttack == false) return;
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
        troopInfo.troop.transform.localRotation = Quaternion.Euler(troopInfo.troop.transform.localEulerAngles.x, troopInfo.rotation,
                                                                   troopInfo.troop.transform.localEulerAngles.z);
        TroopInfo _copiedTroop = GameManagerCS.instance.dataStoringObject.AddComponent<TroopInfo>();
        _copiedTroop.CopyNecessaryTroopInfoToSendToServer(troopInfo);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_copiedTroop, "Rotate"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        ResetAlteredTiles();
        CreateInteractableTiles();
    }

    #endregion

    #region Actions

    public void ConquerCity()
    {
        HideQuickMenu();
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        CityInfo _city = GameManagerCS.instance.cities[_tile.cityId];
        ConquerCity(_tile, _city);
    }

    public void ConquerCity(TileInfo _tile, CityInfo _city)
    {
        int _previousCityOwnerId = _city.ownerId;
        _tile.tile.tag = cityTag;
        _tile.tile.layer = whatIsInteractableValue;
        troopInfo.movementCost = 0;
        troopInfo.canAttack = false;
        _city.isAbleToBeConquered = false;
        _city.isBeingConquered = false;

        _city.ownerId = troopInfo.ownerId;
        _city.InitConqueredCity();
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { _city, "Conquer" } };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);

        Dictionary<TileInfo, string> _tileData;
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
                        _tileData = new Dictionary<TileInfo, string>()
                        { { _tile, "Owned"} };
                        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
                    }
                }
            }
        }

        // Update morale and education
        PlayerCS.instance.ResetMoraleAndEducation();
        ResetAlteredTiles();
    }

    /// <summary>
    /// Attack troop on tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_tile"></param>
    public void Attack(TileInfo _tile)
    {
        HideQuickMenu();
        ResetAlteredTiles();
        TroopInfo _troop = GameManagerCS.instance.troops[_tile.occupyingObjectId];
        bool _attackedFromTheBack = _troop.rotation == troopInfo.rotation;
        AttackAnim(_troop);
        // Expose troop identity
        if (!_troop.isExposed)
        {
            _troop.isExposed = true;
            _troop.troopModel.SetActive(true);
            _troop.healthTextObject.SetActive(true);
            if (_troop.blurredTroopModel.activeSelf)
                _troop.blurredTroopModel.SetActive(false);
        }
        troopInfo.canAttack = false;

        // Check if attacking troop back
        if (_attackedFromTheBack)
            _troop.health -= troopInfo.stealthAttack - _troop.baseDefense;
        else
        {
            _troop.health -= troopInfo.baseAttack - _troop.facingDefense;
        }
        // this troop killed the other troop
        if (_troop.health <= 0)
        {
            _tile.isOccupied = false;
            GameManagerCS.instance.troops.Remove(_troop.id);
            GameManagerCS.instance.objectsToDestroy.Add(_troop.troop);
            _troop.troopActions.DieAnim();

            if (troopInfo.health > 0 && troopInfo.canMoveAfterKill)
            {
                if (_tile.isCity)
                    MoveOntoCity(_tile, GameManagerCS.instance.cities[_tile.cityId]);
                else
                    MoveToNewTile(_tile);
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
                // If troop is ranged then they are immune to counter attack
                if (troopInfo.attackRange == 1)
                    troopInfo.health -= _troop.counterAttack - troopInfo.baseDefense;
            }
            // Play hurt animation
            _troop.troopActions.HurtAnim();
        }

        // This troop was killed
        if (troopInfo.health <= 0)
        {
            _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
            _tile.isOccupied = false;
            DieAnim();
            if (troopInfo.troopName == "King")
                GameManagerCS.instance.KingIsDead();
            GameManagerCS.instance.troops.Remove(troopInfo.id);
            GameManagerCS.instance.objectsToDestroy.Add(troopInfo.troop);
        }
        else if(!_attackedFromTheBack && _troop.health > 0)
        {
            // Ranged troops are immune to counter attacks so only show counter attack info if the troop is not ranged
            if(troopInfo.attackRange == 1)
            {
                _troop.troopActions.AttackAnim(troopInfo);
                HurtAnim();
            }
        }
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
                        if (_troop.ownerId != troopInfo.ownerId)
                        {
                            // Enemy troop within seeing range
                            if (_troop.isExposed)
                            {
                                _troop.troopModel.SetActive(true);
                                _troop.healthTextObject.SetActive(true);
                                if (_troop.blurredTroopModel.activeSelf)
                                    _troop.blurredTroopModel.SetActive(false);
                            }
                            else
                                _troop.blurredTroopModel.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Troop Animations

    /// <summary>
    /// Sets up attack animations and adds attack information to modified troop dict to send to server
    /// </summary>
    /// <param name="_troopToAttack"></param>
    public void AttackAnim(TroopInfo _troopToAttack)
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

        int _distance = Mathf.Abs(_troopToAttack.xIndex - troopInfo.xIndex) + Mathf.Abs(_troopToAttack.zIndex - troopInfo.zIndex);
        if (_distance == 1)
        {
            PlayerCS.instance.animationQueue.Enqueue(SwordAnim(_attackDirection));
        }
        else
            PlayerCS.instance.animationQueue.Enqueue(ArrowAnim(_troopToAttack, _attackDirection));

        troopInfo.attackRotation = _attackDirection;
        troopInfo.lastTroopAttackedId = _troopToAttack.id;
        TroopInfo _copiedTroop = GameManagerCS.instance.dataStoringObject.AddComponent<TroopInfo>();
        _copiedTroop.CopyNecessaryTroopInfoToSendToServer(troopInfo);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_copiedTroop, "Attack"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
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
        int errorCatcher = 0;
        while (GameManagerCS.instance.sword.transform.localEulerAngles.x < .1f || GameManagerCS.instance.sword.transform.localEulerAngles.x > 10.1f)
        {
            if (errorCatcher > 10000)
                break;
            GameManagerCS.instance.sword.transform.localRotation *= Quaternion.Euler(10, 0, 0);
            errorCatcher++;
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
    public void HurtAnim()
    {
        TroopInfo _copiedTroop = GameManagerCS.instance.dataStoringObject.AddComponent<TroopInfo>();
        _copiedTroop.CopyNecessaryTroopInfoToSendToServer(troopInfo);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_copiedTroop, "Hurt"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        PlayerCS.instance.animationQueue.Enqueue(HurtAnimHelper());
    }

    /// <summary>
    /// Rotate troop 360 degrees on the x axis by rotating 20 degree every .01 seconds of scaled time
    /// </summary>
    public IEnumerator HurtAnimHelper()
    {
        PlayerCS.instance.isAnimInProgress = true;
        troopInfo.healthText.text = troopInfo.health.ToString();
        for (int i = 0; i < 18; i++)
        {
            yield return new WaitForSeconds(.01f);
            troopInfo.troopModel.transform.localRotation *= Quaternion.Euler(0, 20, 0);
        }
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
        troopInfo.troopModel.transform.rotation = Quaternion.Euler(0, troopInfo.rotation, 0);
    }

    /// <summary>
    /// Call when troop dies. Resets altered tiles, hides quick menu, and places die animation in queue
    /// </summary>
    public void DieAnim()
    {
        TroopInfo _copiedTroop = GameManagerCS.instance.dataStoringObject.AddComponent<TroopInfo>();
        _copiedTroop.CopyNecessaryTroopInfoToSendToServer(troopInfo);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_copiedTroop, "Die"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        troopInfo.healthTextObject.SetActive(false);
        // Reset altered tiles and hide troop quick menu if this is a local rather than remote troop
        if(troopInfo.ownerId == ClientCS.instance.myId)
        {
            ResetAlteredTiles();
            HideQuickMenu();
        }
        PlayerCS.instance.animationQueue.Enqueue(DieAnimHelper());
    }

    /// <summary>
    /// Rotates troop on the x and z axis while decreasing the troops y position until it is out of view.
    /// When the troop is out of view it is disabled.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DieAnimHelper()
    {
        PlayerCS.instance.isAnimInProgress = true;
        while (troopInfo.troopModel.transform.localPosition.y > -1.5f)
        {
            troopInfo.troopModel.transform.localRotation *= Quaternion.Euler(5, 0, 5);
            troopInfo.troopModel.transform.localPosition = new Vector3(troopInfo.troopModel.transform.localPosition.x,
                                                                       troopInfo.troopModel.transform.localPosition.y - .05f,
                                                                       troopInfo.troopModel.transform.localPosition.z);
            yield return new WaitForSeconds(.001f);
        }
        troopInfo.troop.SetActive(false);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
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
        if (troopInfo.troopName == "King")
            mainKingContainer.SetActive(true);
        else
            mainContainer.SetActive(true);

        DisplayerPossibleActions();
    }

    public void DisplayerPossibleActions()
    {
        int _currentXCoord = -700, _xCoordIncrement = 500, _currentYCoord = -400;

        // Deactivate all action buttons beside close 
        createCityButton.gameObject.SetActive(false);
        conquerKingCityButton.gameObject.SetActive(false);
        conquerMainCityButton.gameObject.SetActive(false);

        // Check if troop can conquer a city
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xIndex, troopInfo.zIndex];
        if(_tile.isCity )
        {
            CityInfo _city = GameManagerCS.instance.cities[_tile.cityId];
            if(_city.isAbleToBeConquered)
            {
                if (troopInfo.troopName == "King")
                {
                    conquerKingCityButton.gameObject.SetActive(true);
                    conquerKingCityButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
                    _currentXCoord += _xCoordIncrement;
                }
                else
                {
                    conquerMainCityButton.gameObject.SetActive(true);
                    conquerMainCityButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
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

        // If no actions possible then close quick menu
        if (!conquerMainCityButton.gameObject.activeInHierarchy && !conquerKingCityButton.gameObject.activeInHierarchy && 
            !createCityButton.gameObject.activeInHierarchy)
        {
            ResetQuickMenu();
        }
    }

    public void ResetQuickMenu()
    {
        quickMenuContainer.SetActive(false);
        mainContainer.SetActive(false);
        mainKingContainer.SetActive(false);
        PlayerCS.instance.isAbleToCommitActions = true;
        PlayerCS.instance.playerUI.menuButton.SetActive(false);
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