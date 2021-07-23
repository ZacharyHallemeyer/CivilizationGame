﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCS : MonoBehaviour
{
    public static GameManagerCS instance;

    public int starCount = 100;

    public int currentTroopIndex = 0;
    public Dictionary<int, TroopInfo> troops = new Dictionary<int, TroopInfo>();
    public Dictionary<int, CityInfo> cities = new Dictionary<int, CityInfo>();
    public TileInfo[,] tiles;

    public bool isAllTroopInfoReceived = false, isAllTileInfoReceived = false, isAllCityInfoReceived = false;
    public List<Dictionary<TroopInfo, string>> modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
    public List<Dictionary<TileInfo, string>> modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
    public List<Dictionary<CityInfo, string>> modifiedCityInfo = new List<Dictionary<CityInfo, string>>();

    public GameObject playerPrefab;
    public GameObject troopPrefab;
    public GameObject starPrefab; 
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
        CreateStars();
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
        _tile.transform.parent = transform;
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

    public void InstantiateTroop(int _ownerId, string _troopName,int _xCoord, int _zCoord, int _rotation)
    {
        GameObject _troop = Instantiate(troopPrefab, new Vector3(_xCoord, 1, _zCoord), Quaternion.identity);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        _troop.AddComponent<TroopInfo>().InitTroopInfo(_troopName, _troop, _troopActions, currentTroopIndex, _ownerId, _xCoord, _zCoord);
       
        troops.Add(currentTroopIndex, _troop.GetComponent<TroopInfo>());
        currentTroopIndex++;
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop.GetComponent<TroopInfo>(), "Spawn"} };
        modifiedTroopInfo.Add(_troopData);
    }

    public void InstantiateTroop(TroopInfo _troopInfoToCopy)
    {
        GameObject _troop = Instantiate(troopPrefab, new Vector3(_troopInfoToCopy.xCoord, 1, _troopInfoToCopy.zCoord),
                                        Quaternion.identity);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        TroopInfo _troopInfo = _troop.AddComponent<TroopInfo>();
        _troopInfo.InitTroopInfo(_troopInfoToCopy, _troop, _troopActions);

        troops.Add(_troopInfo.id, _troopInfo);
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

    public void CreateStars()
    {
        for(int i = 0; i < starCount; i++)
        {
            MeshRenderer _starMesh = Instantiate(starPrefab, RandomStarPosition(), Quaternion.identity).GetComponent<MeshRenderer>();
        }
    }

    public Vector3 RandomStarPosition()
    {
        Vector3 _position = Vector3.zero;
        int _lengthX = tiles.GetLength(0) * 2;
        int _lengthZ = tiles.GetLength(1) * 2;
        // Random side to spawn
        switch (Random.Range(0, 7))
        {
            case 0:
                _position = new Vector3(Random.Range(-_lengthX, _lengthX), -60, Random.Range(-_lengthZ, _lengthZ));
                break;
            case 1:
                _position = new Vector3(Random.Range(-_lengthX, _lengthX), -60, Random.Range(-_lengthZ, _lengthZ));
                break;
            case 2:
                _position = new Vector3(-_lengthX - _lengthX, Random.Range(-_lengthX, _lengthX), Random.Range(-_lengthZ, _lengthZ));
                break;
            case 3:
                _position = new Vector3(_lengthX + _lengthX, Random.Range(-_lengthX, _lengthX), Random.Range(-_lengthZ, _lengthZ));
                break;
            case 4:
                _position = new Vector3(Random.Range(-_lengthX, _lengthX), Random.Range(-_lengthZ, _lengthZ), -_lengthZ - _lengthZ);
                break;
            case 5:
                _position = new Vector3(Random.Range(-_lengthX, _lengthX), Random.Range(-_lengthZ, _lengthZ), _lengthZ + _lengthZ);
                break;
            default:
                break;
        }

        return _position;
    }

    public void WaitAndStartTurn(Packet _packet)
    {
        StartCoroutine(WaitAndCallStartTurn(_packet));
    }

    private IEnumerator WaitAndCallStartTurn(Packet _packet)
    {
        yield return new WaitForSeconds(.1f);
        ClientHandle.PlayerStartTurn(_packet);
    }

    public void ClearModifiedData()
    {
        /*
        foreach(Dictionary<TroopInfo, string> _troopDict in modifiedTroopInfo)
        {
            foreach (TroopInfo _troop in _troopDict.Keys)
                Destroy(_troop);
        }
        */
        modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
        foreach(Dictionary<TileInfo, string> _tileDict in modifiedTileInfo)
        {
            foreach (TileInfo _tile in _tileDict.Keys)
                Destroy(_tile);
        }
        modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
        foreach(Dictionary<CityInfo, string> _cityDict in modifiedCityInfo)
        {
            foreach (CityInfo _city in _cityDict.Keys)
                Destroy(_city);
        }
        modifiedCityInfo = new List<Dictionary<CityInfo, string>>();
    }

    public void PlayPastMoves()
    {
        // Update/Show Other Player troop movements/actions
        foreach (Dictionary<TroopInfo, string> _troopDict in modifiedTroopInfo)
        {
            foreach (TroopInfo _troop in _troopDict.Keys)
            {
                switch (_troopDict[_troop])
                {
                    case "Spawn":
                        InstantiateTroop(_troop);
                        break;
                    case "Move":

                        break;
                    case "Rotate":

                        break;
                    case "Attack":

                        break;
                    case "Hurt":

                        break;
                    case "Die":

                        break;
                    default:
                        Debug.LogError("Could not find troop action: " + _troopDict[_troop]);
                        break;
                }

            }
        }
        ClearModifiedData();

        PlayerCS.instance.enabled = true;
    }
}
