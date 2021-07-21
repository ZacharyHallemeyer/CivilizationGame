﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCS : MonoBehaviour
{
    public static GameManagerCS instance;

    public int currentTroopIndex = 0;
    public Dictionary<int, TroopInfo> troops = new Dictionary<int, TroopInfo>();
    public Dictionary<int, CityInfo> cities = new Dictionary<int, CityInfo>();
    public TileInfo[,] tiles;

    public GameObject playerPrefab;
    public GameObject troopPrefab;
    public GameObject desertTilePrefab, forestTilePrefab, grasslandTilePrefab, rainForestTilePrefab, swampTilePrefab,
                      tundraTilePrefab, waterTilePrefab;

    public string[] troopNames;
    public string[] biomeOptions;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        int _index = 0;
        biomeOptions = new string[Constants.biomeInfo.Count];
        foreach (string _biomeName in Constants.biomeInfo.Keys)
        {
            biomeOptions[_index] = _biomeName;
            _index++;
        }
    }

    public void SpawnPlayer(int _id, string _username)
    {
        GameObject _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        _player.GetComponent<PlayerCS>().InitPlayer(_id, _username);
    }

    public void CreateNewTile(int _id, int _ownerId, int _movementCost, int _occupyingObjectId, string _biome,
                              float _temp, float _height, bool _isWater, bool _isRoad, bool _isCity, bool _isOccupied,
                              Vector2 _position, int _xIndex, int _zIndex, string _name)
    {
        GameObject _tile = null;
        if(_isWater)
        {
            _tile = Instantiate(waterTilePrefab, new Vector3(_position.x, 0, _position.y), Quaternion.identity);
        }
        else
        {
            switch (_biome)
            {
                case "Desert":
                    _tile = Instantiate(desertTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                case "Forest":
                    _tile = Instantiate(forestTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                case "Grassland":
                    _tile = Instantiate(grasslandTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                case "RainForest":
                    _tile = Instantiate(rainForestTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                case "Swamp":
                    _tile = Instantiate(swampTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                case "Tundra":
                    _tile = Instantiate(tundraTilePrefab, new Vector3(_position.x, 0, _position.y),
                                             Quaternion.identity);
                    break;
                default:
                    Debug.LogError("Could not find Biome");
                    break;
            }
        }
        TileInfo _tileInfo = _tile.AddComponent<TileInfo>();
        _tile.name = _name;
        _tileInfo.tile = _tile;
        _tileInfo.id = _id;
        _tileInfo.ownerId = _ownerId;
        _tileInfo.movementCost = _movementCost;
        _tileInfo.occupyingObjectId = _occupyingObjectId;
        _tileInfo.biome = _biome;
        _tileInfo.temperature = _temp;
        _tileInfo.height = _height;
        _tileInfo.isWater = _isWater;
        _tileInfo.isRoad = _isRoad;
        _tileInfo.isCity = _isCity;
        _tileInfo.isOccupied = _isOccupied;
        _tileInfo.position = _position;

        tiles[_xIndex, _zIndex] = _tileInfo;
    }

    public void InstantiateTroop(int _ownerId, int _id, string _troopName,int _xCoord, int _zCoord, int _rotation)
    {
        GameObject _troop = Instantiate(troopPrefab, new Vector3(_xCoord, 1, _zCoord), Quaternion.identity);
        TroopActionsCS _troopActions = new TroopActionsCS();
        _troop.AddComponent<TroopInfo>().InitTroopInfo(_troopName, _troop, _troopActions, _id, _ownerId, _xCoord, _zCoord);
       
        troops.Add(currentTroopIndex, _troop.GetComponent<TroopInfo>());
        currentTroopIndex++;
    }

    public void AddCityResources()
    {
        foreach(CityInfo _city in cities.Values)
        {
            if(_city.ownerId == ClientCS.instance.myId)
            {
                PlayerCS.instance.wood += _city.woodResourcesPerTurn;
                PlayerCS.instance.metal += _city.metalResourcesPerTurn;
                PlayerCS.instance.food += _city.foodResourcesPerTurn;
            }
        }
    }
}
