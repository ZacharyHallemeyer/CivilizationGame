using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopActionsCS : MonoBehaviour
{
    public TileInfo[] objecstToBeReset;
    public int whatIsInteractableValue, whatIsDefaultValue;
    private string moveableTileTag = "MoveableTile", attackableTileTag = "AttackableTile", defaultTileTag = "Tile";
    private string conquerableCityTag = "ConquerableCity", cityTag = "City", moveableCityTag = "MoveableCity";
    public TroopInfo troopInfo;

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

    /// <summary>
    /// Sets tiles to iteractable if troop can reach them
    /// </summary>
    public void CreateInteractableTiles()
    {
        if (troopInfo.movementCost <= 0 && troopInfo.canAttack == false) return;
        int _index = 0;
        objecstToBeReset = new TileInfo[troopInfo.movementCost];

        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (troopInfo.rotation)
        {
            case 0:
                for (int z = troopInfo.zCoord + 1; z < troopInfo.zCoord + troopInfo.movementCost + 1; z++)
                {
                    if (CheckTileExists(troopInfo.xCoord, z)) 
                    {
                        CreateInteractableTilesHelper(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index);
                        _index++;
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xCoord + 1; x < troopInfo.xCoord + troopInfo.movementCost + 1; x++)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        CreateInteractableTilesHelper(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index);
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zCoord - 1; z > troopInfo.zCoord - troopInfo.movementCost - 1; z--)
                {
                    if (CheckTileExists(troopInfo.xCoord, z))
                    {
                        CreateInteractableTilesHelper(GameManagerCS.instance.tiles[troopInfo.xCoord, z], _index);
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xCoord - 1; x > troopInfo.xCoord - troopInfo.movementCost - 1; x--)
                {
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        CreateInteractableTilesHelper(GameManagerCS.instance.tiles[x, troopInfo.zCoord], _index);
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

    public void CreateInteractableTilesHelper(TileInfo _tile, int _index)
    {
        if (_tile.isWater) return;
        if(_tile.isCity)
        {
            Debug.Log(_tile.cityId);
            Debug.Log(GameManagerCS.instance.cities[_tile.cityId]);
            Debug.Log(GameManagerCS.instance.cities[_tile.cityId].ownerId);
            if (GameManagerCS.instance.cities[_tile.cityId].ownerId != ClientCS.instance.myId)     // Client does NOT owns city
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = moveableCityTag;
                _tile.moveUI.SetActive(true);
            }
        }
        else if (_tile.isOccupied != true)
        {
            if (_tile.occupyingObjectId != troopInfo.ownerId)
            {
                _tile.tile.layer = whatIsInteractableValue;
                _tile.tile.tag = moveableTileTag;
                _tile.moveUI.SetActive(true);
            }
        }
        else
        {
            _tile.tile.layer = whatIsInteractableValue;
            _tile.tile.tag = attackableTileTag;
            _tile.moveUI.SetActive(true);
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
    /// Move troop to new tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_newTile"></param>
    public void MoveToNewTile(TileInfo _newTile)
    {
        // Update old tile
        TileInfo _oldTile = GameManagerCS.instance.tiles[troopInfo.xCoord, troopInfo.zCoord];
        _oldTile.isOccupied = false;
        _oldTile.occupyingObjectId = -1;

        // Move troop
        troopInfo.xCoord = (int)_newTile.position.x;
        troopInfo.zCoord = (int)_newTile.position.y;
        troopInfo.troop.transform.position = new Vector3(troopInfo.xCoord, troopInfo.troop.transform.position.y,
                                                         troopInfo.zCoord);

        // Update new tile
        troopInfo.movementCost -= Mathf.Abs(_newTile.xIndex - _oldTile.xIndex) + Mathf.Abs(_newTile.yIndex - _oldTile.yIndex);
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
                if(_tile.isCity)
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
    }

    /// <summary>
    /// Attack troop on tile and update modified troop and tile dicts.
    /// </summary>
    /// <param name="_tile"></param>
    public void Attack(TileInfo _tile)
    {
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
    }

    #region Troop Animations

    public void AttackAnim()
    {

    }

    public void MoveAnim()
    {

    }

    #endregion

}
