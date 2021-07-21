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

    public static void PlayerStartTurn(Packet _packet)
    {
        GameManagerCS.instance.currentTroopIndex = _packet.ReadInt();

        PlayerCS.instance.enabled = true;
    }
}
