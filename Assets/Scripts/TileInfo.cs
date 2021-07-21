using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public GameObject tile;
    public int id;
    public int ownerId;
    public int movementCost;
    public string biome;
    public float temperature;       // Range -1 to 1
    public float height;            // Range -1 to 1
    public bool isWater = false;
    public bool isRoad = false;
    public bool isCity = false;
    public bool isOccupied = false;
    public int occupyingObjectId;
    public Vector2 position;

    public void InitTileInfo(GameObject _tile, string _biomeName, int _id, int _ownerId)
    {
        tile = _tile;
        id = _id;
        biome = _biomeName;
        temperature = Constants.biomeInfo[_biomeName]["Temperature"];
        height = Constants.biomeInfo[_biomeName]["Height"];
        movementCost = (int)Constants.biomeInfo[_biomeName]["MovementCost"];
        position = new Vector2((int)_tile.transform.position.x, (int)_tile.transform.position.z);
    }

    public void InitWaterInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        isWater = true;
        biome = "Water";
        position = new Vector2((int)_tile.transform.position.x, (int)_tile.transform.position.z);
    }
}
