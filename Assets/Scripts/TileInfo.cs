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

    public void FillDesertInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 1;
        biome = "Desert";
        temperature = .75f;
        height = 0f;
    }

    public void FillForestInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 1;
        biome = "Forest";
        temperature = -.25f;
        height = 0f;
    }

    public void FillGrasslandInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 1;
        biome = "Grassland";
        temperature = .1f;
        height = 0f;
    }

    public void FillRainForestInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 1;
        biome = "RainForest";
        temperature = .25f;
        height = 0f;
    }

    public void FillSwampInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 2;
        biome = "Swamp";
        temperature = .25f;
        height = 0f;
    }

    public void FillTundraInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        movementCost = 1;
        biome = "Tundra";
        temperature = -.75f;
        height = 0f;
    }

    public void FillWaterInfo(GameObject _tile, int _id)
    {
        tile = _tile;
        id = _id;
        isWater = true;
    }
}
