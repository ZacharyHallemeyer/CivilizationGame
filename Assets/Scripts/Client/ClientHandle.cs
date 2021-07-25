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

        ClientCS.allClients.Add(_clientId, _clientUsername);
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
        GameManagerCS.instance.tiles = new TileInfo[_xSizeOfTiles, _zSizeOfTiles];

        // Turn off lobby UI if it has not already
        if (ClientCS.instance.lobby.lobbyParent.activeInHierarchy)
            ClientCS.instance.lobby.lobbyParent.SetActive(false);
        GameManagerCS.instance.SpawnPlayer(_id, _username);

    }
    
    /// <summary>
    /// Recieve new tile info from client and call spawn tile function
    /// </summary>
    /// <param name="_packet"> Packet containing new tile info </param>
    public static void CreateNewTile(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _ownerId = _packet.ReadInt();
        int _movementCost = _packet.ReadInt();
        int _occupyingObjectId = _packet.ReadInt();
        string _biome = _packet.ReadString();
        float _temperature = _packet.ReadFloat();
        float _height = _packet.ReadFloat();
        bool _isWater = _packet.ReadBool();
        bool _isFood = _packet.ReadBool();
        bool _isWood = _packet.ReadBool();
        bool _isMetal = _packet.ReadBool();
        bool _isRoad = _packet.ReadBool();
        bool _isCity = _packet.ReadBool();
        bool _isOccupied = _packet.ReadBool();
        Vector2 _position = _packet.ReadVector2();
        int _xIndex = _packet.ReadInt();
        int _zIndex = _packet.ReadInt();
        string _name = "ClientTile " + _xIndex + " " + _zIndex;

        GameManagerCS.instance.CreateNewTile(_id, _ownerId, _movementCost, _occupyingObjectId, _biome, _temperature,
                                            _height, _isWater, _isFood, _isWood, _isMetal, _isRoad, _isCity, _isOccupied, _position, _xIndex, _zIndex,
                                            _name);
    }

    public static void CreateNeutralCity(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Debug.Log("Recieved city "+_id+" from server");
        int _ownerId = _packet.ReadInt();
        float _morale = _packet.ReadFloat();
        float _education = _packet.ReadFloat();
        int _manPower = _packet.ReadInt();
        int _money = _packet.ReadInt();
        int _metal = _packet.ReadInt();
        int _wood = _packet.ReadInt();
        int _food = _packet.ReadInt();
        int _ownerShipRange = _packet.ReadInt();
        int _woodResourcePerTurn = _packet.ReadInt();
        int _metalResourcePerTurn = _packet.ReadInt();
        int _foodResourcePerTurn = _packet.ReadInt();
        int  _xIndex = _packet.ReadInt();
        int  _zIndex = _packet.ReadInt();

        GameManagerCS.instance.CreateNewNeutralCity(_id, _ownerId, _morale, _education, _manPower, _money, _metal, _wood, _food,
                                                    _ownerShipRange, _woodResourcePerTurn, _metalResourcePerTurn, _foodResourcePerTurn,
                                                    _xIndex, _zIndex);

    }

    /// <summary>
    /// Recieve modified troop data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified troop info </param>
    public static void RecieveModifiedTroopInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if(_id == -1)       // If the id is equal to -1 then all data has been recieved
        {
            GameManagerCS.instance.isAllTroopInfoReceived = true;
            return;
        }
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        _troop.id = _id;
        _troop.ownerId = _packet.ReadInt();
        _troop.xCoord = _packet.ReadInt();
        _troop.zCoord = _packet.ReadInt();
        _troop.rotation = _packet.ReadInt();
        _troop.health = _packet.ReadInt();
        _troop.baseAttack = _packet.ReadInt();
        _troop.stealthAttack = _packet.ReadInt();
        _troop.counterAttack = _packet.ReadInt();
        _troop.baseDefense = _packet.ReadInt();
        _troop.facingDefense = _packet.ReadInt();
        _troop.movementCost = _packet.ReadInt();
        _troop.attackRange = _packet.ReadInt();
        _troop.seeRange = _packet.ReadInt();
        _troop.lastTroopAttackedId = _packet.ReadInt();
        _troop.lastHurtById = _packet.ReadInt();
        _troop.canMoveNextTurn = _packet.ReadBool();
        _troop.canMultyKill= _packet.ReadBool();
        string _command = _packet.ReadString();

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);  

    }

    /// <summary>
    /// Recieve modified tile data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified tile info </param>
    public static void RecieveModifiedTileInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        //Debug.Log("Recieved tile " + _id + " from server");
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
        _tile.isOccupied = _packet.ReadBool();
        _tile.occupyingObjectId = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.yIndex = _packet.ReadInt();
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        // Add data to dictionary to be used when displaying past moves
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);  
    }

    /// <summary>
    /// Recieve modified city data from server
    /// </summary>
    /// <param name="_packet"> Packet containing modified city info </param>
    public static void RecieveModifiedCityInfo(Packet _packet)
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
        _city.morale = _packet.ReadFloat();
        _city.education = _packet.ReadFloat();
        _city.manPower = _packet.ReadInt();
        _city.money = _packet.ReadInt();
        _city.metal = _packet.ReadInt();
        _city.wood = _packet.ReadInt();
        _city.food = _packet.ReadInt();
        _city.ownerShipRange = _packet.ReadInt();
        _city.woodResourcesPerTurn = _packet.ReadInt();
        _city.metalResourcesPerTurn = _packet.ReadInt();
        _city.foodResourcesPerTurn = _packet.ReadInt();
        _city.isBeingConquered = _packet.ReadBool();
        _city.isConstructingBuilding = _packet.ReadBool();
        _city.occupyingObjectId = _packet.ReadInt();
        _city.xIndex = _packet.ReadInt();
        _city.zIndex = _packet.ReadInt();
        string _command = _packet.ReadString();

        // Add data to dictionary to be used when displaying past moves
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

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
        GameManagerCS.instance.isAllTroopInfoReceived = false;
        GameManagerCS.instance.isAllTileInfoReceived = false;
        GameManagerCS.instance.isAllCityInfoReceived = false;
        GameManagerCS.instance.currentTroopIndex = _packet.ReadInt();
        GameManagerCS.instance.currentCityIndex = _packet.ReadInt();

        GameManagerCS.instance.PlayPastMoves();
    }
}
