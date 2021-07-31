using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManagerCS : MonoBehaviour
{
    public static GameManagerCS instance;

    public bool recievedAllNewTileData = false, recievedAllNewNeutralCityData = false, isTurn = false;

    public int starCount = 100;

    public int currentTroopIndex = 0, currentCityIndex;
    public Dictionary<int, TroopInfo> troops = new Dictionary<int, TroopInfo>();
    public Dictionary<int, CityInfo> cities = new Dictionary<int, CityInfo>();
    public TileInfo[,] tiles;
    public bool isAllTroopInfoReceived = false, isAllTileInfoReceived = false, isAllCityInfoReceived = false;
    public List<Dictionary<TroopInfo, string>> modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
    public List<Dictionary<TileInfo, string>> modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
    public List<Dictionary<CityInfo, string>> modifiedCityInfo = new List<Dictionary<CityInfo, string>>();
    public List<GameObject> objectsToDestroy = new List<GameObject>();

    public GameObject playerPrefab;
    public GameObject localTroopPrefab, remoteTroopPrefab;
    public GameObject starPrefab; 
    public GameObject desertTilePrefab, forestTilePrefab, grasslandTilePrefab, rainForestTilePrefab, swampTilePrefab,
                      tundraTilePrefab, waterTilePrefab;
    public GameObject foodResourcePrefab, woodResourcePrefab, metalResourcePrefab;
    public GameObject cityPrefab;
    public GameObject ownershipObjectPrefab;
    public GameObject lumberYardPrefab, farmPrefab, minePrefab, schoolPrefab, libraryPrefab, domePrefab, housingPrefab, marketPrefab;

    public string[] troopNames;
    public string[] biomeOptions;

    public GameObject startScreenUI, startButtonObject;
    public Button startButton;

    public int minDistanceBetweenCities = 5, maxDistanceBetweenCities = 15, maxDistanceFromResource = 5;

    public PlayerUI playerUI;

    private string cityTag = "City";
    public int whatIsInteractableValue, whatIsDefaultValue;

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
    public void SpawnPlayer(int _id, string _username)
    {
        GameObject _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        _player.GetComponent<PlayerCS>().InitPlayer(_id, _username);
        CreateStars();
        // Spawn King
        //SpawnTroop(ClientCS.instance.myId, "King", Random.Range(0, 10), Random.Range(0, 10), 0);
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
    public void CreateNewTile(int _id, int _ownerId, int _movementCost, int _occupyingObjectId, string _biome,
                              float _temp, float _height, bool _isWater, bool _isFood, bool _isWood, bool _isMetal, 
                              bool _isRoad, bool _isCity, bool _isOccupied, Vector2 _position, int _xIndex, int _zIndex, 
                              int _cityId, string _name)
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
        // Change tile color just a little bit
        MeshRenderer _tileMeshRender = _tile.GetComponent<MeshRenderer>();
        _tileMeshRender.material.color = new Color(_tileMeshRender.material.color.r + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.g + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.b + Random.Range(-.1f, .1f),
                                                    _tileMeshRender.material.color.a);
        // End of changing tile color

        _tile.transform.parent = transform;
        TileInfo _tileInfo = _tile.AddComponent<TileInfo>();
        _tileInfo.moveUI = _tile.transform.GetChild(0).gameObject;  // Get move UI
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
        _tileInfo.isFood = _isFood;
        _tileInfo.isWood = _isWood;
        _tileInfo.isMetal = _isMetal;
        _tileInfo.isRoad = _isRoad;
        _tileInfo.isCity = _isCity;
        _tileInfo.isOccupied = _isOccupied;
        _tileInfo.xIndex = _xIndex;
        _tileInfo.zIndex = _zIndex;
        _tileInfo.cityId = _cityId;
        _tileInfo.position = _position;

        tiles[_xIndex, _zIndex] = _tileInfo;

        // Spawn appropriate resource object
        if (_tileInfo.isFood)
            _tileInfo.resourceObject = Instantiate(foodResourcePrefab, new Vector3(_position.x, foodResourcePrefab.transform.position.y, 
                                                                                    _position.y), foodResourcePrefab.transform.localRotation);
        if (_tileInfo.isWood)
            _tileInfo.resourceObject = Instantiate(woodResourcePrefab, new Vector3(_position.x, woodResourcePrefab.transform.position.y,
                                                                                    _position.y), woodResourcePrefab.transform.localRotation);
        if (_tileInfo.isMetal)
            _tileInfo.resourceObject = Instantiate(metalResourcePrefab, new Vector3(_position.x, metalResourcePrefab.transform.position.y,
                                                                                    _position.y), metalResourcePrefab.transform.localRotation);
        // Spawn tile ownership object
        _tileInfo.ownerShipVisualObject = Instantiate(ownershipObjectPrefab, new Vector3(_position.x, ownershipObjectPrefab.transform.position.y,
                                                                                         _position.y), Quaternion.identity);
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

    /// <summary>
    /// Updates tile's owner and displays it through visual object
    /// </summary>
    /// <param name="_tile"></param>
    public void UpdateOwnedTileInfo(TileInfo _tile)
    {
        tiles[_tile.xIndex, _tile.zIndex].ownerShipVisualObject.SetActive(true);
        UpdateTileInfo(_tile);
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
    public void SpawnTroop(int _ownerId, string _troopName, int _xIndex, int _zIndex, int _rotation)
    {
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        GameObject _troop = Instantiate(localTroopPrefab, new Vector3(_xCoord, 1, _zCoord), Quaternion.identity);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        _troop.AddComponent<TroopInfo>().InitTroopInfo(_troopName, _troop, _troopActions, currentTroopIndex, _ownerId, 
                                                        _xIndex, _zIndex);
       
        troops.Add(currentTroopIndex, _troop.GetComponent<TroopInfo>());
        currentTroopIndex++;
        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop.GetComponent<TroopInfo>(), "Spawn"} };
        modifiedTroopInfo.Add(_troopData);

        TileInfo _tile = tiles[_xIndex, _zIndex];
        _tile.isOccupied = true;
        _tile.occupyingObjectId = _troop.GetComponent<TroopInfo>().id;
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, "Update"} };
        modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Spawn new troop
    /// Does NOT update modified troop dicts.
    /// </summary>
    /// <param name="_troopInfoToCopy"> Existing troop spawn and init </param>
    public void SpawnTroop(TroopInfo _troopInfoToCopy)
    {
        int _xCoord = (int)tiles[_troopInfoToCopy.xIndex, _troopInfoToCopy.zIndex].position.x;
        int _zCoord = (int)tiles[_troopInfoToCopy.xIndex, _troopInfoToCopy.zIndex].position.y;
        GameObject _troop = Instantiate(remoteTroopPrefab, new Vector3(_xCoord, 1, _zCoord),
                                        Quaternion.identity);
        TroopActionsCS _troopActions = _troop.GetComponent<TroopActionsCS>();
        TroopInfo _troopInfo = _troop.AddComponent<TroopInfo>();
        _troopInfo.CopyTroopInfo(_troopInfoToCopy, _troop, _troopActions);

        troops.Add(_troopInfo.id, _troopInfo);
    }

    /// <summary>
    /// Spawn King troop
    /// </summary>
    public void SpawnKing()
    {
        if (!isTurn) return;
        playerUI.playerUIContainer.SetActive(true);
        playerUI.SetAllResourceUI(PlayerCS.instance.food, PlayerCS.instance.food, PlayerCS.instance.metal, PlayerCS.instance.money,
                                  PlayerCS.instance.morale, PlayerCS.instance.education, PlayerCS.instance.population);
        SpawnTroop(ClientCS.instance.myId, "King", Random.Range(0, 10), Random.Range(0, 10), 0);
        startScreenUI.SetActive(false);
    }

    /// <summary>
    /// Move troop to new tile and update modified troop and tile dicts
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"></param>
    public void MoveTroopToNewTile(TroopInfo _troopInfo)
    {
        troops[_troopInfo.id].xIndex = _troopInfo.xIndex;
        troops[_troopInfo.id].zIndex = _troopInfo.zIndex;
        int _xCoord = (int)tiles[_troopInfo.xIndex, _troopInfo.zIndex].position.x;
        int _zCoord = (int)tiles[_troopInfo.xIndex, _troopInfo.zIndex].position.y;
        troops[_troopInfo.id].troop.transform.position = new Vector3(_xCoord,
                                                            troops[_troopInfo.id].troop.transform.position.y,
                                                            _zCoord);
    }

    /// <summary>
    /// Rotate troop based on troop passed through parems 
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"> Troop to rotate </param>
    public void RotateTroop(TroopInfo _troopInfo)
    {
        troops[_troopInfo.id].rotation = _troopInfo.rotation;
        troops[_troopInfo.id].troop.transform.localRotation = Quaternion.Euler(0, troops[_troopInfo.id].rotation, 0);
    }

    /// <summary>
    /// Updates troop with the same id of parem 
    /// Does NOT update modified troop and tile dicts.
    /// </summary>
    /// <param name="_troopInfo"> Troop to rotate </param>
    public void UpdateTroopInfo(TroopInfo _troopInfo)
    {
        troops[_troopInfo.id].UpdateTroopInfo(_troopInfo);
    }

    public void RemoveTroopInfo(TroopInfo _troopInfo)
    {
        _troopInfo.troop.SetActive(false);
        troops.Remove(_troopInfo.id);
        objectsToDestroy.Add(_troopInfo.troop);
    }

    /// <summary>
    /// Reset player's troop to default start turn values for start of next turn
    /// </summary>
    public void ResetTroops()
    {
        foreach (TroopInfo _troop in troops.Values)
        {
            if (_troop.ownerId == ClientCS.instance.myId)
            {
                _troop.movementCost = Constants.troopInfoInt[_troop.troopName]["MovementCost"];
                _troop.canAttack = true;
                _troop.boxCollider.enabled = true;
            }
        }
    }

    /// <summary>
    /// Enable spawn king button
    /// </summary>
    public void ToggleSpawnKingButton()
    {
        if (!(recievedAllNewNeutralCityData && recievedAllNewTileData)) return;
        startButton.enabled = true;
    }

    #endregion

    #region City

    /// <summary>
    /// Create New neutral city
    /// Does NOT update modified cities dict.
    /// </summary>
    public void CreateNewNeutralCity(int _id, int _ownerId, float _moral, float _education, int _ownerShipRange, int _woodResourcesPerTurn, 
                                     int _metalResourcesPerTurn, int _foodResourcesPerTurn, int _moneyResourcesPerTurn, int _populationResourcesPerTurn,
                                     int _xIndex, int _zIndex)
    {
        tiles[_xIndex, _zIndex].tile.tag = "City"; 
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        GameObject _city = Instantiate(cityPrefab, new Vector3(_xCoord, .625f, _zCoord), Quaternion.identity);
        CityInfo _cityInfo = _city.AddComponent<CityInfo>();
        _cityInfo.city = _city;
        _cityInfo.id = _id;
        _cityInfo.ownerId = _ownerId;
        _cityInfo.morale = _moral;
        _cityInfo.education = _education;
        _cityInfo.ownerShipRange = _ownerShipRange;
        _cityInfo.woodResourcesPerTurn = _woodResourcesPerTurn;
        _cityInfo.metalResourcesPerTurn = _metalResourcesPerTurn;
        _cityInfo.foodResourcesPerTurn = _foodResourcesPerTurn;
        _cityInfo.moneyResourcesPerTurn = _moneyResourcesPerTurn;
        _cityInfo.populationResourcesPerTurn = _populationResourcesPerTurn;
        _cityInfo.xIndex = _xIndex;
        _cityInfo.zIndex = _zIndex;
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
        int _xCoord = (int)tiles[_xIndex, _zIndex].position.x;
        int _zCoord = (int)tiles[_xIndex, _zIndex].position.y;
        string _biomeName = tiles[_xIndex, _zIndex].biome;
        GameObject _city = Instantiate(cityPrefab, new Vector3(_xCoord, .625f, _zCoord), Quaternion.identity);
        CityInfo _cityInfo = _city.AddComponent<CityInfo>();
        CityActionsCS _cityActions = _city.GetComponent<CityActionsCS>();
        _cityInfo.InitCity(_biomeName, _city, currentCityIndex, ClientCS.instance.myId, _xIndex, _zIndex, _cityActions);
        _cityActions.InitCityActions(_cityInfo);
        cities.Add(_cityInfo.id, _cityInfo);

        // Update city dicts
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { {_cityInfo, "Spawn" } };
        modifiedCityInfo.Add(_cityData);

        // Update tile
        TileInfo _tile = tiles[_xIndex, _zIndex];
        _tile.isCity = true;
        _tile.cityId = currentCityIndex;
        _tile.ownerId = _cityInfo.ownerId;
        _tile.tile.tag = cityTag;
        _tile.tile.layer = whatIsInteractableValue;
        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
        { { _tile, "Update"} };
        modifiedTileInfo.Add(_tileData);

        // Create owned tiles
        for(int x = _xIndex - _cityInfo.ownerShipRange; x < _xIndex + _cityInfo.ownerShipRange + 1; x++)
        {
            for(int z = _zIndex - _cityInfo.ownerShipRange; z < _zIndex + _cityInfo.ownerShipRange + 1; z++)
            {
                if(x >= 0 && x < tiles.GetLength(0)
                 && z >= 0 && z < tiles.GetLength(1))
                {
                    _tile = tiles[x, z];
                    _tile.ownerId = _cityInfo.ownerId;
                    _tile.ownerShipVisualObject.SetActive(true);
                    _tileData = new Dictionary<TileInfo, string>()
                    { { _tile, "Owned"} };
                    modifiedTileInfo.Add(_tileData);
                }
            }
        }

        // Increase city cost (double values)
        List<string> _priceKeys = Constants.prices["City"].Keys.ToList();
        for (int i = 0; i < _priceKeys.Count; i++)
        {
            Constants.prices["City"][_priceKeys[i]] *= 2;
        }

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
        _cityInfo.InitExistingCity(_cityInfoToSpawn, _city);
        cities.Add(_cityInfo.id, _cityInfo);

    }
    
    public void UpdateCityInfo(CityInfo _cityToCopy)
    {
        cities[_cityToCopy.id].UpdateCityInfo(_cityToCopy);
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
        playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.food, PlayerCS.instance.metal, PlayerCS.instance.money, PlayerCS.instance.population);
        PlayerCS.instance.ResetMoraleAndEducation();
    }

    public void ResetCities()
    {
        /*
        foreach (CityInfo _city in cities.Values)
        {
            if (_city.ownerId == ClientCS.instance.myId)
            {
                //_city.isTrainingTroops = false;
            }
        }
        */
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
        switch (_buildingName)
        {
            case "LumberYard":
                Instantiate(lumberYardPrefab, new Vector3(_xCoord, lumberYardPrefab.transform.position.y,
                                                                       _tile.position.y), lumberYardPrefab.transform.localRotation);
                break;
            case "Farm":
                Instantiate(farmPrefab, new Vector3(_xCoord, farmPrefab.transform.position.y,
                                                       _zCoord), farmPrefab.transform.localRotation);
                break;
            case "Mine":
                Instantiate(minePrefab, new Vector3(_xCoord, minePrefab.transform.position.y,
                                                       _zCoord), minePrefab.transform.localRotation);
                break;
            case "School":
                Instantiate(schoolPrefab, new Vector3(_xCoord, schoolPrefab.transform.position.y,
                                                       _zCoord), schoolPrefab.transform.localRotation);
                break;
            case "Library":
                Instantiate(libraryPrefab, new Vector3(_xCoord, libraryPrefab.transform.position.y,
                                                       _zCoord), libraryPrefab.transform.localRotation);
                break;
            case "Dome":
                Instantiate(domePrefab, new Vector3(_xCoord, domePrefab.transform.position.y,
                                                       _zCoord), domePrefab.transform.localRotation);
                break;
            case "Housing":
                Instantiate(housingPrefab, new Vector3(_xCoord, housingPrefab.transform.position.y,
                                                       _zCoord), housingPrefab.transform.localRotation);
                break;
            case "Market":
                Instantiate(marketPrefab, new Vector3(_xCoord, marketPrefab.transform.position.y,
                                                       _zCoord), marketPrefab.transform.localRotation);
                break;
            default:
                Debug.LogError("Building " + _buildingName + " not found");
                break;
        }

        // Update player resource UI
        playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.wood, PlayerCS.instance.metal, PlayerCS.instance.money, PlayerCS.instance.population);

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
        int _xCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.x;
        int _zCoord = (int)tiles[_tile.xIndex, _tile.zIndex].position.y;
        switch (_tile.buildingName)
        {
            case "LumberYard":
                Instantiate(lumberYardPrefab, new Vector3(_xCoord, lumberYardPrefab.transform.position.y,
                                                                       _tile.position.y), lumberYardPrefab.transform.localRotation);
                break;
            case "Farm":
                Instantiate(farmPrefab, new Vector3(_xCoord, farmPrefab.transform.position.y,
                                                       _zCoord), farmPrefab.transform.localRotation);
                break;
            case "Mine":
                Instantiate(minePrefab, new Vector3(_xCoord, minePrefab.transform.position.y,
                                                       _zCoord), minePrefab.transform.localRotation);
                break;
            case "School":
                Instantiate(schoolPrefab, new Vector3(_xCoord, schoolPrefab.transform.position.y,
                                                       _zCoord), schoolPrefab.transform.localRotation);
                break;
            case "Library":
                Instantiate(libraryPrefab, new Vector3(_xCoord, libraryPrefab.transform.position.y,
                                                       _zCoord), libraryPrefab.transform.localRotation);
                break;
            case "Dome":
                Instantiate(domePrefab, new Vector3(_xCoord, domePrefab.transform.position.y,
                                                       _zCoord), domePrefab.transform.localRotation);
                break;
            case "Housing":
                Instantiate(housingPrefab, new Vector3(_xCoord, housingPrefab.transform.position.y,
                                                       _zCoord), housingPrefab.transform.localRotation);
                break;
            case "Market":
                Instantiate(marketPrefab, new Vector3(_xCoord, marketPrefab.transform.position.y,
                                                       _zCoord), marketPrefab.transform.localRotation);
                break;
            default:
                Debug.LogError("Building " + _tile.buildingName + " not found");
                break;
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
        // Update/Show Other Player troop movements/actions
        foreach (Dictionary<TroopInfo, string> _troopDict in modifiedTroopInfo)
        {
            foreach (TroopInfo _troop in _troopDict.Keys)
            {
                switch (_troopDict[_troop])
                {
                    case "Spawn":
                        SpawnTroop(_troop);
                        break;
                    case "Move":
                        MoveTroopToNewTile(_troop);
                        break;
                    case "Rotate":
                        RotateTroop(_troop);
                        break;
                    case "Attack":
                        UpdateTroopInfo(_troop);
                        break;
                    case "Hurt":
                        UpdateTroopInfo(_troop);
                        break;
                    case "Die":
                        RemoveTroopInfo(troops[_troop.id]);
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
                    case "Owned":
                        UpdateOwnedTileInfo(_tile);
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
                if(_city.ownerId == ClientCS.instance.myId)
                {
                    Debug.Log("Found in first loop with value " + _city.isTrainingTroops);
                }
                switch (_cityDict[_city])
                {
                    case "Spawn":
                        SpawnCity(_city);
                        break;
                    case "Update":
                        UpdateCityInfo(_city);
                        break;
                    case "TrainTroop":
                        //UpdateCityInfo(_city);
                        break;
                    case "Conquer":
                        UpdateCityInfo(_city);
                        break;
                    case "SpawnTroop":
                        //UpdateCityInfo(_city);
                        break;
                    default:
                        Debug.LogError("Could not find city action: " + _cityDict[_city]);
                        break;
                }
                Destroy(_city);
            }
        }
        ClearModifiedData();

        // Start of turn city stuff
        foreach(CityInfo _city in cities.Values)
        {
            if (_city.isBeingConquered)
                _city.isAbleToBeConquered = true;
            else
                _city.isAbleToBeConquered = false;
            if(_city.ownerId == ClientCS.instance.myId)
            {
                if(_city.isTrainingTroops)
                {
                    _city.isTrainingTroops = false;
                    _city.cityActions.SpawnTroop();
                }
            }
        }

        AddCityResourcesAtStartOfTurn();
        PlayerCS.instance.enabled = true;
    }

    #endregion
}
