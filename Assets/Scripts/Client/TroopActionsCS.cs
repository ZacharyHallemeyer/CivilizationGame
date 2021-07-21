using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopActionsCS : MonoBehaviour
{
    public GameObject[] objecstToBeReset;
    public int whatIsInteractableValue, whatIsDefaultValue;
    public string moveableTileTag = "MoveableTile", defaultTileTag = "Tile";
    public TroopInfo troopInfo;

    public void InitTroopActions(TroopInfo _troopInfo)
    {
        whatIsInteractableValue = LayerMask.NameToLayer("Interactable");
        whatIsDefaultValue = LayerMask.NameToLayer("Default");
        troopInfo = _troopInfo;
    }

    public void CreateInteractableTiles()
    {
        int _index = 0;
        objecstToBeReset = new GameObject[troopInfo.movementCost];
        // Rotation troop is facing
        int _rotation = (int)troopInfo.rotation;

        // Add/Minus 1 to for loop conditions to not include tile troop is currently on
        switch (_rotation)
        {
            case 0:
                for (int z = troopInfo.zCoord + 1; z < troopInfo.zCoord + troopInfo.movementCost + 1; z++)
                {
                    TileInfo _tile = WorldGeneratorSS.tiles[troopInfo.xCoord, z];
                    if (CheckTileExists(troopInfo.xCoord, z))
                    {
                        if (_tile.isOccupied == true)
                        {
                            if (_tile.occupyingObjectId != troopInfo.ownerId)
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = moveableTileTag;
                                objecstToBeReset[_index] = _tile.tile;
                                _index++;
                            }
                        }
                        else
                        {
                            _tile.tile.layer = whatIsInteractableValue;
                            _tile.tile.tag = moveableTileTag;
                            objecstToBeReset[_index] = _tile.tile;
                            _index++;
                        }
                    }
                }
                break;
            case 90:
                for (int x = troopInfo.xCoord + 1; x < troopInfo.xCoord + troopInfo.movementCost + 1; x++)
                {
                    TileInfo _tile = WorldGeneratorSS.tiles[x, troopInfo.zCoord];
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        _tile.tile.layer = whatIsInteractableValue;
                        _tile.tile.tag = moveableTileTag;
                        objecstToBeReset[_index] = _tile.tile;
                        _index++;
                    }
                }
                break;
            case 180:
                for (int z = troopInfo.zCoord - 1; z > troopInfo.zCoord - troopInfo.movementCost - 1; z--)
                {
                    TileInfo _tile = WorldGeneratorSS.tiles[troopInfo.xCoord, z];
                    if (CheckTileExists(troopInfo.xCoord, z))
                    {
                        _tile.tile.layer = whatIsInteractableValue;
                        _tile.tile.tag = moveableTileTag;
                        objecstToBeReset[_index] = _tile.tile;
                        _index++;
                    }
                }
                break;
            case 270:
                for (int x = troopInfo.xCoord - 1; x > troopInfo.xCoord - troopInfo.movementCost - 1; x--)
                {
                    TileInfo _tile = WorldGeneratorSS.tiles[x, troopInfo.zCoord];
                    if (CheckTileExists(x, troopInfo.zCoord))
                    {
                        _tile.tile.layer = whatIsInteractableValue;
                        _tile.tile.tag = moveableTileTag;
                        objecstToBeReset[_index] = _tile.tile;
                        _index++;
                    }
                }
                break;

            default:
                Debug.LogError("Troop " + troopInfo.id + " rotation is not compatible");
                break;
        }
    }

    private bool CheckTileExists(int _x, int _z)
    {
        return (_x >= 0 && _x < WorldGeneratorSS.tiles.GetLength(0)
                && _z >= 0 && _z < WorldGeneratorSS.tiles.GetLength(1));
    }

    public void MoveToNewTile(TileInfo _newTile)
    {
        WorldGeneratorSS.tiles[troopInfo.xCoord, troopInfo.zCoord].isOccupied = false;
        troopInfo.xCoord = (int)_newTile.tile.transform.position.x;
        troopInfo.zCoord = (int)_newTile.tile.transform.position.z;
        troopInfo.troop.transform.position = new Vector3(troopInfo.xCoord, troopInfo.troop.transform.position.y,
                                                         troopInfo.zCoord);
        _newTile.isOccupied = true;
        ResetAlteredTiles();
    }

    /// <summary>
    /// pass in -1 for troop to rotate counter clockwise and 1 to rotate clockwise
    /// </summary>
    public void Rotate(int _rotateValue)
    {
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
        ResetAlteredTiles();
        CreateInteractableTiles();
    }

    public void ResetAlteredTiles()
    {
        if (objecstToBeReset == null) return;
        foreach (GameObject _tile in objecstToBeReset)
        {
            if (_tile != null)
            {
                _tile.layer = whatIsDefaultValue;
                _tile.tag = defaultTileTag;
            }
        }
        objecstToBeReset = null;
    }

    public void Attack(TileInfo _tile)
    {
        TroopInfo _troop = GameManagerCS.instance.troops[_tile.occupyingObjectId];
        // Check if attacking troop back
        if (_troop.rotation + 180 == troopInfo.rotation || _troop.rotation - 180 == troopInfo.rotation)
        {
            _troop.health -= troopInfo.stealthAttack - _troop.baseDefense;
        }
        else
            _troop.health -= troopInfo.baseAttack - _troop.facingDefense;
    }
}
