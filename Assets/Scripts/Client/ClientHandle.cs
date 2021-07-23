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

        Debug.Log($"Message from server: {_msg}");
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
        Debug.Log("Player disconnected called");
        int _id = _packet.ReadInt();

        ClientCS.allClients.Remove(_id);
        ClientCS.instance.lobby.InitLobbyUI();
    }

    public static void WorldCreated(Packet _packet)
    {
        ClientCS.instance.lobby.ToggleStartButtonState();
    }

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
        bool _isRoad = _packet.ReadBool();
        bool _isCity = _packet.ReadBool();
        bool _isOccupied = _packet.ReadBool();
        Vector2 _position = _packet.ReadVector2();
        int _xIndex = _packet.ReadInt();
        int _zIndex = _packet.ReadInt();
        string _name = "ClientTile " + _xIndex + " " + _zIndex;

        GameManagerCS.instance.CreateNewTile(_id, _ownerId, _movementCost, _occupyingObjectId, _biome, _temperature,
                                            _height, _isWater, _isRoad, _isCity, _isOccupied, _position, _xIndex, _zIndex,
                                            _name);
    }

    public static void RecieveModifiedTroopInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if(_id == -1)
        {
            GameManagerCS.instance.isAllTroopInfoReceived = true;
            return;
        }
        TroopInfo _troop = GameManagerCS.instance.gameObject.AddComponent<TroopInfo>();
        Debug.Log("Recieved troop id: " + _id);
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
        GameManagerCS.instance.modifiedTroopInfo.Add(_troopData);

    }

    public static void RecieveModifiedTileInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)
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
        string _command = _packet.ReadString();

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile.GetComponent<TileInfo>(), _command} };
        GameManagerCS.instance.modifiedTileInfo.Add(_tileData);
    }

    public static void RecieveModifiedCityInfo(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)
        {
            GameManagerCS.instance.isAllCityInfoReceived = true;
            return;
        }
        string _command = _packet.ReadString();

        /*
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city.GetComponent<CityInfo>(), _command} };
        */
    }

    public static void PlayerStartTurn(Packet _packet)
    {
        Debug.Log("Player start turn has been called");
        if (!(GameManagerCS.instance.isAllTroopInfoReceived && GameManagerCS.instance.isAllTileInfoReceived
            && GameManagerCS.instance.isAllCityInfoReceived))
        {
            GameManagerCS.instance.WaitAndStartTurn(_packet);
            return;
        }
        GameManagerCS.instance.isAllTroopInfoReceived = false;
        GameManagerCS.instance.isAllTileInfoReceived = false;
        GameManagerCS.instance.isAllCityInfoReceived = false;
        GameManagerCS.instance.currentTroopIndex = _packet.ReadInt();
        Debug.Log("Index: " + GameManagerCS.instance.currentTroopIndex);

        GameManagerCS.instance.PlayPastMoves();

        //PlayerCS.instance.enabled = true;
    }
}
