﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public GameObject tile;
    public GameObject moveUI;
    public int id;
    public int ownerId;
    public int movementCost;
    public string biome;
    public float temperature;       // Range -1 to 1
    public float height;            // Range -1 to 1
    public bool isWater = false;
    public bool isFood = false;
    public bool isWood = false;
    public bool isMetal = false;
    public bool isRoad = false;
    public bool isCity = false;
    public bool isOccupied = false;
    public int occupyingObjectId = -1;
    public int cityId = -1;
    public int xIndex;
    public int yIndex;
    public Vector2 position;
    public int idOfPlayerThatSentInfo;

    public GameObject resourceObject;
    public GameObject ownerShipVisualObject;

    public void InitTileInfo(GameObject _tile, string _biomeName, int _id, int _ownerId, int _xIndex, int _yIndex)
    {
        tile = _tile;
        id = _id;
        biome = _biomeName;
        temperature = Constants.biomeInfo[_biomeName]["Temperature"];
        height = Constants.biomeInfo[_biomeName]["Height"];
        movementCost = (int)Constants.biomeInfo[_biomeName]["MovementCost"];
        position = new Vector2((int)_tile.transform.position.x, (int)_tile.transform.position.z);
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void InitWaterInfo(GameObject _tile, int _id, int _xIndex, int _yIndex)
    {
        tile = _tile;
        id = _id;
        isWater = true;
        biome = "Water";
        position = new Vector2((int)_tile.transform.position.x, (int)_tile.transform.position.z);
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void UpdateTileInfo(TileInfo _tileToCopy)
    {
        id = _tileToCopy.id;
        ownerId = _tileToCopy.ownerId;
        isRoad = _tileToCopy.isRoad;
        isCity = _tileToCopy.isCity;
        isOccupied = _tileToCopy.isOccupied;
        occupyingObjectId = _tileToCopy.occupyingObjectId;
        xIndex = _tileToCopy.xIndex;
        yIndex = _tileToCopy.yIndex;
        cityId = _tileToCopy.cityId;
    }

    public void CopyTileInfo(TileInfo _tileToCopy)
    {
        id = _tileToCopy.id;
        ownerId = _tileToCopy.ownerId;
        biome = _tileToCopy.biome;
        temperature = _tileToCopy.temperature;
        height = _tileToCopy.height;
        movementCost = _tileToCopy.movementCost;
        position = _tileToCopy.position;
        isWater = _tileToCopy.isWater;
        isFood = _tileToCopy.isFood;
        isWood = _tileToCopy.isWood;
        isMetal = _tileToCopy.isMetal;
        isRoad = _tileToCopy.isRoad;
        isCity = _tileToCopy.isCity;
        isOccupied = _tileToCopy.isOccupied;
        occupyingObjectId = _tileToCopy.occupyingObjectId;
        xIndex = _tileToCopy.xIndex;
        yIndex = _tileToCopy.yIndex;
        cityId = _tileToCopy.cityId;
    }
}
