using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
    /// <summary>
    /// Connects client to server
    /// </summary>
    /// <param name="_packet">msg and id</param>
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        //Debug.Log($"Message from server: {_msg}");
        ClientCS.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        ClientCS.instance.udp.Connect(((IPEndPoint)ClientCS.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary>
    /// Adds client to allClient dictionary and inits lobby with new client
    /// </summary>
    /// <param name="_packet"></param>
    public static void AddClient(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _clientUsername = _packet.ReadString();
        string _clientTribe = _packet.ReadString();

        ClientCS.allClients.Add(_clientId, new Dictionary<string, string>() { { "Username", _clientUsername}, { "Tribe", _clientTribe } });
        ClientCS.instance.lobby.InitLobbyUI();
    }

    /// <summary>
    /// Recieve which player disconnected from server and remove from dictionries
    /// </summary>
    /// <param name="_packet"> id </param>
    public static void PlayerDisconnected(Packet _packet)
    {
        //Debug.Log("Player disconnected called");
        int _id = _packet.ReadInt();

        ClientCS.allClients.Remove(_id);
        ClientCS.instance.lobby.InitLobbyUI();
    }

    public static void RecieveUpdateTribeChoice(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        // Set Tribe
        ClientCS.allClients[_clientId]["Tribe"] = _packet.ReadString();
        ClientCS.instance.lobby.InitLobbyUI();
    }

    public static void RecieveAvaliableTribes(Packet _packet)
    {
        int _tribeLength = _packet.ReadInt();
        GameManagerCS.instance.avaliableTribes = new List<string>();
        // Add each tribe to avaliable tribe list
        for (int i = 0; i < _tribeLength; i++)
            GameManagerCS.instance.avaliableTribes.Add(_packet.ReadString());
    }

    /// <summary>
    /// Toggle lobby start button to allow host to start game
    /// </summary>
    /// <param name="_packet"></param>
    public static void WorldCreated(Packet _packet)
    {
        ClientCS.instance.lobby.ToggleStartButtonState();
    }

    /// <summary>
    /// Spawn player controller and init needed values
    /// </summary>
    /// <param name="_packet"> Packet from server </param>
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        int _xSizeOfTiles = _packet.ReadInt();
        int _zSizeOfTiles = _packet.ReadInt();
        string _tribe = _packet.ReadString();
        GameManagerCS.instance.tiles = new TileInfo[_xSizeOfTiles, _zSizeOfTiles];
        ClientCS.instance.tribe = _tribe;

        // Turn off lobby UI if it has not already
        if (ClientCS.instance.lobby.lobbyParent.activeInHierarchy)
            ClientCS.instance.lobby.lobbyParent.SetActive(false);
        // Turn on spawn king UI
        GameManagerCS.instance.startScreenUI.SetActive(true);
        GameManagerCS.instance.SpawnPlayer(_id, _username, _tribe);
    }
    
    /// <summary>
    /// Recieve new tile info from client and call spawn tile function
    /// </summary>
    /// <param name="_packet"> Packet containing new tile info </param>
    public static void CreateNewTile(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if(_id == -1)
        {
            GameManagerCS.instance.recievedAllNewTileData = true;
            GameManagerCS.instance.ToggleSpawnKingButton();
            return;
        }
        int _ownerId = _packet.ReadInt();
        int _occupyingObjectId = _packet.ReadInt();
        string _biome = _packet.ReadString();
        float _temperature = _packet.ReadFloat();
        float _height = _packet.ReadFloat();
        bool _isWater = _packet.ReadBool();
        bool _isFood = _packet.ReadBool();
        bool _isWood = _packet.ReadBool();
        bool _isMetal = _packet.ReadBool();
        bool _isRoad = _packet.ReadBool();
        bool _isWall = _packet.ReadBool();
        bool _isCity = _packet.ReadBool();
        bool _isOccupied = _packet.ReadBool();
        bool _isObstacle = _packet.ReadBool();
        Vector2 _position = _packet.ReadVector2();
        int _xIndex = _packet.ReadInt();
        int _zIndex = _packet.ReadInt();
        int _cityId = _packet.ReadInt();
        string _name = "ClientTile " + _xIndex + " " + _zIndex;

        GameManagerCS.instance.CreateNewTile(_id, _ownerId, _occupyingObjectId, _biome, _temperature,
                                            _height, _isWater, _isFood, _isWood, _isMetal, _isRoad, _isWall, _isCity, 
                                            _isOccupied, _isObstacle, _position, _xIndex, _zIndex, _cityId ,_name);
    }

    public static void CreateNeutralCity(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)
        {
            GameManagerCS.instance.recievedAllNewNeutralCityData = true;
            GameManagerCS.instance.ToggleSpawnKingButton();
            return;
        }
        int _ownerId = _packet.ReadInt();
        int _morale = _packet.ReadInt();
        int _education = _packet.ReadInt();
        int _ownerShipRange = _packet.ReadInt();
        int _woodResourcePerTurn = _packet.ReadInt();
        int _metalResourcePerTurn = _packet.ReadInt();
        int _foodResourcePerTurn = _packet.ReadInt();
        int _moneyResourcesPerTurn = _packet.ReadInt();
        int _populationResourcePerTurn = _packet.ReadInt();
        int  _xIndex = _packet.ReadInt();
        int  _zIndex = _packet.ReadInt();
        int  _level = _packet.ReadInt();

        GameManagerCS.instance.CreateNewNeutralCity(_id, _ownerId, _morale, _education,_ownerShipRange, _woodResourcePerTurn, 
                                                    _metalResourcePerTurn, _foodResourcePerTurn,_moneyResourcesPerTurn, 
                                                    _populationResourcePerTurn, _xIndex, _zIndex, _level);

    }

    #region Troop Info

    /// <summary>
    /// Recieve new troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveSpawnTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.ownerId = _packet.ReadInt();
        _troop.troopName = _packet.ReadString();
        _troop.xIndex = _packet.ReadInt();
        _troop.zIndex = _packet.ReadInt();
        _troop.rotation = _packet.ReadInt();
        _troop.health = _packet.ReadInt();
        _troop.baseAttack = _packet.ReadInt();
        _troop.stealthAttack = _packet.ReadInt();
        _troop.counterAttack = _packet.ReadInt();
        _troop.baseDefense = _packet.ReadInt();
        _troop.facingDefense = _packet.ReadInt();
        _troop.attackRange = _packet.ReadInt();
        _troop.seeRange = _packet.ReadInt();
        _troop.canMultyKill = _packet.ReadBool();
        _troop.lastTroopAttackedId = _packet.ReadInt();
        _troop.attackRotation = _packet.ReadInt();
        _troop.shipName = _packet.ReadString();
        _troop.shipAttack = _packet.ReadInt();
        _troop.shipStealthAttack = _packet.ReadInt();
        _troop.shipCounterAttack = _packet.ReadInt();
        _troop.shipBaseDefense = _packet.ReadInt();
        _troop.shipFacingDefense = _packet.ReadInt();
        _troop.shipMovementCost = _packet.ReadInt();
        _troop.shipAttackRange = _packet.ReadInt();
        _troop.shipSeeRange = _packet.ReadInt();
        _troop.shipCanMultyKill = _packet.ReadBool();
        _troop.shipCanMoveAfterKill = _packet.ReadBool();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve moved troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveMoveTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.xIndex = _packet.ReadInt();
        _troop.zIndex = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve rotated troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveRotateTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.rotation = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve attack troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveAttackTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.lastTroopAttackedId = _packet.ReadInt();
        _troop.attackRotation = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve hurt troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveHurtTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.health = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve dead troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveDieTroopInfo(Packet _packet)
    {
        Debug.Log("Received switch model info from server");
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }
    

    /// <summary>
    /// Recieve info on whether troop has switch to a ship or back into a troop troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveSwitchLandOrSeaModelTroopInfo(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.isBoat = _packet.ReadBool();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    public static void ReceiveChangeShipModel(Packet _packet)
    {
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _packet.ReadInt();
        _troop.shipName = _packet.ReadString();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    /// <summary>
    /// Recieve all updated troop data from server
    /// When all modified troop data is sent from server. This function will receive a -1 for an id which represents all data was received
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void ReceiveUpdatedTroopInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)      // If the id is equal to -1 then all data has been recieved
        {
            GameManagerCS.instance.isAllTroopInfoReceived = true;
            return;
        }
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _id;
        _troop.ownerId = _packet.ReadInt();
        _troop.troopName = _packet.ReadString();
        _troop.xIndex = _packet.ReadInt();
        _troop.zIndex = _packet.ReadInt();
        _troop.rotation = _packet.ReadInt();
        _troop.health = _packet.ReadInt();
        _troop.baseAttack = _packet.ReadInt();
        _troop.stealthAttack = _packet.ReadInt();
        _troop.counterAttack = _packet.ReadInt();
        _troop.baseDefense = _packet.ReadInt();
        _troop.facingDefense = _packet.ReadInt();
        _troop.attackRange = _packet.ReadInt();
        _troop.seeRange = _packet.ReadInt();
        _troop.canMultyKill = _packet.ReadBool();
        _troop.lastTroopAttackedId = _packet.ReadInt();
        _troop.attackRotation = _packet.ReadInt();
        _troop.shipName = _packet.ReadString();
        _troop.shipAttack = _packet.ReadInt();
        _troop.shipStealthAttack = _packet.ReadInt();
        _troop.shipCounterAttack = _packet.ReadInt();
        _troop.shipBaseDefense = _packet.ReadInt();
        _troop.shipFacingDefense = _packet.ReadInt();
        _troop.shipMovementCost = _packet.ReadInt();
        _troop.shipAttackRange = _packet.ReadInt();
        _troop.shipSeeRange = _packet.ReadInt();
        _troop.shipCanMultyKill = _packet.ReadBool();
        _troop.shipCanMoveAfterKill = _packet.ReadBool();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);
    }

    #endregion

    #region Tile Info

    /// <summary>
    /// Recieve Occupation change tile data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void ReceiveOccupyChangeTileInfo(Packet _packet)
    {
        TileInfo _tile = GameManagerCS.instance.gameObject.AddComponent<TileInfo>();
        _tile.id = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.isOccupied = _packet.ReadBool();
        _tile.occupyingObjectId = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Recieve ownership change tile data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void ReceiveOwnershipChangeTileInfo(Packet _packet)
    {
        TileInfo _tile = GameManagerCS.instance.gameObject.AddComponent<TileInfo>();
        _tile.id = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.ownerId = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Recieve data to build a specific building on a certain tile from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void ReceiveBuildBuildingTileInfo(Packet _packet)
    {
        TileInfo _tile = GameManagerCS.instance.gameObject.AddComponent<TileInfo>();
        _tile.id = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.isRoad = _packet.ReadBool();
        _tile.isWall = _packet.ReadBool();
        _tile.isBuilding = _packet.ReadBool();
        _tile.buildingName = _packet.ReadString();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Recieve data to build a road on a certain tile from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void ReceiveBuildRoadTileInfo(Packet _packet)
    {
        TileInfo _tile = GameManagerCS.instance.gameObject.AddComponent<TileInfo>();
        _tile.id = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.isRoad = _packet.ReadBool();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    /// <summary>
    /// Recieve all updated tile data from server
    /// When all modified tile data is sent from server. This function will receive a -1 for an id which represents all data was received
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void ReceiveUpdatedTileInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)      // If the id is equal to -1 then all data has been recieved
        {
            GameManagerCS.instance.isAllTileInfoReceived = true;
            return;
        }
        TileInfo _tile = GameManagerCS.instance.gameObject.AddComponent<TileInfo>();
        _tile.id = _id;
        _tile.ownerId = _packet.ReadInt();
        _tile.isRoad = _packet.ReadBool();
        _tile.isCity = _packet.ReadBool();
        _tile.isWall = _packet.ReadBool();
        _tile.isBuilding = _packet.ReadBool();
        _tile.isOccupied = _packet.ReadBool();
        _tile.occupyingObjectId = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.cityId = _packet.ReadInt();
        _tile.buildingName = _packet.ReadString();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    #endregion

    #region City Info

    /// <summary>
    /// Recieve new city data from server to spawn on client side
    /// </summary>
    /// <param name="_packet"> Packet containing modified city info </param>
    public static void ReceiveCreateCityInfo(Packet _packet)
    {
        CityInfo _city = GameManagerCS.instance.gameObject.AddComponent<CityInfo>();
        _city.id = _packet.ReadInt();
        _city.ownerId = _packet.ReadInt();
        _city.morale = _packet.ReadInt();
        _city.education = _packet.ReadInt();
        _city.ownerShipRange = _packet.ReadInt();
        _city.woodResourcesPerTurn = _packet.ReadInt();
        _city.metalResourcesPerTurn = _packet.ReadInt();
        _city.foodResourcesPerTurn = _packet.ReadInt();
        _city.moneyResourcesPerTurn = _packet.ReadInt();
        _city.populationResourcesPerTurn = _packet.ReadInt();
        _city.isBeingConquered = _packet.ReadBool();
        _city.isConstructingBuilding = _packet.ReadBool();
        _city.occupyingObjectId = _packet.ReadInt();
        _city.xIndex = _packet.ReadInt();
        _city.zIndex = _packet.ReadInt();
        _city.level = _packet.ReadInt();
        _city.experience = _packet.ReadInt();
        _city.experienceToNextLevel = _packet.ReadInt();
        string _command = _packet.ReadString();

        // Add data to dictionary to be used when displaying past moves
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    /// <summary>
    /// Recieve city data to level up certain city from server on client side
    /// </summary>
    /// <param name="_packet"> Packet containing modified city info </param>
    public static void ReceiveLevelUpCityInfo(Packet _packet)
    {
        CityInfo _city = GameManagerCS.instance.gameObject.AddComponent<CityInfo>();
        _city.id = _packet.ReadInt();
        _city.ownerId = _packet.ReadInt();
        _city.ownerShipRange = _packet.ReadInt();
        _city.woodResourcesPerTurn = _packet.ReadInt();
        _city.metalResourcesPerTurn = _packet.ReadInt();
        _city.foodResourcesPerTurn = _packet.ReadInt();
        _city.moneyResourcesPerTurn = _packet.ReadInt();
        _city.populationResourcesPerTurn = _packet.ReadInt();
        _city.level = _packet.ReadInt();
        _city.experience = _packet.ReadInt();
        _city.experienceToNextLevel = _packet.ReadInt();
        string _command = _packet.ReadString();

        // Add data to dictionary to be used when displaying past moves
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    /// <summary>
    /// Recieve conquered city data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified city info </param>
    public static void ReceiveConquerCityInfo(Packet _packet)
    {
        CityInfo _city = GameManagerCS.instance.gameObject.AddComponent<CityInfo>();
        _city.id = _packet.ReadInt();
        _city.ownerId = _packet.ReadInt();
        string _command = _packet.ReadString();

        // Add data to dictionary to be used when displaying past moves
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    /// <summary>
    /// Recieve modified city data from server
    /// When all modified tile data is sent from server. This function will receive a -1 for an id which represents all data was received
    /// </summary>
    /// <param name="_packet"> Packet containing modified city info </param>
    public static void ReceiveUpdatedCityInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)      // If the id is equal to -1 then all data has been recieved
        {
            GameManagerCS.instance.isAllCityInfoReceived = true;
            return;
        }
        CityInfo _city = GameManagerCS.instance.gameObject.AddComponent<CityInfo>();
        _city.id = _id;
        _city.ownerId = _packet.ReadInt();
        _city.morale = _packet.ReadInt();
        _city.education = _packet.ReadInt();
        _city.ownerShipRange = _packet.ReadInt();
        _city.woodResourcesPerTurn = _packet.ReadInt();
        _city.metalResourcesPerTurn = _packet.ReadInt();
        _city.foodResourcesPerTurn = _packet.ReadInt();
        _city.moneyResourcesPerTurn = _packet.ReadInt();
        _city.populationResourcesPerTurn = _packet.ReadInt();
        _city.isBeingConquered = _packet.ReadBool();
        _city.isConstructingBuilding = _packet.ReadBool();
        _city.occupyingObjectId = _packet.ReadInt();
        _city.xIndex = _packet.ReadInt();
        _city.zIndex = _packet.ReadInt();
        _city.level = _packet.ReadInt();
        _city.experience = _packet.ReadInt();
        _city.experienceToNextLevel = _packet.ReadInt();
        string _command = _packet.ReadString();

        // Add data to dictionary to be used when displaying past moves
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    #endregion

    /// <summary>
    /// Start player turn once all data is recieved and all past moves are completed
    /// </summary>
    /// <param name="_packet"></param>
    public static void PlayerStartTurn(Packet _packet)
    {
        if (!(GameManagerCS.instance.isAllTroopInfoReceived && GameManagerCS.instance.isAllTileInfoReceived
            && GameManagerCS.instance.isAllCityInfoReceived))
        {
            GameManagerCS.instance.WaitAndStartTurn(_packet);       // If all data is not recieved wait and try again
            return;
        }
        GameManagerCS.instance.isTurn = true;
        GameManagerCS.instance.isAllTroopInfoReceived = false;
        GameManagerCS.instance.isAllTileInfoReceived = false;
        GameManagerCS.instance.isAllCityInfoReceived = false;
        GameManagerCS.instance.currentTroopIndex = _packet.ReadInt();
        GameManagerCS.instance.currentCityIndex = _packet.ReadInt();
        GameManagerCS.instance.turnCount = _packet.ReadInt();

        GameManagerCS.instance.PlayPastMoves();
    }
}