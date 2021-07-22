using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

public class ServerHandle
{
    /// <summary>
    /// Send client to lobby
    /// </summary>
    /// <param name="_fromClient"> client that just connected to server </param>
    /// <param name="_packet"> client id and client username </param>
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            return;
        }


        ClientSS _client = new ClientSS(_fromClient);
        _client.userName = _username;
        ClientSS.allClients.Add(_fromClient, _client);
        Server.clients[_fromClient].SendIntoLobby(_username);
    }

    /// <summary>
    /// Sends all client in lobby into game
    /// </summary>
    /// <param name="_fromClient"> client that called this method </param>
    /// <param name="_packet"> game mode name to send clients into </param>
    public static void SendLobbyIntoGame(int _fromClient, Packet _packet)
    {
        GameManagerSS.instance.InitPlayerTurnArray();
        string gameModeName = _packet.ReadString();

        foreach (ClientSS _client in ClientSS.allClients.Values)
        {
            switch (gameModeName)
            {
                case "Domination":
                    _client.SendPlayerIntoGame();
                    break;
                default:
                    break;
            }
        }
        ServerSend.SendModifiedTroop(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedTile(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedCity(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.PlayerStartTurn(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
    }

    public static void RecieveTroopInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        if(_id == -1)
        {
            GameManagerSS.instance.isAllTroopInfoReceived = true;
            return;
        }

        TroopInfo _troop = GameManagerSS.instance.gameObject.AddComponent<TroopInfo>();

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

        GameManagerSS.instance.modifiedTroopInfo.Add(_troop);
    }

    public static void RecieveTileInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)
        {
            GameManagerSS.instance.isAllTileInfoReceived = true;
            return;
        }

        TileInfo _tile = GameManagerSS.instance.gameObject.AddComponent<TileInfo>();

        _tile.id = _id;
        _tile.ownerId = _packet.ReadInt();
        _tile.isRoad = _packet.ReadBool();
        _tile.isCity = _packet.ReadBool();
        _tile.isOccupied = _packet.ReadBool();
        _tile.occupyingObjectId = _packet.ReadInt();

        GameManagerSS.instance.modifiedTileInfo.Add(_tile);
    }

    public static void RecieveCityInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)
        {
            GameManagerSS.instance.isAllCityInfoReceived = true;
            return;
        }
        CityInfo _city = GameManagerSS.instance.gameObject.AddComponent<CityInfo>();

        _city.id = _id;
        _city.ownerId = _packet.ReadInt();
        _city.isBeingConquered = _packet.ReadBool();
        _city.isOccupied = _packet.ReadBool();
        _city.isConstructingBuilding = _packet.ReadBool();
        _city.isTrainingTroops = _packet.ReadBool();
        _city.morale = _packet.ReadFloat();
        _city.education = _packet.ReadFloat();
        _city.manPower = _packet.ReadInt();
        _city.money = _packet.ReadInt();
        _city.metal = _packet.ReadInt();
        _city.wood = _packet.ReadInt();
        _city.food = _packet.ReadInt();
        _city.ownerShipRange = _packet.ReadInt();

        GameManagerSS.instance.modifiedCityInfo.Add(_city);
    }

    public static void EndTurn(int _fromClient, Packet _packet)
    {
        if(!(GameManagerSS.instance.isAllTroopInfoReceived && GameManagerSS.instance.isAllTileInfoReceived 
            && GameManagerSS.instance.isAllCityInfoReceived))
        {
            GameManagerSS.instance.WaitAndEndTurn(_fromClient, _packet);
            return;
        }
        GameManagerSS.instance.isAllTroopInfoReceived = false;
        GameManagerSS.instance.isAllTileInfoReceived = false;
        GameManagerSS.instance.isAllCityInfoReceived = false;

        if (GameManagerSS.instance.currentPlayerTurnId + 1 >= GameManagerSS.instance.playerIds.Count)
            GameManagerSS.instance.currentPlayerTurnId = 0;
        else
            GameManagerSS.instance.currentPlayerTurnId++;

        GameManagerSS.instance.currentTroopId = _packet.ReadInt();

        // Start turn for next player
        ServerSend.SendModifiedTroop(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedTile(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedCity(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.PlayerStartTurn(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
    }
}