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
                for (int z = troopInfo.zCoord + 1; z < troopInfo.zCoord + troopInfo.movementCost + 1; z++)
                {
                    if (CheckTileExists(troopInfo.xCoord, z)) 
                    {
                        if (!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xCoord + 1; x < troopInfo.xCoord + troopInfo.movementCost + 1; x++)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        if(!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zCoord - 1; z > troopInfo.zCoord - troopInfo.movementCost - 1; z--)
                {
                    if (CheckTileExists(troopInfo.xCoord, z))
                    {
                        if(!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index))
                            break;
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xCoord - 1; x > troopInfo.xCoord - troopInfo.movementCost - 1; x--)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        if(!CreateInteractableTilesHelperMovement(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index))
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
        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                for (int z = troopInfo.zCoord + 1; z < troopInfo.zCoord + troopInfo.attackRange + 1; z++)
                {
                    if (CheckTileExists(troopInfo.xCoord, z)) 
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index);
                        _index++;
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xCoord + 1; x < troopInfo.xCoord + troopInfo.attackRange + 1; x++)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index);
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zCoord - 1; z > troopInfo.zCoord - troopInfo.attackRange - 1; z--)
                {
                    if (CheckTileExists(troopInfo.xCoord, z))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index);
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xCoord - 1; x > troopInfo.xCoord - troopInfo.attackRange - 1; x--)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        CreateInteractableTilesHelperAttack(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index);
                        _index++;
                    }
                }
                break;

            default:
                Debug.LogError("Troop " + troopInfo.id + " rotation is not compatible");
                break;
        }

        // Check if can standing on and can conquer acity
        TileInfo _tileInfo = GameManagerCS.instance.tiles[troopInfo.xCoord, troopInfo.zCoord];
        if(_tileInfo.isCity)
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
        if (_tile.isWater || _tile.isOccupied) return false;
        if (_tile.isCity)
        {
            if (GameManagerCS.instance.cities[_tile.cityId].ownerId != ClientCS.instance.myId)     // Client does NOT owns city
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = moveableCityTag;
                _tile.moveUI.SetActive(true);
            }
            else
            {
                // Prevent troops from moving onto a city that is training troops 
                if (GameManagerCS.instance.cities[_tile.cityId].isTrainingTroops) return false;
            }
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
        if (_tile.isWater) return;
        if (_tile.isOccupied)
        {
            if (GameManagerCS.instance.troops[_tile.occupyingObjectId].ownerId != troopInfo.ownerId)
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = attackableTileTag;
                _tile.moveUI.SetActive(true);
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
                }
                else
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = defaultTileTag;
                    _tile.moveUI.SetActive(false);
                }
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
        HideQuickMenu();
        // Update old tile
        TileInfo _oldTile = GameManagerCS.instance.tiles[troopInfo.xCoord, troopInfo.zCoord];
        _oldTile.isOccupied = false;
        _oldTile.occupyingObjectId = -1;
        if(_oldTile.isCity)
        {
            CityInfo _city = GameManagerCS.instance.cities[_oldTile.cityId];
            _city.isBeingConquered = false;
            _city.isAbleToBeConquered = false;
        }

        // Move troop
        troopInfo.xCoord = (int)_newTile.position.x;
        troopInfo.zCoord = (int)_newTile.position.y;
        troopInfo.troop.transform.position = new Vector3(troopInfo.xCoord, troopInfo.troop.transform.position.y,
                                                         troopInfo.zCoord);

        // Update new tile
        troopInfo.movementCost -= Mathf.Abs(_newTile.xIndex - _oldTile.xIndex) + Mathf.Abs(_newTile.yIndex - _oldTile.yIndex);
        if (troopInfo.movementCost < 0) troopInfo.movementCost = 0;
        _newTile.isOccupied = true;
        _newTile.occupyingObjectId = troopInfo.id;
        // Add Troopdata to send to server
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {troopInfo, "Move"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        // Add tile data to send to server
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_oldTile, "Update"} };
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
        _tileData = new Dictionary<TileInfo, string>()
            { {_newTile, "Update"} };
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
        ResetAlteredTiles();
        CreateInteractableTiles();
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
        troopInfo.troop.transform.localRotation = Quaternion.Euler(0, troopInfo.rotation, 0);
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {troopInfo, "Rotate"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        ResetAlteredTiles();
        CreateInteractableTiles();
    }

    #endregion

    #region Actions

    public void ConquerCity()
    {
        HideQuickMenu();
        TileInfo _tile = GameManagerCS.instance.tiles[troopInfo.xCoord, troopInfo.zCoord];
        CityInfo _city = GameManagerCS.instance.cities[_tile.cityId];
        ConquerCity(_tile, _city);
    }

    public void ConquerCity(TileInfo _tile, CityInfo _city)
    {
        _tile.tile.tag = cityTag;
        troopInfo.movementCost = 0;
        troopInfo.canAttack = false;
        _city.isAbleToBeConquered = false;
        _city.isBeingConquered = false;

        _city.ownerId = troopInfo.ownerId;
        _city.InitConqueredCity();
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { _city, "Conquer" } };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
        ResetAlteredTiles();
    }

    /// <summary>
    /// Attack troop on tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_tile"></param>
    public void Attack(TileInfo _tile)
    {
        HideQuickMenu();
        TroopInfo _troop = GameManagerCS.instance.troops[_tile.occupyingObjectId];
        bool _attackedFromTheBack = _troop.rotation + 180 == troopInfo.rotation || _troop.rotation - 180 == troopInfo.rotation;
        // Check if attacking troop back
        if (_attackedFromTheBack)
            _troop.health -= troopInfo.stealthAttack - _troop.baseDefense;
        else
        {
            _troop.health -= troopInfo.baseAttack - _troop.facingDefense;
            troopInfo.health -= _troop.counterAttack - troopInfo.baseDefense;
        }
        troopInfo.lastTroopAttackedId = _troop.id;
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {troopInfo, "Attack"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, "Hurt"} };
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);

        if(_attackedFromTheBack)
        {
            _troopData = new Dictionary<TroopInfo, string>()
            { {troopInfo, "Hurt"} };
            GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
        }

        // troop killed the other troop
        if(_troop.health <= 0)
        {
            _tile.isOccupied = false;
            _troop.troop.SetActive(false);
            GameManagerCS.instance.troops.Remove(_troop.id);
            GameManagerCS.instance.objectsToDestroy.Add(_troop.troop);
            _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, "Die"} };
            GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);

            if (_tile.isCity)
                MoveOntoCity(_tile, GameManagerCS.instance.cities[_tile.cityId]);
            else
                MoveToNewTile(_tile);
        }
    }

    public void Die()
    {

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
        ResetQuickMenu();
    }

    public void ShowQuickMenu()
    {
        quickMenuContainer.SetActive(true);
        if (troopInfo.troopName == "King")
        {
            mainKingContainer.SetActive(true);
            if (!CanCreateCity())
                createCityButton.enabled = false;
        }
        else
            mainContainer.SetActive(true);
    }

    public void ResetQuickMenu()
    {
        quickMenuContainer.SetActive(false);
        mainContainer.SetActive(false);
        mainKingContainer.SetActive(false);
        createCityButton.enabled = true;
    }

    #endregion

    #region Troop Animations

    public void AttackAnim()
    {

    }

    public void MoveAnim()
    {

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
        for (int x = troopInfo.xCoord - GameManagerCS.instance.maxDistanceFromResource;
            x <= troopInfo.xCoord + GameManagerCS.instance.maxDistanceFromResource;
            x++)
        {
            for (int z = troopInfo.zCoord - GameManagerCS.instance.maxDistanceFromResource;
                z <= troopInfo.zCoord + GameManagerCS.instance.maxDistanceFromResource;
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
        for (int x = troopInfo.xCoord - GameManagerCS.instance.minDistanceBetweenCities;
            x <= troopInfo.xCoord + GameManagerCS.instance.minDistanceBetweenCities;
            x++)
        {
            for (int z = troopInfo.zCoord - GameManagerCS.instance.minDistanceBetweenCities;
                z <= troopInfo.zCoord + GameManagerCS.instance.minDistanceBetweenCities;
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
        for (int x = troopInfo.xCoord - GameManagerCS.instance.maxDistanceBetweenCities;
            x <= troopInfo.xCoord + GameManagerCS.instance.maxDistanceBetweenCities;
            x++)
        {
            for (int z = troopInfo.zCoord - GameManagerCS.instance.maxDistanceBetweenCities;
                z <= troopInfo.zCoord + GameManagerCS.instance.maxDistanceBetweenCities;
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

        GameManagerCS.instance.SpawnCity(troopInfo.xCoord, troopInfo.zCoord);
    }

    #endregion

}