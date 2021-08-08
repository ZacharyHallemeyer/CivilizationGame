using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_client">The new Client</param>
    public static void SendNewClient(int _toClient, ClientSS _client)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addClient))
        {
            _packet.Write(_client.id);
            _packet.Write(_client.userName);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Tells all clients that a client has disconnected
    /// </summary>
    /// <param name="_playerId"> The player's id that disconnected</param>
    public static void PlayerDisconnect(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Tells host cient when world has been fully created 
    /// </summary>
    public static void WorldCreated()
    {
        using (Packet _packet = new Packet((int)ServerPackets.worldCreated))
        {
            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Spawn player controller on client side
    /// </summary>
    /// <param name="_toClient"> client to sent player data to </param>
    /// <param name="_player"> player data needed to send to client </param>
    public static void SpawnPlayer(int _toClient, PlayerSS _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(WorldGeneratorSS.instance.tiles.GetLength(0));
            _packet.Write(WorldGeneratorSS.instance.tiles.GetLength(1));

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Send all tile data to client at start of game
    /// </summary>
    /// <param name="_toClient"> client id to send tile data </param>
    /// <param name="_tileInfo"> the tile info to send </param>
    public static void SendTileInfo(int _toClient, TileInfo[,] _tiles)
    {
        for (int x = 0; x < _tiles.GetLength(0); x++)
        {
            for (int z = 0; z < _tiles.GetLength(1); z++)
            {
                using (Packet _packet = new Packet((int)ServerPackets.sendTileInfo))
                {
                    TileInfo _tile = _tiles[x, z];
                    _packet.Write(_tile.id);
                    _packet.Write(_tile.ownerId);
                    _packet.Write(_tile.movementCost);
                    _packet.Write(_tile.occupyingObjectId);
                    _packet.Write(_tile.biome);
                    _packet.Write(_tile.temperature);
                    _packet.Write(_tile.height);
                    _packet.Write(_tile.isWater);
                    _packet.Write(_tile.isFood);
                    _packet.Write(_tile.isWood);
                    _packet.Write(_tile.isMetal);
                    _packet.Write(_tile.isRoad);
                    _packet.Write(_tile.isCity);
                    _packet.Write(_tile.isOccupied);
                    _packet.Write(_tile.isObstacle);
                    _packet.Write(_tile.position);
                    _packet.Write(_tile.xIndex);
                    _packet.Write(_tile.zIndex);
                    _packet.Write(_tile.cityId);

                    SendTCPData(_toClient, _packet);
                }
            }
        }
        using (Packet _packet = new Packet((int)ServerPackets.sendTileInfo))
        {
            _packet.Write(-1);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Send all neutral city data to client at start of game
    /// </summary>
    /// <param name="_toClient"> client id to send city data </param>
    /// <param name="_cityInfo"> the city data to send </param>
    public static void SendNeutralCityInfo(int _toClient, List<CityInfo> _neutralCities)
    {
        for(int i = 0; i < _neutralCities.Count; i++)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendNeutralCityInfo))
            {
                CityInfo _city = _neutralCities[i];
                _packet.Write(_city.id);
                _packet.Write(_city.ownerId);
                _packet.Write(_city.morale);
                _packet.Write(_city.education);
                _packet.Write(_city.ownerShipRange);
                _packet.Write(_city.woodResourcesPerTurn);
                _packet.Write(_city.metalResourcesPerTurn);
                _packet.Write(_city.foodResourcesPerTurn);
                _packet.Write(_city.moneyResourcesPerTurn);
                _packet.Write(_city.populationResourcesPerTurn);
                _packet.Write(_city.xIndex);
                _packet.Write(_city.zIndex);
                _packet.Write(_city.level);

                SendTCPData(_toClient, _packet);
            }
        }
        using (Packet _packet = new Packet((int)ServerPackets.sendNeutralCityInfo))
        {
            _packet.Write(-1);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Send all modified troop data to new player
    /// </summary>
    /// <param name="_playerId"> client id that this data is being sent to </param>
    public static void SendModifiedTroop(int _playerId)
    {
        List<Dictionary<TroopInfo, string>> _itemsToRemove = new List<Dictionary<TroopInfo, string>>();
        List<TroopInfo> _itemsToDestroy = new  List<TroopInfo>();
        foreach (Dictionary<TroopInfo, string> _troopDict in GameManagerSS.instance.modifiedTroopInfo)
        {
            foreach(TroopInfo _troop in _troopDict.Keys)
            {
                // Remove troop from list and remove component once troop info has been sent to call clients
                if(_troop.idOfPlayerThatSentInfo == GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId])
                {
                    _itemsToRemove.Add(_troopDict);
                    _itemsToDestroy.Add(_troop);
                }
                else
                {
                    using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.ownerId);
                        _packet.Write(_troop.troopName);
                        _packet.Write(_troop.xIndex);
                        _packet.Write(_troop.zIndex);
                        _packet.Write(_troop.rotation);
                        _packet.Write(_troop.health);
                        _packet.Write(_troop.baseAttack);
                        _packet.Write(_troop.stealthAttack);
                        _packet.Write(_troop.counterAttack);
                        _packet.Write(_troop.baseDefense);
                        _packet.Write(_troop.facingDefense);
                        _packet.Write(_troop.movementCost);
                        _packet.Write(_troop.attackRange);
                        _packet.Write(_troop.seeRange);
                        _packet.Write(_troop.lastHurtById);
                        _packet.Write(_troop.canMoveNextTurn);
                        _packet.Write(_troop.canMultyKill);
                        _packet.Write(_troop.lastTroopAttackedId);
                        _packet.Write(_troop.attackRotation);
                        _packet.Write(_troopDict[_troop]);
                        SendTCPData(_playerId, _packet);
                    }
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTroopInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
        // Remove all unnecessary data
        foreach (Dictionary<TroopInfo, string> _itemToRemove in _itemsToRemove)
        {
            GameManagerSS.instance.modifiedTroopInfo.Remove(_itemToRemove);
        }
        foreach(TroopInfo _itemToDestroy in _itemsToDestroy)
        {
            GameManagerSS.instance.RemoveModifiedTroop(_itemToDestroy);
        }
    }

    /// <summary>
    /// Send all modified tile data to new player
    /// </summary>
    /// <param name="_playerId"> client id that this data is being sent to </param>
    public static void SendModifiedTile(int _playerId)
    {
        List<Dictionary<TileInfo, string>> _itemsToRemove = new List<Dictionary<TileInfo, string>>();
        List<TileInfo> _itemsToDestroy = new List<TileInfo>();
        foreach (Dictionary<TileInfo, string> _tileDict in GameManagerSS.instance.modifiedTileInfo)
        {
            foreach (TileInfo _tile in _tileDict.Keys)
            {
                // Remove troop from list and remove component once troop info has been sent to call clients
                if (_tile.idOfPlayerThatSentInfo == GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId])
                {
                    _itemsToRemove.Add(_tileDict);
                    _itemsToDestroy.Add(_tile);
                }
                else
                {
                    //Debug.Log("Sending tile " + _tile.id + " to client");
                    using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTileInfo))
                    {
                        _packet.Write(_tile.id);
                        _packet.Write(_tile.ownerId);
                        _packet.Write(_tile.isRoad);
                        _packet.Write(_tile.isCity);
                        _packet.Write(_tile.isBuilding);
                        _packet.Write(_tile.isOccupied);
                        _packet.Write(_tile.occupyingObjectId);
                        _packet.Write(_tile.xIndex);
                        _packet.Write(_tile.zIndex);
                        _packet.Write(_tile.cityId);
                        _packet.Write(_tile.buildingName);
                        _packet.Write(_tileDict[_tile]);

                        SendTCPData(_playerId, _packet);
                    }
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTileInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
        // Remove all unnecessary data
        foreach (Dictionary<TileInfo, string> _itemToRemove in _itemsToRemove)
        {
            GameManagerSS.instance.modifiedTileInfo.Remove(_itemToRemove);
        }
        foreach (TileInfo _itemToDestroy in _itemsToDestroy)
        {
            GameManagerSS.instance.RemoveModifiedTile(_itemToDestroy);
        }
    }

    public static void SendModifiedCity(int _playerId)
    {
        List<Dictionary<CityInfo, string>> _itemsToRemove = new List<Dictionary<CityInfo, string>>();
        List<CityInfo> _itemsToDestroy = new List<CityInfo>();
        foreach (Dictionary<CityInfo, string> _cityDict in GameManagerSS.instance.modifiedCityInfo)
        {
            foreach (CityInfo _city in _cityDict.Keys)
            {
                // Remove troop from list and remove component once troop info has been sent to call clients
                if (_city.idOfPlayerThatSentInfo == 
                        GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId])
                {
                    _itemsToRemove.Add(_cityDict);
                    _itemsToDestroy.Add(_city);
                }
                else
                {
                    using (Packet _packet = new Packet((int)ServerPackets.sendModifiedCityInfo))
                    {
                        _packet.Write(_city.id);
                        _packet.Write(_city.ownerId);
                        _packet.Write(_city.morale);
                        _packet.Write(_city.education);
                        _packet.Write(_city.ownerShipRange);
                        _packet.Write(_city.woodResourcesPerTurn);
                        _packet.Write(_city.metalResourcesPerTurn);
                        _packet.Write(_city.foodResourcesPerTurn);
                        _packet.Write(_city.moneyResourcesPerTurn);
                        _packet.Write(_city.populationResourcesPerTurn);
                        _packet.Write(_city.isBeingConquered);
                        _packet.Write(_city.isConstructingBuilding);
                        _packet.Write(_city.occupyingObjectId);
                        _packet.Write(_city.xIndex);
                        _packet.Write(_city.zIndex);
                        _packet.Write(_city.level);
                        _packet.Write(_city.experience);
                        _packet.Write(_city.experienceToNextLevel);
                        _packet.Write(_cityDict[_city]);

                        SendTCPData(_playerId, _packet);
                    }
                }
            }
        }
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedCityInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
        // Remove all unnecessary data
        foreach (Dictionary<CityInfo, string> _itemToRemove in _itemsToRemove)
        {
            GameManagerSS.instance.modifiedCityInfo.Remove(_itemToRemove);
        }
        foreach (CityInfo _itemToDestroy in _itemsToDestroy)
        {
            GameManagerSS.instance.RemoveModifiedCity(_itemToDestroy);
        }
    }

    public static void PlayerStartTurn(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startTurn))
        {
            _packet.Write(GameManagerSS.instance.currentTroopId);
            _packet.Write(GameManagerSS.instance.currentCityId);
            _packet.Write(GameManagerSS.instance.turnCount);

            SendTCPData(_playerId, _packet);
        }
    }
    #endregion
}