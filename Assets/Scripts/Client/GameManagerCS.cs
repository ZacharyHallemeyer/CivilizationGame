﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManagerCS : MonoBehaviour
{
    public static GameManagerCS instance;

    public bool recievedAllNewTileData = false, recievedAllNewNeutralCityData = false, isTurn = false;
    public int turnCount;

    public int starCount = 100;

    public int currentTroopIndex = 0, currentCityIndex;
    public List<string> avaliableTribes = new List<string>();
    public Dictionary<int, TroopInfo> troops = new Dictionary<int, TroopInfo>();
    public Dictionary<int, CityInfo> cities = new Dictionary<int, CityInfo>();
    public TileInfo[,] tiles;
    public bool isAllTroopInfoReceived = false, isAllTileInfoReceived = false, isAllCityInfoReceived = false;
    public List<Dictionary<TroopInfo, string>> modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
    public List<Dictionary<TileInfo, string>> modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
    public List<Dictionary<CityInfo, string>> modifiedCityInfo = new List<Dictionary<CityInfo, string>>();
    public List<GameObject> objectsToDestroy = new List<GameObject>();
    public bool isKingAlive;
    public GameObject dataStoringObject;

    public GameObject playerPrefab;
    public GameObject localTroopPrefab, remoteTroopPrefab, blurredTroopPrefab;
    public GameObject troopHealthTextPrefab, troopRotationIndicationPrefab;
    public GameObject starPrefab;
    public GameObject tilePrefab;
    public GameObject foodResourcePrefab, woodResourcePrefab, metalResourcePrefab, obstaclePrefab;
    public GameObject cityPrefab, cityLevel1Prefab, cityLevel2Prefab, cityLevel3Prefab, cityLevel4Prefab, cityLevel5Prefab;
    public GameObject ownershipObjectPrefab;
    public GameObject lumberYardPrefab, farmPrefab, minePrefab, schoolPrefab, libraryPrefab, domePrefab, housingPrefab, marketPrefab, 
                      portPrefab, wallsPrefab;
    public GameObject scoutPrefab, militiaPrefab, armyPrefab, misslePrefab, defensePrefab, stealthPrefab,  
                      snipperPrefab, kingPrefab, canoePrefab, warShipPrefab, watchTowerPrefab, heavyHitterPrefab;
    public GameObject swordPrefab, arrowPrefab;

    public string[] troopNames;
    public string[] biomeOptions;

    public GameObject sword, gun, arrow;
    public Rigidbody arrowRB;
    public ParticleSystem gunBullet;

    public int minDistanceBetweenCities = 5, maxDistanceBetweenCities = 15, maxDistanceFromResource = 5;

    // Lobby
    public GameObject startScreenUI, startButtonObject;
    public Button startButton;

    // King Death Screen
    public GameObject kingDeathScreen;

    private string cityTag = "City";
    public int whatIsInteractableValue, whatIsDefaultValue;

    public Dictionary<string, GameObject> buildingPrefabs;
    public Dictionary<string, GameObject> shipModels;
    public Dictionary<string, GameObject> troopModels;

    #region Set Up Functions

    // Set instance or destroy if instance already exist
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        buildingPrefabs = new Dictionary<string, GameObject>()
        {
            { "LumberYard", lumberYardPrefab},
            { "Farm", farmPrefab},
            { "Mine", minePrefab},
            { "Housing", housingPrefab},
            { "School", schoolPrefab},
            { "Library", libraryPrefab},
            { "Market", marketPrefab},
            { "Dome", domePrefab},
            { "Port", portPrefab},
            { "Walls", wallsPrefab},
        };

        shipModels = new Dictionary<string, GameObject>()
        {
            { "Canoe", canoePrefab },
            { "Warship", warShipPrefab },
        };

        troopModels = new Dictionary<string, GameObject>()
        {
            { "Scout", scoutPrefab },
            { "Militia", militiaPrefab },
            { "Army", armyPrefab },
            { "Missle", misslePrefab },
            { "Defense", defensePrefab },
            { "Stealth", stealthPrefab },
            { "Snipper", snipperPrefab },
            { "King", kingPrefab },
            { "HeavyHitter", heavyHitterPrefab },
            { "WatchTower", watchTowerPrefab },
        };
    }

    // Init needed data
    private void Start()
    {
        int _index = 0;
        biomeOptions = new string[Constants.biomeInfo.Count];
        foreach (string _biomeName in Constants.biomeInfo.Keys)
        {
            biomeOptions[_index] = _biomeName;
            _index++;
        }
        _index = 0;
        troopNames = new string[Constants.troopInfoInt.Count];
        foreach (string _troopName in Constants.troopInfoInt.Keys)
        {
            troopNames[_index] = _troopName;
            _index++;
        }
        whatIsInteractableValue = LayerMask.NameToLayer("Interactable");
        whatIsDefaultValue = LayerMask.NameToLayer("Default");
    }

    /// <summary>
    /// Spawns player controller
    /// </summary>
    /// <param name="_id"> client id </param>
    /// <param name="_username"> client username </param>
    public void SpawnPlayer(int _id, string _username, string _tribe)
    {
        GameObject _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        _player.GetComponent<PlayerCS>().InitPlayer(_id, _username, _tribe);
        CreateStars();
        // Spawn weapons
        sword = Instantiate(swordPrefab, Vector3.zero, swordPrefab.transform.localRotation);
        sword.SetActive(false);
        arrow = Instantiate(arrowPrefab, Vector3.zero, arrowPrefab.transform.localRotation);
        arrowRB = arrow.GetComponent<Rigidbody>();
        arrow.SetActive(false);
    }

    public void CreateStars()
    {
        for (int i = 0; i < starCount; i++)
        {
            Transform _starMesh = Instantiate(starPrefab, RandomStarPosition(), Quaternion.identity).GetComponent<Transform>();
            _starMesh.transform.parent = transform;
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
                _position = new Vector3(Random.Range(-_lengthX, _lengthX), 60, Random.Range(-_lengthZ, _lengthZ));
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

    #endregion

    #region Tiles

    /// <summary>
    /// Spawn new tile w/ tile data from server and visual style to represent what kind of biome the tile is
    /// </summary>
    /// <param name="_id"> Tile id </param>
    /// <param name="_ownerId"> Tile ownerId </param>
    /// <param name="_movementCost"> Tile movementCost </param>
    /// <param name="_occupyingObjectId"> Tile occupyingObjectId </param>
    /// <param name="_biome"> Tile biome </param>
    /// <param name="_temp"> Tile temperature </param>
    /// <param name="_height"> Tile height </param>
    /// <param name="_isWater"> Tile isWater </param>
    /// <param name="_isRoad"> Tile isRoad </param>
    /// <param name="_isCity"> Tile isCity </param>
    /// <param name="_isOccupied"> Tile isOccupied </param>
    /// <param name="_position"> Tile position </param>
    /// <param name="_xIndex"> Tile xIndex </param>
    /// <param name="_zIndex"> Tile zIndex </param>
    /// <param name="_name"> Tile name </param>
    public void CreateNewTile(int _id, int _ownerId, int _occupyingObjectId, string _biome,
                              float _temp, float _height, bool _isWater, bool _isFood, bool _isWood, bool _isMetal,
                              bool _isRoad, bool _isWall, bool _isCity, bool _isOccupied, bool _isObstacle, Vector2 _position, 
                              int _xIndex, int _zIndex, int _cityId, string _name)
    {
        GameObject _tile = Instantiate(tilePrefab, new Vector3(_position.x, 0, _position.y), tilePrefab.transform.localRotation);
        // Set tile color, tile model is the first child of tile prefab
        MeshRenderer _tileMeshRender = _tile.transform.GetChild(0).GetComponent<MeshRenderer>();
        _tileMeshRender.material.color = Constants.tileColors[_biome];
        // Change tile color just a little bit
        _tileMeshRender.material.color = new Color(_tileMeshRender.material.color.r + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.g + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.b + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.a);
        // End of changing tile color

        _tile.transform.parent = transform;
        TileInfo _tileInfo = _tile.GetComponent<TileInfo>();
        _tile.name = _name;
        _tileInfo.id = _id;
        _tileInfo.ownerId = _ownerId;
        _tileInfo.occupyingObjectId = _occupyingObjectId;
        _tileInfo.biome = _biome;
        _tileInfo.temperature = _temp;
        _tileInfo.height = _height;
        _tileInfo.isWater = _isWater;
        _tileInfo.isFood = _isFood;
        _tileInfo.isWood = _isWood;
        _tileInfo.isMetal = _isMetal;
        _tileInfo.isRoad = _isRoad;
        _tileInfo.isWall = _isWall;
        _tileInfo.isCity = _isCity;
        _tileInfo.isOccupied = _isOccupied;
        _tileInfo.isObstacle = _isObstacle;
        _tileInfo.xIndex = _xIndex;
        _tileInfo.zIndex = _zIndex;
        _tileInfo.cityId = _cityId;
        _tileInfo.position = _position;

        if (_isCity)
        {
            _tileInfo.fixedCell = true;
            _tileInfo.boxCollider.enabled = true;
        }

        tiles[_xIndex, _zIndex] = _tileInfo;

        // Spawn appropriate resource object
        if (_tileInfo.isFood)
        {
            _tileInfo.resourceObject = Instantiate(foodResourcePrefab, new Vector3(_position.x, foodResourcePrefab.transform.position.y,
                                                                                    _position.y), foodResourcePrefab.transform.localRotation);
            _tileInfo.resourceObject.transform.parent = _tile.transform;
        }

        if (_tileInfo.isWood)
        {
            _tileInfo.resourceObject = Instantiate(woodResourcePrefab, new Vector3(_position.x, woodResourcePrefab.transform.position.y,
                                                                                    _position.y), woodResourcePrefab.transform.localRotation);
            _tileInfo.resourceObject.transform.parent = _tile.transform;
        }
        if (_tileInfo.isMetal)
        {
            _tileInfo.resourceObject = Instantiate(metalResourcePrefab, new Vector3(_position.x, metalResourcePrefab.transform.position.y,
                                                                                    _position.y), metalResourcePrefab.transform.localRotation);
            _tileInfo.resourceObject.transform.parent = _tile.transform;

        }
        if (_tileInfo.isObstacle)
        {
            _tileInfo.resourceObject = Instantiate(obstaclePrefab, new Vector3(_position.x, obstaclePrefab.transform.position.y,
                                                                                _position.y), obstaclePrefab.transform.localRotation);
            _tileInfo.resourceObject.transform.parent = _tile.transform;
        }
        // Spawn tile ownership object
        _tileInfo.ownerShipVisualObject = Instantiate(ownershipObjectPrefab, new Vector3(_position.x, ownershipObjectPrefab.transform.position.y,
                                                                                         _position.y), ownershipObjectPrefab.transform.localRotation);
        _tileInfo.ownerShipVisualObject.transform.parent = _tile.transform;
        _tileInfo.ownerShipVisualObject.SetActive(false);
    }

    /// <summary>
    /// Updates tile info
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_tile"> tile to update </param>
    public void UpdateTileInfo(TileInfo _tile)
    {
        tiles[_tile.xIndex, _tile.zIndex].UpdateTileInfo(_tile);
    }

    public void ChangeTileOccupation(TileInfo _tile)
    {
        tiles[_tile.xIndex, _tile.zIndex].isOccupied = _tile.isOccupied;
        tiles[_tile.xIndex, _tile.zIndex].occupyingObjectId = _tile.occupyingObjectId;
    }

    /// <summary>
    /// Updates tile's owner and displays it through visual object
    /// </summary>
    /// <param name="_tile"></param>
    public void ChangeTileOwnership(TileInfo _tile)
    {
        tiles[_tile.xIndex, _tile.zIndex].ownerId = _tile.ownerId;
        tiles[_tile.xIndex, _tile.zIndex].ownerShipVisualObject.SetActive(true);
        // Change Color to tribe color
        tiles[_tile.xIndex, _tile.zIndex].ownerShipVisualObject.GetComponent<MeshRenderer>().material.color =
            Constants.tribeBodyColors[ClientCS.allClients[_tile.ownerId]["Tribe"]];
    }

    #endregion

    #region Troop

    /// <summary>
    /// Spawn new troop
    /// </summary>
    /// <param name="_ownerId"> client id that wants to spawn new troop </param>
    /// <param name="_troopName"> troop name to use when grabbing init troop data </param>
    /// <param name="_xCoord"> x coord to spawn troop </param>
    /// <param name="_zCoord"> z coord to spawn troop </param>
    /// <param name="_rotation"> rotation of new troop </param>
    public void SpawnLocalTroop(int _ownerId, string _troopName, int _xIndex, int _zIndex, int _rotation, bool _canUseThisWave)
    {
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        GameObject _troop = Instantiate(localTroopPrefab, new Vector3(_xCoord, Constants.troopYPosition, _zCoord), localTroopPrefab.transform.localRotation);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        TroopInfo _troopInfo = _troop.AddComponent<TroopInfo>();
        _troopInfo.troop = _troop;
        SpawnTroopModel(_troopInfo, _troopName);
        // Get rotation indication object from troop prefab 
        _troopInfo.rotationIndicationModel = _troop.transform.GetChild(0).gameObject;
        _troopInfo.rotationIndicationModel.SetActive(false);
        // Get exhausted particle system
        _troopInfo.exhaustedParicleSystem = _troop.transform.GetChild(1).GetComponent<ParticleSystem>();
        // Spawn troop with canoe ship stats because they will always start as a canoe when they step onto a port for the first time
        _troopInfo.InitTroopInfo(_troopName, _troopActions, currentTroopIndex, _ownerId, "Canoe", _xIndex, _zIndex);
        _troopInfo.isExposed = true;
        _troopInfo.healthTextObject = Instantiate(troopHealthTextPrefab, new Vector3(_troop.transform.position.x,
                                                                                    troopHealthTextPrefab.transform.position.y,
                                                                                    _troop.transform.position.z),
                                                                         troopHealthTextPrefab.transform.rotation);
        _troopInfo.healthText = _troopInfo.healthTextObject.transform.GetChild(0).GetComponent<TextMeshPro>();
        _troopInfo.healthText.text = _troopInfo.health.ToString();
        _troopInfo.troopActions.CheckTroopSeeingRange();

        // Change color to tribe color
        _troopInfo.troopModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = Constants.tribeBodyColors[PlayerCS.instance.tribe];
        _troopInfo.troopModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].color = Constants.tribeEyeColors[PlayerCS.instance.tribe];
        _troopInfo.shipModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color =
                                                        Constants.tribeBodyColors[ClientCS.allClients[_troopInfo.ownerId]["Tribe"]];
        _troopInfo.rotationIndicationModel.GetComponent<MeshRenderer>().material.color = 
                                                    Constants.tribeBodyColors[ClientCS.allClients[_troopInfo.ownerId]["Tribe"]];

        troops.Add(currentTroopIndex, _troop.GetComponent<TroopInfo>());
        currentTroopIndex++;

        if(!_canUseThisWave)
        {
            _troopInfo.movementCost = 0;
            _troopInfo.canAttack = false;
            _troopInfo.troopActions.TroopCanNotCommitAnyMoreActions();
        }

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troopInfo, "Spawn"} };
        modifiedTroopInfo.Add(_troopData);

        TileInfo _tile = tiles[_xIndex, _zIndex];
        _tile.isOccupied = true;
        _tile.occupyingObjectId = _troop.GetComponent<TroopInfo>().id;
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, "OccupyChange"} };
        modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Spawn new troop
    /// Does NOT update modified troop dicts.
    /// </summary>
    /// <param name="_troopInfoToCopy"> Existing troop spawn and init </param>
    public void SpawnRemoteTroop(TroopInfo _troopInfoToCopy)
    {
        int _xCoord = (int)tiles[_troopInfoToCopy.xIndex, _troopInfoToCopy.zIndex].position.x;
        int _zCoord = (int)tiles[_troopInfoToCopy.xIndex, _troopInfoToCopy.zIndex].position.y;
        GameObject _troop = Instantiate(remoteTroopPrefab, new Vector3(_xCoord, .899f, _zCoord), localTroopPrefab.transform.localRotation);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        TroopInfo _troopInfo = _troop.AddComponent<TroopInfo>();
        _troopInfo.troop = _troop;
        SpawnTroopModel(_troopInfo, _troopInfoToCopy.troopName);
        _troopInfo.CopyTroopInfo(_troopInfoToCopy, _troopActions);
        _troopInfo.healthTextObject = Instantiate(troopHealthTextPrefab, new Vector3(
                                                                            _troop.transform.position.x,
                                                                            troopHealthTextPrefab.transform.position.y,
                                                                            _troop.transform.position.z),
                                                                 troopHealthTextPrefab.transform.rotation);
        _troopInfo.healthText = _troopInfo.healthTextObject.transform.GetChild(0).GetComponent<TextMeshPro>();
        _troopInfo.healthText.text = _troopInfo.health.ToString();
        _troopInfo.healthTextObject.SetActive(false);
        _troopInfo.blurredTroopModel = Instantiate(blurredTroopPrefab, _troop.transform.position, blurredTroopPrefab.transform.localRotation);
        _troopInfo.blurredTroopModel.transform.parent = _troop.transform;
        _troopInfo.blurredTroopModel.SetActive(false);
        // Blurred ship model is just the canoe model without tribe color
        _troopInfo.blurredShipModel = Instantiate(canoePrefab, _troop.transform.position, canoePrefab.transform.localRotation);
        _troopInfo.blurredShipModel.transform.parent = _troop.transform;
        _troopInfo.blurredShipModel.SetActive(false);
        // Only deactivate troop model because it has already been spawned
        _troopInfo.troopModel.SetActive(false);

        // Change color to tribe color
        _troopInfo.troopModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color =
                                                                Constants.tribeBodyColors[ClientCS.allClients[_troopInfo.ownerId]["Tribe"]];
        _troopInfo.troopModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].color = Constants.tribeEyeColors[PlayerCS.instance.tribe];
        _troopInfo.shipModel.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color =
                                                                Constants.tribeBodyColors[ClientCS.allClients[_troopInfo.ownerId]["Tribe"]];

        troops.Add(_troopInfo.id, _troopInfo);
    }

    public void SpawnTroopModel(TroopInfo _troop, string _troopName)
    {
        _troop.troopModel = Instantiate(scoutPrefab, _troop.troop.transform.position, scoutPrefab.transform.localRotation);
        foreach(string _troopKey in troopModels.Keys)
        {
            if(_troopKey == _troopName)
            {
                _troop.troopModel = Instantiate(troopModels[_troopKey], _troop.troop.transform.position,
                                                troopModels[_troopKey].transform.localRotation);
            }
        }

        _troop.troopModel.transform.parent = _troop.troop.transform;

        // Spawn canoe model and deactivate it because troop cannot be spawned as a ship
        _troop.shipModel = Instantiate(canoePrefab, _troop.troop.transform.position, canoePrefab.transform.localRotation);
        _troop.shipModel.transform.parent = _troop.troop.transform;
        _troop.shipModel.SetActive(false);
    }

    /// <summary>
    /// Spawn King troop
    /// </summary>
    public void SpawnKing()
    {
        if (!isTurn) return;
        isKingAlive = true;
        PlayerCS.instance.playerUI.playerUIContainer.SetActive(true);
        PlayerCS.instance.playerUI.SetAllResourceUI(PlayerCS.instance.food, PlayerCS.instance.food, PlayerCS.instance.metal, 
                                                    PlayerCS.instance.money, PlayerCS.instance.morale, PlayerCS.instance.education, 
                                                    PlayerCS.instance.population);
        // Find king spawn
        int _xPos = 0, _yPos = 0;
        int _distanceLimiterX = tiles.GetLength(0) / ClientCS.allClients.Count;
        int _distanceLimiterY = tiles.GetLength(1) / ClientCS.allClients.Count;
        bool _isSpawnPointGood = false;
        while(!_isSpawnPointGood)
        {
            _isSpawnPointGood = true;
            _xPos = Random.Range(0, tiles.GetLength(0));
            _yPos = Random.Range(0, tiles.GetLength(1));
            for(int _index = 0; _index < troops.Count; _index++)
            {
                if( Mathf.Abs(troops[_index].xIndex - _xPos) < _distanceLimiterX 
                    || Mathf.Abs(troops[_index].zIndex - _yPos) < _distanceLimiterY
                    || tiles[_xPos, _yPos].isWater)
                {
                    _isSpawnPointGood = false;
                }
            }
        }

        SpawnLocalTroop(ClientCS.instance.myId, "King", _xPos, _yPos, 0, true);
        
        startScreenUI.SetActive(false);
    }

    /// <summary>
    /// Move troop to new tile and update modified troop and tile dicts
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"></param>
    public IEnumerator MoveTroopToNewTile(TroopInfo _troopToCopy)
    {
        TroopInfo _troop = troops[_troopToCopy.id];
        PlayerCS.instance.isAnimInProgress = true;
        StartCoroutine(DescendTroopMoveAnin(_troop, _troopToCopy));
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    ///  Troop descends below current tile and teleports to the tile that _troopToCopy is at.
    ///  Then this function calls AscendTroopMoveAnim
    /// </summary>
    /// <param name="_troop"> Troop to move </param>
    /// <param name="_troopToCopy"> Troop containing the info on where to move to </param>
    public IEnumerator DescendTroopMoveAnin(TroopInfo _troop, TroopInfo _troopToCopy)
    {
        while (_troop.transform.position.y > -.2f)
        {
            _troop.transform.position = new Vector3(_troop.transform.position.x, _troop.transform.position.y - .1f, _troop.transform.position.z);
            // Using overlay texture so turn off health text when troop is under a tile
            if (_troop.healthTextObject.transform.position.y < -1 && _troop.healthTextObject.activeSelf)
                _troop.healthTextObject.SetActive(false);
            yield return new WaitForSeconds(.0001f);
        }
        _troop.xIndex = _troopToCopy.xIndex;
        _troop.zIndex = _troopToCopy.zIndex;
        _troop.transform.position = new Vector3((int)tiles[_troopToCopy.xIndex, _troopToCopy.zIndex].position.x, 
                                                      _troop.transform.position.y,
                                                      (int)tiles[_troopToCopy.xIndex, _troopToCopy.zIndex].position.y);
        _troop.healthTextObject.transform.position = new Vector3(_troop.transform.position.x, Constants.troopHealthYPositionDescend,
                                                                 _troop.transform.position.z);

        // Change to ship model if player moved onto a port
        if (_troop.isBoat && !_troop.shipModel.gameObject.activeInHierarchy)
        {
            if (_troop.isExposed)
            {
                _troop.troopModel.SetActive(false);
                _troop.shipModel.SetActive(true);
            }
            else
            {
                _troop.blurredTroopModel.SetActive(false);
                _troop.blurredShipModel.SetActive(true);
            }
        }
        // Change to troop model if player moved onto a land and was a boat before moving
        else if (!_troop.isBoat && _troop.shipModel.gameObject.activeInHierarchy)
        {
            if (_troop.isExposed)
            {
                _troop.troopModel.SetActive(true);
                _troop.shipModel.SetActive(false);
            }
            else
            {
                _troop.blurredTroopModel.SetActive(true);
                _troop.blurredShipModel.SetActive(false);
            }
        }

        StartCoroutine(AscendTroopMoveAnim(_troop));
    }

    /// <summary>
    /// Troop ascends above the current tile it is under
    /// </summary>
    /// <param name="_troop"> Troop to move </param>
    public IEnumerator AscendTroopMoveAnim(TroopInfo _troop)
    {
        while (_troop.transform.position.y < 1)
        {
            _troop.transform.position = new Vector3(_troop.transform.position.x, _troop.transform.position.y + .1f, _troop.transform.position.z);
            // Using overlay texture so turn on health text when troop is above a tile
            if (_troop.healthTextObject.transform.position.y > -1 && !_troop.healthTextObject.activeSelf && _troop.isExposed)
                _troop.healthTextObject.SetActive(true);
            _troop.healthTextObject.transform.position = new Vector3(_troop.transform.position.x,
                                                            _troop.healthTextObject.transform.position.y + .1f,
                                                            _troop.transform.position.z);
            yield return new WaitForSeconds(.0001f);
        }
        _troop.transform.position = new Vector3(_troop.transform.position.x, Constants.troopYPosition, _troop.transform.position.z);
        _troop.healthTextObject.transform.position = new Vector3(_troop.transform.position.x,
                                                                  Constants.troopHealthYPositionAscend,
                                                                  _troop.transform.position.z);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Rotates troop with the information in _troopInfo
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"> Troop info class containing rotation to update with </param>
    /// <returns></returns>
    public IEnumerator RotateTroop(TroopInfo _troopInfo)
    {
        yield return new WaitForEndOfFrame();
        troops[_troopInfo.id].rotation = _troopInfo.rotation;
        troops[_troopInfo.id].troop.transform.localRotation = Quaternion.Euler(troops[_troopInfo.id].troop.transform.localEulerAngles.x,
                                                                               troops[_troopInfo.id].rotation,
                                                                               troops[_troopInfo.id].troop.transform.localEulerAngles.z);
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Sets up attack animation coming from server and updates troop info 
    /// </summary>
    /// <param name="_troopInfo"> Troop that is attack </param>
    /// <returns></returns>
    public IEnumerator AttackTroop(TroopInfo _troopInfo)
    {
        yield return new WaitForEndOfFrame();
        TroopInfo _troopToAttack = troops[_troopInfo.lastTroopAttackedId];
        TroopInfo _troopAttacking = troops[_troopInfo.id];
        _troopAttacking.lastTroopAttackedId = _troopInfo.lastTroopAttackedId;
        _troopAttacking.attackRotation = _troopInfo.attackRotation;
        //_troopAttacking.UpdateTroopInfo(_troopInfo);
        int _distance = Mathf.Abs(_troopToAttack.xIndex - _troopAttacking.xIndex) + Mathf.Abs(_troopToAttack.zIndex - _troopAttacking.zIndex);
        if (_distance == 1)
            StartCoroutine(SwordAttackAnimation(_troopAttacking, _troopAttacking.attackRotation));
        else
            StartCoroutine(ArrowAttackAnimation(_troopAttacking, _troopToAttack));
    }

    /// <summary>
    /// Plays sword attack animation comming from server
    /// </summary>
    /// <param name="_troopAttacking"> Troop that is attacking </param>
    /// <param name="_localXRotation"> rotation to attack in </param>
    /// <returns></returns>
    public IEnumerator SwordAttackAnimation(TroopInfo _troopAttacking, int _localXRotation)
    {
        int _xCoord = (int)tiles[_troopAttacking.xIndex, _troopAttacking.zIndex].position.x;
        int _zCoord = (int)tiles[_troopAttacking.xIndex, _troopAttacking.zIndex].position.y;
        PlayerCS.instance.isAnimInProgress = true;
        sword.transform.position = new Vector3(_xCoord, 2, _zCoord);
        sword.transform.localRotation = Quaternion.Euler(-90, _localXRotation, 0);
        sword.SetActive(true);
        while (sword.transform.localEulerAngles.x < .1f || sword.transform.localEulerAngles.x > 10.1f)
        {
            sword.transform.localRotation *= Quaternion.Euler(10, 0, 0);
            yield return new WaitForSeconds(.001f);
        }
        sword.SetActive(false);
        sword.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Plays arrow attack animation comming from server
    /// </summary>
    /// <param name="_troopAttacking"> Troop attacking </param>
    /// <param name="_troopToAttack"> Troop to attack </param>
    /// <returns></returns>
    public IEnumerator ArrowAttackAnimation(TroopInfo _troopAttacking, TroopInfo _troopToAttack)
    {
        PlayerCS.instance.isAnimInProgress = true;
        TileInfo _tile = tiles[_troopAttacking.xIndex, _troopAttacking.zIndex];
        switch (_troopAttacking.rotation)
        {
            case 0:
                arrow.transform.position = new Vector3(_tile.transform.position.x,
                                                        1,
                                                        _tile.transform.position.z + 1);
                break;
            case 90:
                arrow.transform.position = new Vector3(_tile.transform.position.x + 1,
                                                        1,
                                                        _tile.transform.position.z);
                break;
            case 180:
                arrow.transform.position = new Vector3(_tile.transform.position.x,
                                                        1,
                                                        _tile.transform.position.z - 1);
                break;
            case 270:
                arrow.transform.position = new Vector3(_tile.transform.position.x - 1,
                                                        1,
                                                        _tile.transform.position.z);
                break;
            default:
                Debug.LogError("Could not accomplish task with rotation " + _troopAttacking.rotation);
                break;
        }
        arrow.transform.localRotation = Quaternion.Euler(arrow.transform.localEulerAngles.x, _troopAttacking.rotation, 0);
        arrow.SetActive(true);

        TileInfo _tileToGoTo = tiles[_troopToAttack.xIndex, _troopToAttack.zIndex];

        arrowRB.AddForce(_troopAttacking.troopModel.transform.forward * 200 * Time.deltaTime, ForceMode.Impulse);

        while (Mathf.Abs(arrow.transform.position.x - _tileToGoTo.transform.position.x) > .5f)
        {
            yield return new WaitForSeconds(.01f);
        }
        while (Mathf.Abs(arrow.transform.position.z - _tileToGoTo.transform.position.z) > .5f)
        {
            yield return new WaitForSeconds(.01f);
        }
        arrow.SetActive(false);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Plays troop hurt animation and updates troop info 
    /// </summary>
    /// <param name="_troopToCopy"> Troop info class containing information to copy </param>
    /// <returns></returns>
    public IEnumerator HurtTroop(TroopInfo _troopToCopy)
    {
        TroopInfo _troopInfo = troops[_troopToCopy.id];
        PlayerCS.instance.isAnimInProgress = true;
        _troopInfo.health = _troopToCopy.health;
        _troopInfo.healthText.text = _troopInfo.health.ToString();
        for (int i = 0; i < 18; i++)
        {
            yield return new WaitForSeconds(.01f);
            _troopInfo.troop.transform.localRotation *= Quaternion.Euler(0, 20, 0);
        }
        _troopInfo.troop.transform.rotation = Quaternion.Euler(0, _troopInfo.rotation, 0);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Plays troop die animation coming from server and removes troop from appropriate dicts/list
    /// </summary>
    /// <param name="_troopInfo"></param>
    /// <returns></returns>
    public IEnumerator RemoveTroopInfo(TroopInfo _troopInfo)
    {
        TroopInfo _troop = troops[_troopInfo.id];
        // Die animation
        PlayerCS.instance.isAnimInProgress = true;
        while (_troop.troop.transform.localPosition.y > -1.5f)
        {
            _troop.troop.transform.localRotation *= Quaternion.Euler(5, 0, 5);
            _troop.troop.transform.localPosition = new Vector3(_troop.troop.transform.localPosition.x,
                                                                        _troop.troop.transform.localPosition.y - .05f,
                                                                        _troop.troop.transform.localPosition.z);
            yield return new WaitForSeconds(.001f);
        }
        _troop.troop.SetActive(false);
        _troop.healthTextObject.SetActive(false);
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
        
        // Remove Troop
        if (_troop.troopName == "King" && _troop.ownerId == ClientCS.instance.myId)
            KingIsDead();
        _troop.troop.SetActive(false);
        troops.Remove(_troop.id);
        objectsToDestroy.Add(_troop.troop);
        PlayerCS.instance.runningCoroutine = null;
    }

    public IEnumerator UpdateTroopInfo(TroopInfo _troopInfo)
    {
        troops[_troopInfo.id].UpdateTroopInfo(_troopInfo);
        PlayerCS.instance.runningCoroutine = null;
        yield return null;
    }

    /// <summary>
    /// Plays troop die animation coming from server and removes troop from appropriate dicts/list
    /// </summary>
    /// <param name="_troopInfo"></param>
    /// <returns></returns>
    public IEnumerator SwitchLandOrSeaModelTroopInfo(TroopInfo _troopInfo)
    {
        PlayerCS.instance.isAnimInProgress = true;
        TroopInfo _troop = troops[_troopInfo.id];
        _troop.isBoat = _troopInfo.isBoat;
        yield return null;
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Plays troop die animation coming from server and removes troop from appropriate dicts/list
    /// </summary>
    /// <param name="_troopInfo"></param>
    /// <returns></returns>
    public IEnumerator ChangeShipModelTroopInfo(TroopInfo _troopInfo)
    {
        PlayerCS.instance.isAnimInProgress = true;
        TroopInfo _troop = troops[_troopInfo.id];
        Destroy(_troop.shipModel);
        _troop.shipModel = Instantiate(shipModels[_troopInfo.shipName], _troop.troop.transform.position, 
                                       shipModels[_troopInfo.shipName].transform.localRotation);
        _troop.shipModel.transform.parent = _troop.troop.transform;
        _troop.shipModel.SetActive(false);
        yield return null;
        PlayerCS.instance.isAnimInProgress = false;
        PlayerCS.instance.runningCoroutine = null;
    }

    /// <summary>
    /// Reset player's troop to default start turn values for start of next turn
    /// </summary>
    public IEnumerator ResetTroops()
    {
        foreach (TroopInfo _troop in troops.Values)
        {
            if (_troop.ownerId == ClientCS.instance.myId)
            {
                _troop.movementCost = Constants.troopInfoInt[_troop.troopName]["MovementCost"];
                _troop.canAttack = true;
                _troop.boxCollider.enabled = true;
                _troop.troopActions.TroopCanCommitMoreActions();
            }
        }
        PlayerCS.instance.runningCoroutine = null;
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Enable spawn king button
    /// </summary>
    public void ToggleSpawnKingButton()
    {
        if (!(recievedAllNewNeutralCityData && recievedAllNewTileData)) return;
        startButton.enabled = true;
    }

    /// <summary>
    /// Sets up info for owned king dying
    /// </summary>
    public void KingIsDead()
    {
        PlayerCS.instance.animationQueue.Enqueue(KingIsDeadHelper());
    }

    /// <summary>
    /// Turns off player action controls and turns on king death screen
    /// </summary>
    /// <returns></returns>
    public IEnumerator KingIsDeadHelper()
    {
        isKingAlive = false;
        kingDeathScreen.SetActive(true);
        PlayerCS.instance.isAbleToCommitActions = false;
        yield return new WaitForEndOfFrame();
    }

    public void ResetOwnedCitiesAndTiles(int _ownerId)
    {
        TileInfo _tile;
        Dictionary<TileInfo, string> _tileData;
        Dictionary<CityInfo, string> _cityData;
        // Change all owned tiles and all owned cities to netural
        foreach (CityInfo _cityInfo in cities.Values)
        {
            if (_cityInfo.ownerId == _ownerId)
            {
                // Change all owned tiles to neutral
                for (int x = _cityInfo.xIndex - _cityInfo.ownerShipRange; x < _cityInfo.xIndex + _cityInfo.ownerShipRange + 1; x++)
                {
                    for (int z = _cityInfo.zIndex - _cityInfo.ownerShipRange; z < _cityInfo.zIndex + _cityInfo.ownerShipRange + 1; z++)
                    {
                        if (x >= 0 && x < tiles.GetLength(0)
                         && z >= 0 && z < tiles.GetLength(1))
                        {
                            _tile = tiles[x, z];
                            if (_tile.ownerId == _ownerId)
                            {
                                _tile.ownerId = -1;
                                _tile.ownerShipVisualObject.SetActive(false);
                                _tileData = new Dictionary<TileInfo, string>()
                                 { { _tile, "Ownership"} };
                                modifiedTileInfo.Add(_tileData);
                            }
                        }
                    }
                }
                // Change all owned cities to neutral
                _cityInfo.ownerId = -1;
                _cityData = new Dictionary<CityInfo, string>()
                { { _cityInfo, "Conquer" } };
                modifiedCityInfo.Add(_cityData);
            }
        }
    }

    #endregion

    #region City

    /// <summary>
    /// Create New neutral city
    /// Does NOT update modified cities dict.
    /// </summary>
    public void CreateNewNeutralCity(int _id, int _ownerId, int _morale, int _education, int _ownerShipRange, int _woodResourcesPerTurn, 
                                     int _metalResourcesPerTurn, int _foodResourcesPerTurn, int _moneyResourcesPerTurn, int _populationResourcesPerTurn,
                                     int _xIndex, int _zIndex, int _level)
    {
        tiles[_xIndex, _zIndex].tile.tag = "City"; 
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        GameObject _city = Instantiate(cityPrefab, new Vector3(_xCoord, .625f, _zCoord), Quaternion.identity);
        CityInfo _cityInfo = _city.AddComponent<CityInfo>();
        _cityInfo.cityModel = Instantiate(cityLevel1Prefab, new Vector3(_city.transform.position.x,
                                                                        cityLevel1Prefab.transform.position.y,
                                                                        _city.transform.position.z),
                                                            cityLevel1Prefab.transform.localRotation);
        _cityInfo.cityModel.transform.parent = _city.transform;
        _cityInfo.CopyNeutralCityData(_city, _id, _ownerId, _morale, _education, _ownerShipRange, _woodResourcesPerTurn, 
                                      _metalResourcesPerTurn, _foodResourcesPerTurn, _moneyResourcesPerTurn, _populationResourcesPerTurn, 
                                      _xIndex, _zIndex, _level);
        cities.Add(_cityInfo.id, _cityInfo);
    }

    /// <summary>
    /// Spawns new city and updates modified cities dict.
    /// </summary>
    /// <param name="_xCoord"> x coord to spawn city </param>
    /// <param name="_zCoord"> z coord to spawn city </param>
    public void SpawnCity(int _xIndex, int _zIndex)
    {
        if (tiles[_xIndex, _zIndex].isWater || tiles[_xIndex, _zIndex].isCity) return;
        tiles[_xIndex, _zIndex].tile.tag = "City";
        tiles[_xIndex, _zIndex].fixedCell = true;
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        string _biomeName = tiles[_xIndex, _zIndex].biome;
        GameObject _city = Instantiate(cityPrefab, new Vector3(_xCoord, .625f, _zCoord), Quaternion.identity);
        CityInfo _cityInfo = _city.AddComponent<CityInfo>();
        _cityInfo.cityModel = Instantiate(cityLevel1Prefab, new Vector3(_city.transform.position.x,
                                                                        cityLevel1Prefab.transform.position.y,
                                                                        _city.transform.position.z),
                                                            cityLevel1Prefab.transform.localRotation);
        _cityInfo.cityModel.transform.parent = _city.transform;
        CityActionsCS _cityActions = _city.GetComponent<CityActionsCS>();
        _cityInfo.InitCity(_biomeName, _city, currentCityIndex, ClientCS.instance.myId, _xIndex, _zIndex, _cityActions);
        _cityActions.InitCityActions(_cityInfo);
        cities.Add(_cityInfo.id, _cityInfo);

        // Update city dicts
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { {_cityInfo, "Create" } };
        modifiedCityInfo.Add(_cityData);

        // Update tile
        TileInfo _tile = tiles[_xIndex, _zIndex];
        _tile.isCity = true;
        _tile.cityId = currentCityIndex;
        _tile.ownerId = _cityInfo.ownerId;
        _tile.tile.tag = cityTag;
        _tile.tile.layer = whatIsInteractableValue;
        _tile.fixedCell = true;
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
        { { _tile, "Update"} };
        modifiedTileInfo.Add(_tileData);

        // Create owned tiles
        CreateOwnedTiles(_cityInfo);

        // Increase city cost (double values)
        List<string> _priceKeys = Constants.prices["City"].Keys.ToList();
        for (int i = 0; i < _priceKeys.Count; i++)
        {
            Constants.prices["City"][_priceKeys[i]] *= 2;
        }

        // Add resources based on biome info
        PlayerCS.instance.food += (int)Constants.biomeInfo[_biomeName]["Food"];
        PlayerCS.instance.wood += (int)Constants.biomeInfo[_biomeName]["Wood"];
        PlayerCS.instance.metal += (int)Constants.biomeInfo[_biomeName]["Metal"];
        PlayerCS.instance.population += (int)Constants.biomeInfo[_biomeName]["Population"];
        PlayerCS.instance.playerUI.SetAllIntResourceUI();

        // Update morale and education
        PlayerCS.instance.ResetMoraleAndEducation();
        currentCityIndex++;
    }

    /// <summary>
    /// Spawns city with existing city data
    /// Does NOT update modified cities dict.
    /// </summary>
    /// <param name="_cityInfoToSpawn"> city to spawn </param>
    public void SpawnCity(CityInfo _cityInfoToSpawn)
    {
        int _xCoord = (int)tiles[_cityInfoToSpawn.xIndex, _cityInfoToSpawn.zIndex].position.x;
        int _zCoord = (int)tiles[_cityInfoToSpawn.xIndex, _cityInfoToSpawn.zIndex].position.y;
        tiles[_cityInfoToSpawn.xIndex, _cityInfoToSpawn.zIndex].tile.tag = "City";
        GameObject _city = Instantiate(cityPrefab, new Vector3(_xCoord, .75f, _zCoord),
                           Quaternion.identity);
        CityInfo _cityInfo = _city.AddComponent<CityInfo>();
        _cityInfo.cityModel = Instantiate(cityLevel1Prefab, new Vector3(_city.transform.position.x,
                                                                        cityLevel1Prefab.transform.position.y,
                                                                        _city.transform.position.z),
                                                            cityLevel1Prefab.transform.localRotation);
        _cityInfo.cityModel.transform.parent = _city.transform;
        _cityInfo.InitExistingCity(_cityInfoToSpawn, _city);
        cities.Add(_cityInfo.id, _cityInfo);
    }

    /// <summary>
    /// Modifies the tile's ownership around the city given in parems to be owned by city owner.
    /// This function skips over already owned tiles.
    /// Do not use for creating owned tiles when troops conquer cities because of previous comment line.
    /// </summary>
    /// <param name="_cityInfo"></param>
    public void CreateOwnedTiles(CityInfo _cityInfo)
    {
        TileInfo _tile;
        Dictionary<TileInfo, string> _tileData;
        // Create owned tiles
        for (int x = _cityInfo.xIndex - _cityInfo.ownerShipRange; x < _cityInfo.xIndex + _cityInfo.ownerShipRange + 1; x++)
        {
            for (int z = _cityInfo.zIndex - _cityInfo.ownerShipRange; z < _cityInfo.zIndex + _cityInfo.ownerShipRange + 1; z++)
            {
                if (x >= 0 && x < tiles.GetLength(0)
                 && z >= 0 && z < tiles.GetLength(1))
                {
                    _tile = tiles[x, z];
                    if(_tile.ownerId == -1)
                    { 
                        _tile.ownerId = _cityInfo.ownerId;
                        _tile.ownerShipVisualObject.SetActive(true);
                        // Change color of ownership visual object
                        _tile.ownerShipVisualObject.GetComponent<MeshRenderer>().material.color =
                            Constants.tribeBodyColors[ClientCS.allClients[_tile.ownerId]["Tribe"]];
                        _tileData = new Dictionary<TileInfo, string>()
                        { { _tile, "OwnershipChange"} };
                        modifiedTileInfo.Add(_tileData);
                    }
                }
            }
        }
    }
    
    public void UpdateCityInfo(CityInfo _cityToCopy)
    {
        cities[_cityToCopy.id].UpdateCityInfo(_cityToCopy);
    }

    public void ConquerCityInfo(CityInfo _cityToCopy)
    {
        cities[_cityToCopy.id].ownerId = _cityToCopy.ownerId;
    }

    public void LevelUpCity(CityInfo _cityToLevelUp)
    {
        CityInfo _city = cities[_cityToLevelUp.id];
        // Update necessary Info
        _city.ownerShipRange = _cityToLevelUp.ownerShipRange;
        _city.woodResourcesPerTurn = _cityToLevelUp.woodResourcesPerTurn;
        _city.metalResourcesPerTurn = _cityToLevelUp.metalResourcesPerTurn;
        _city.foodResourcesPerTurn = _cityToLevelUp.foodResourcesPerTurn;
        _city.moneyResourcesPerTurn = _cityToLevelUp.moneyResourcesPerTurn;
        _city.populationResourcesPerTurn = _cityToLevelUp.populationResourcesPerTurn;
        _city.level = _cityToLevelUp.level;
        _city.experience = _cityToLevelUp.experience;
        _city.experienceToNextLevel = _cityToLevelUp.experienceToNextLevel;

        // Destroy old model and instantiate new one
        Destroy(_city.cityModel);
        GameObject _cityModel = null;
        if (_city.level == 2)
            _cityModel = cityLevel2Prefab;
        else if (_city.level == 3)
            _cityModel = cityLevel3Prefab;
        else if (_city.level == 4)
            _cityModel = cityLevel4Prefab;
        else if (_city.level == 5)
            _cityModel = cityLevel5Prefab;
        _city.cityModel = Instantiate(_cityModel, new Vector3(_city.city.transform.position.x,
                                                              _cityModel.transform.position.y,
                                                              _city.city.transform.position.z),
                                                          _cityModel.transform.localRotation);
    }

    public void AddCityResourcesAtStartOfTurn()
    {
        foreach(CityInfo _city in cities.Values)
        {
            if(_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
            {
                PlayerCS.instance.wood += _city.woodResourcesPerTurn;
                PlayerCS.instance.metal += _city.metalResourcesPerTurn;
                PlayerCS.instance.food += _city.foodResourcesPerTurn;
                PlayerCS.instance.money += _city.moneyResourcesPerTurn;
                PlayerCS.instance.population += _city.populationResourcesPerTurn;
            }
        }
        PlayerCS.instance.playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.food, PlayerCS.instance.metal, 
                                                       PlayerCS.instance.money, PlayerCS.instance.population);
        PlayerCS.instance.ResetMoraleAndEducation();
    }

    /// <summary>
    /// Returns amount of resource that player will recieve next turn based on resource type given
    /// </summary>
    /// <param name="_resourceType"> Has to be "Wood", "Metal", "Food", "Money", "Population" </param>
    /// <returns></returns>
    public int GetCityResourcesAddedNextTurn(string _resourceType)
    {
        int _resourceAmount = 0;

        if(_resourceType == "Wood")
        {
            foreach (CityInfo _city in cities.Values)
                if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
                    _resourceAmount += _city.woodResourcesPerTurn;
        }
        else if(_resourceType == "Metal")
        {
            foreach (CityInfo _city in cities.Values)
                if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
                    _resourceAmount += _city.metalResourcesPerTurn;
        }
        else if(_resourceType == "Food")
        {
            foreach (CityInfo _city in cities.Values)
                if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
                    _resourceAmount += _city.foodResourcesPerTurn;
        }
        else if(_resourceType == "Money")
        {
            foreach (CityInfo _city in cities.Values)
                if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
                    _resourceAmount += _city.moneyResourcesPerTurn;
        }
        else if(_resourceType == "Population")
        {
            foreach (CityInfo _city in cities.Values)
                if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
                    _resourceAmount += _city.populationResourcesPerTurn;
        }

        return _resourceAmount;
    }

    #endregion

    #region Building

    /// <summary>
    /// Spawns building cooresponding to building name (parem) at tile (parem) and updates modified tile dict
    /// </summary>
    /// <param name="_buildingName"> type of building to spawn </param>
    /// <param name="_tile"> tile to spawn building </param>
    public void SpawnBuilding(string _buildingName, TileInfo _tile)
    {
        int _xCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.x;
        int _zCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.y;
        foreach(string _key in buildingPrefabs.Keys)
        {
            if(_key == _buildingName)
                Instantiate(buildingPrefabs[_key], new Vector3(_xCoord, buildingPrefabs[_key].transform.position.y,
                                                               _zCoord), buildingPrefabs[_key].transform.localRotation);
        }
        // Update player resource UI
        PlayerCS.instance.playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.wood, PlayerCS.instance.metal, PlayerCS.instance.money, PlayerCS.instance.population);

        // Update tile
        _tile.isBuilding = true;
        _tile.buildingName = _buildingName;

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
        { { _tile, "BuildBuilding"} };
        modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Spawns building cooresponding to tile information
    /// Does NOT update modified tile dict.
    /// </summary>
    /// <param name="_tile"> tile to spawn building and contains building name to spawn </param>
    public void SpawnBuilding(TileInfo _tile)
    {
        tiles[_tile.xIndex, _tile.zIndex].isRoad = _tile.isRoad;
        tiles[_tile.xIndex, _tile.zIndex].isBuilding = _tile.isBuilding;
        tiles[_tile.xIndex, _tile.zIndex].isWall = _tile.isBuilding;
        tiles[_tile.xIndex, _tile.zIndex].buildingName = _tile.buildingName;
        int _xCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.x;
        int _zCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.y;
        foreach (string _key in buildingPrefabs.Keys)
        {
            if (_key == _tile.buildingName)
                Instantiate(buildingPrefabs[_key], new Vector3(_xCoord, buildingPrefabs[_key].transform.position.y,
                                                               _zCoord), buildingPrefabs[_key].transform.localRotation);
        }
    }

    #endregion

    #region Turn Functions

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
        modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
        modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
        modifiedCityInfo = new List<Dictionary<CityInfo, string>>();
    }

    public void DestroyObjectsToDestroyAtEndOfTurn()
    {
        foreach (GameObject _object in objectsToDestroy)
            Destroy(_object);
        objectsToDestroy = new List<GameObject>();
    }

    /// <summary>
    /// Plays the moves for each other players since this players last turn
    /// </summary>
    public void PlayPastMoves()
    {
        dataStoringObject = new GameObject("Data Storing Object");
        objectsToDestroy.Add(dataStoringObject);
        // Update/Show Other Player troop movements/actions
        foreach (Dictionary<TroopInfo, string> _troopDict in modifiedTroopInfo)
        {
            foreach (TroopInfo _troop in _troopDict.Keys)
            {
                switch (_troopDict[_troop])
                {
                    case "Spawn":
                        SpawnRemoteTroop(_troop);
                        break;
                    case "Move":
                        PlayerCS.instance.animationQueue.Enqueue(MoveTroopToNewTile(_troop));
                        break;
                    case "Rotate":
                        PlayerCS.instance.animationQueue.Enqueue(RotateTroop(_troop));
                        break;
                    case "Attack":
                        PlayerCS.instance.animationQueue.Enqueue(AttackTroop(_troop));
                        break;
                    case "Hurt":
                        PlayerCS.instance.animationQueue.Enqueue(HurtTroop(_troop));
                        break;
                    case "Die":
                        PlayerCS.instance.animationQueue.Enqueue(RemoveTroopInfo(_troop));
                        break;
                    case "SwitchModel":
                        PlayerCS.instance.animationQueue.Enqueue(SwitchLandOrSeaModelTroopInfo(_troop));
                        break;
                    case "Update":
                        PlayerCS.instance.animationQueue.Enqueue(UpdateTroopInfo(_troop));
                        break;
                    case "ChangeShipModel":
                        PlayerCS.instance.animationQueue.Enqueue(ChangeShipModelTroopInfo(_troop));
                        break;
                    default:
                        Debug.LogError("Could not find troop action: " + _troopDict[_troop]);
                        break;
                }
                Destroy(_troop);
            }
        } 
        foreach(Dictionary<TileInfo, string> _tileDict in modifiedTileInfo)
        {
            foreach(TileInfo _tile in _tileDict.Keys)
            {
                switch (_tileDict[_tile])
                {
                    case "Update":
                        UpdateTileInfo(_tile);
                        break;
                    case "OccupyChange":
                        ChangeTileOccupation(_tile);
                        break;
                    case "OwnershipChange":
                        ChangeTileOwnership(_tile);
                        break;
                    case "BuildBuilding":
                        SpawnBuilding(_tile);
                        break;
                    default:
                        Debug.LogError("Could not find tile action: " + _tileDict[_tile]);
                        break;
                }
                Destroy(_tile);
            }
        }
        foreach (Dictionary<CityInfo, string> _cityDict in modifiedCityInfo)
        {
            foreach (CityInfo _city in _cityDict.Keys)
            {
                switch (_cityDict[_city])
                {
                    case "Create":
                        SpawnCity(_city);
                        break;
                    case "Update":
                        UpdateCityInfo(_city);
                        break;
                    case "LevelUp":
                        LevelUpCity(_city);
                        break;
                    case "Conquer":
                        UpdateCityInfo(_city);
                        break;
                    default:
                        Debug.LogError("Could not find city action: " + _cityDict[_city]);
                        break;
                }
                Destroy(_city);
            }
        }
        ClearModifiedData();
        PlayerCS.instance.animationQueue.Enqueue(ResetTroops());

        // Start of turn city stuff
        foreach(CityInfo _city in cities.Values)
        {
            if (_city.isBeingConquered)
                _city.isAbleToBeConquered = true;
            else
                _city.isAbleToBeConquered = false;
            if (_city.ownerId == ClientCS.instance.myId)
            {
                if (_city.isFeed)
                {
                    if (_city.morale < _city.maxMorale)
                        _city.morale++;
                    if (_city.education < _city.maxEducation)
                        _city.education++;
                }
                else
                {
                    _city.morale--;
                    _city.education--;
                }

                // Reset feed bool flag
                _city.isFeed = false;
            }
        }

        // Hide all enemy troops
        foreach (TroopInfo _troop in troops.Values)
        {
            if (_troop.ownerId != ClientCS.instance.myId)
            {
                _troop.troopModel.SetActive(false);
                _troop.blurredTroopModel.SetActive(false);
                _troop.shipModel.SetActive(false);
                _troop.blurredShipModel.SetActive(false);
                _troop.blurredTroopModel.SetActive(false);
                _troop.healthTextObject.SetActive(false);
            }
        }

        // Check if any troops are in seeing range and show them if they are
        foreach (TroopInfo _troop in troops.Values)
        {
            if (_troop.ownerId == ClientCS.instance.myId)
                _troop.troopActions.CheckTroopSeeingRange();
        }

        AddCityResourcesAtStartOfTurn();
        PlayerCS.instance.enabled = true;
    }

    public void UpdatePrices()
    {
        foreach(string _priceKey in Constants.prices.Keys)
        {
            Constants.prices[_priceKey]["Food"] = Constants.basePrices[_priceKey]["Food"] - (int)(PlayerCS.instance.morale * Constants.moraleMultiplier);
            Constants.prices[_priceKey]["Metal"] = Constants.basePrices[_priceKey]["Metal"] - (int)(PlayerCS.instance.morale * Constants.moraleMultiplier);
            Constants.prices[_priceKey]["Wood"] = Constants.basePrices[_priceKey]["Wood"] - (int)(PlayerCS.instance.morale * Constants.moraleMultiplier);
            Constants.prices[_priceKey]["Money"] = Constants.basePrices[_priceKey]["Money"] - (int)(PlayerCS.instance.morale * Constants.moraleMultiplier);
            
            if (Constants.prices[_priceKey]["Food"] < 1)
                Constants.prices[_priceKey]["Food"] = 1;
            if (Constants.prices[_priceKey]["Metal"] < 1)
                Constants.prices[_priceKey]["Metal"] = 1;
            if (Constants.prices[_priceKey]["Wood"] < 1)
                Constants.prices[_priceKey]["Wood"] = 1;
            if (Constants.prices[_priceKey]["Money"] < 50)
                Constants.prices[_priceKey]["Money"] = 50;
        }

        List<string> _skillPriceKeys = Constants.allSkills.Keys.ToList();

        foreach(string _priceKey in _skillPriceKeys)
        {
            Constants.allSkills[_priceKey] = Constants.allSkillsBasePrice[_priceKey]
                                             - (int)(PlayerCS.instance.education * Constants.educationMultiplier);
            if (Constants.allSkills[_priceKey] < 50)
                Constants.allSkills[_priceKey] = 50;
        }
    }

    #endregion
}
