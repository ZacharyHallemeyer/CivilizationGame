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

        //Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
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
        GameManagerSS.instance.InitPlayerTurnList();
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

    /// <summary>
    /// Recieve all modified tile data from client and store in a dictionary
    /// </summary>
    /// <param name="_fromClient"> Id of client sending this data </param>
    /// <param name="_packet"> Packet from client </param>
    public static void RecieveTroopInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        if(_id == -1)       // If the id is equal to -1 then all data has been recieved
        {
            GameManagerSS.instance.isAllTroopInfoReceived = true;
            return;
        }

        TroopInfo _troop = GameManagerSS.instance.gameObject.AddComponent<TroopInfo>();

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
        _troop.movementCost = _packet.ReadInt();
        _troop.attackRange = _packet.ReadInt();
        _troop.seeRange = _packet.ReadInt();
        _troop.lastTroopAttackedId = _packet.ReadInt();
        _troop.lastHurtById = _packet.ReadInt();
        _troop.canMoveNextTurn = _packet.ReadBool();
        _troop.canMultyKill = _packet.ReadBool();
        string _command = _packet.ReadString();
        _troop.idOfPlayerThatSentInfo = _fromClient;

        Dictionary<TroopInfo, string> _troopData = new Dictionary<TroopInfo, string>()
            { {_troop, _command} };
        GameManagerSS.instance.modifiedTroopInfo.Add(_troopData);    // Add data to dictionary that will be sent to all clients
    }

    /// <summary>
    /// Recieve all modified tile data from client and store in a dictionary
    /// </summary>
    /// <param name="_fromClient"> Id of client sending this data </param>
    /// <param name="_packet"> Packet from client </param>
    public static void RecieveTileInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        //Debug.Log("Recieved tile " + _id + " from client");
        if (_id == -1)          // If the id is equal to -1 then all data has been recieved
        {
            GameManagerSS.instance.isAllTileInfoReceived = true;
            return;
        }

        TileInfo _tile = GameManagerSS.instance.gameObject.AddComponent<TileInfo>();

        _tile.id = _id;
        _tile.ownerId = _packet.ReadInt();
        _tile.isRoad = _packet.ReadBool();
        _tile.isCity = _packet.ReadBool();
        _tile.isBuilding = _packet.ReadBool();
        _tile.isOccupied = _packet.ReadBool();
        _tile.occupyingObjectId = _packet.ReadInt();
        _tile.xIndex = _packet.ReadInt();
        _tile.zIndex = _packet.ReadInt();
        _tile.cityId = _packet.ReadInt();
        _tile.buildingName = _packet.ReadString();
        string _command = _packet.ReadString();
        _tile.idOfPlayerThatSentInfo = _fromClient;

        Dictionary<TileInfo, string> _tileData = new Dictionary<TileInfo, string>()
            { {_tile, _command} };
        GameManagerSS.instance.modifiedTileInfo.Add(_tileData);     // Add data to dictionary that will be sent to all clients
    }

    /// <summary>
    /// Recieve all modified city data from client and store in a dictionary
    /// </summary>
    /// <param name="_fromClient"> client id that this data is being sent from </param>
    /// <param name="_packet"> Packet from client </param>
    public static void RecieveCityInfo(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (_id == -1)      // If the id is equal to -1 then all data has been recieved
        {
            GameManagerSS.instance.isAllCityInfoReceived = true;
            return;
        }
        CityInfo _city = GameManagerSS.instance.gameObject.AddComponent<CityInfo>();

        _city.id = _id;
        _city.ownerId = _packet.ReadInt();
        _city.morale = _packet.ReadFloat();
        _city.education = _packet.ReadFloat();
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
        _city.experienceToNextLevel = _packet.ReadInt();
        _city.experienceToNextLevel = _packet.ReadInt();
        string _command = _packet.ReadString();
        _city.idOfPlayerThatSentInfo = _fromClient;

        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
            { {_city, _command} };
        GameManagerSS.instance.modifiedCityInfo.Add(_cityData);     // Add data to dictionary that will be sent to all clients
    }

    /// <summary>
    /// End turn for current player and start turn for new player after all data have been recieved from client.
    /// If server has not recieved all data then wait .1 seconds
    /// </summary>
    /// <param name="_fromClient"> Client id that wants to end turn </param>
    /// <param name="_packet"> Packet from client </param>
    public static void EndTurn(int _fromClient, Packet _packet)
    {
        if(!(GameManagerSS.instance.isAllTroopInfoReceived && GameManagerSS.instance.isAllTileInfoReceived 
            && GameManagerSS.instance.isAllCityInfoReceived))
        {
            GameManagerSS.instance.WaitAndEndTurn(_fromClient, _packet);    // Wait if all data has not been recieved
            return;
        }
        GameManagerSS.instance.isAllTroopInfoReceived = false;
        GameManagerSS.instance.isAllTileInfoReceived = false;
        GameManagerSS.instance.isAllCityInfoReceived = false;

        // Chose next player to start turn
        if (GameManagerSS.instance.currentPlayerTurnId + 1 >= GameManagerSS.instance.playerIds.Count)
        {
            GameManagerSS.instance.currentPlayerTurnId = 0;
            GameManagerSS.instance.turnCount++;
        }
        else
            GameManagerSS.instance.currentPlayerTurnId++;

        GameManagerSS.instance.currentTroopId = _packet.ReadInt();
        GameManagerSS.instance.currentCityId = _packet.ReadInt();

        // Start turn for next player
        ServerSend.SendModifiedTroop(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedTile(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.SendModifiedCity(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
        ServerSend.PlayerStartTurn(GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId]);
    }
}