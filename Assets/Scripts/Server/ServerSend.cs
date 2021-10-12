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
            _packet.Write(_client.tribe);

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

    public static void SendUpdatedTribeChoice(int _clientToBeUpdated)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updateTribeChoice))
        {
            _packet.Write(_clientToBeUpdated);
            _packet.Write(Server.clients[_clientToBeUpdated].tribe);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SendAvaliableTribes(List<string> tribes)
    {
        // Sort tribes alphabetically to ensure consistency
        tribes.Sort();
        using (Packet _packet = new Packet((int)ServerPackets.sendTribes))
        {
            _packet.Write(tribes.Count);
            for (int i = 0; i < tribes.Count; i++)
                _packet.Write(tribes[i]);

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
            _packet.Write(Server.clients[_toClient].tribe);

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
                    _packet.Write(_tile.occupyingObjectId);
                    _packet.Write(_tile.biome);
                    _packet.Write(_tile.temperature);
                    _packet.Write(_tile.height);
                    _packet.Write(_tile.isWater);
                    _packet.Write(_tile.isFood);
                    _packet.Write(_tile.isWood);
                    _packet.Write(_tile.isMetal);
                    _packet.Write(_tile.isRoad);
                    _packet.Write(_tile.isWall);
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
    /// Sends all other player stats to certain player
    /// </summary>
    /// <param name="_playerId"> Player to send player stats </param>
    public static void SendPlayerStats(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendClientPlayerStats))
        {
            _packet.Write(ClientSS.allClients.Count - 1);
            foreach (ClientSS _client in ClientSS.allClients.Values)
            {
                if (_client.id != _playerId)
                {
                    _packet.Write(_client.id);
                    _packet.Write(_client.player.troopsKilled);
                    _packet.Write(_client.player.ownedTroopsKilled);
                    _packet.Write(_client.player.citiesOwned);
                }
            }

            SendTCPData(_playerId, _packet);
        }
    }

    /// <summary>
    /// Send all modified troop data to new player
    /// </summary>
    /// <param name="_playerId"> client id that this data is being sent to </param>
    public static void SendModifiedTroop(int _playerId)
    {
        List<Dictionary<TroopInfo, string>> _itemsToRemove = new List<Dictionary<TroopInfo, string>>();
        List<TroopInfo> _itemsToDestroy = new List<TroopInfo>();
        foreach (Dictionary<TroopInfo, string> _troopDict in GameManagerSS.instance.modifiedTroopInfo)
        {
            foreach (TroopInfo _troop in _troopDict.Keys)
            {

                // Remove troop from list and remove component once troop info has been sent to call clients
                if (_troop.idOfPlayerThatSentInfo == GameManagerSS.instance.playerIds[GameManagerSS.instance.currentPlayerTurnId])
                {
                    _itemsToRemove.Add(_troopDict);
                    _itemsToDestroy.Add(_troop);
                }
                else
                {
                    // Check which command and send data appropriately
                    if (_troopDict[_troop] == "Spawn")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientSpawnTroopInfo))
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
                            _packet.Write(_troop.attackRange);
                            _packet.Write(_troop.seeRange);
                            _packet.Write(_troop.canMultyKill);
                            _packet.Write(_troop.lastTroopAttackedId);
                            _packet.Write(_troop.attackRotation);
                            _packet.Write(_troop.shipName);
                            _packet.Write(_troop.shipAttack);
                            _packet.Write(_troop.shipStealthAttack);
                            _packet.Write(_troop.shipCounterAttack);
                            _packet.Write(_troop.shipBaseDefense);
                            _packet.Write(_troop.shipFacingDefense);
                            _packet.Write(_troop.shipMovementCost);
                            _packet.Write(_troop.shipAttackRange);
                            _packet.Write(_troop.shipSeeRange);
                            _packet.Write(_troop.shipCanMultyKill);
                            _packet.Write(_troop.shipCanMoveAfterKill);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Move")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientMoveTroopInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.xIndex);
                            _packet.Write(_troop.zIndex);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Rotate")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientRotateTroopInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.rotation);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Attack")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientAttackTroopInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.lastTroopAttackedId);
                            _packet.Write(_troop.attackRotation);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Hurt")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientHurtTroopInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.health);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Die")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientDieTroopInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "SwitchModel")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientSwitchLandOrSeaModelInfo))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.isBoat);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "ChangeShipModel")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientChangeShipModel))
                        {
                            _packet.Write(_troop.id);
                            _packet.Write(_troop.shipName);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_troopDict[_troop] == "Update")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedTroopInfo))
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
                            _packet.Write(_troop.attackRange);
                            _packet.Write(_troop.seeRange);
                            _packet.Write(_troop.canMultyKill);
                            _packet.Write(_troop.lastTroopAttackedId);
                            _packet.Write(_troop.attackRotation);
                            _packet.Write(_troop.shipName);
                            _packet.Write(_troop.shipAttack);
                            _packet.Write(_troop.shipStealthAttack);
                            _packet.Write(_troop.shipCounterAttack);
                            _packet.Write(_troop.shipBaseDefense);
                            _packet.Write(_troop.shipFacingDefense);
                            _packet.Write(_troop.shipMovementCost);
                            _packet.Write(_troop.shipAttackRange);
                            _packet.Write(_troop.shipSeeRange);
                            _packet.Write(_troop.shipCanMultyKill);
                            _packet.Write(_troop.shipCanMoveAfterKill);
                            _packet.Write(_troopDict[_troop]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedTroopInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
        // Remove all unnecessary data
        foreach (Dictionary<TroopInfo, string> _itemToRemove in _itemsToRemove)
        {
            GameManagerSS.instance.modifiedTroopInfo.Remove(_itemToRemove);
        }
        foreach (TroopInfo _itemToDestroy in _itemsToDestroy)
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
                    // Check which command and send data appropriately
                    if (_tileDict[_tile] == "OccupyChange")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientOccupyChangeTileInfo))
                        {
                            _packet.Write(_tile.id);
                            _packet.Write(_tile.xIndex);
                            _packet.Write(_tile.zIndex);
                            _packet.Write(_tile.isOccupied);
                            _packet.Write(_tile.occupyingObjectId);
                            _packet.Write(_tileDict[_tile]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_tileDict[_tile] == "OwnershipChange")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientOwnershipChangeTileInfo))
                        {
                            _packet.Write(_tile.id);
                            _packet.Write(_tile.xIndex);
                            _packet.Write(_tile.zIndex);
                            _packet.Write(_tile.ownerId);
                            _packet.Write(_tileDict[_tile]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_tileDict[_tile] == "BuildBuilding")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientBuildBuildingTileInfo))
                        {
                            _packet.Write(_tile.id);
                            _packet.Write(_tile.xIndex);
                            _packet.Write(_tile.zIndex);
                            _packet.Write(_tile.isRoad);
                            _packet.Write(_tile.isWall);
                            _packet.Write(_tile.isBuilding);
                            _packet.Write(_tile.buildingName);
                            _packet.Write(_tileDict[_tile]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_tileDict[_tile] == "BuildRoad")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientBuildRoadTileInfo))
                        {
                            _packet.Write(_tile.id);
                            _packet.Write(_tile.xIndex);
                            _packet.Write(_tile.zIndex);
                            _packet.Write(_tile.isRoad);
                            _packet.Write(_tileDict[_tile]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_tileDict[_tile] == "Update")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedTileInfo))
                        {
                            _packet.Write(_tile.id);
                            _packet.Write(_tile.ownerId);
                            _packet.Write(_tile.isRoad);
                            _packet.Write(_tile.isWall);
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
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedTileInfo))
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
                    // Check which command and send data appropriately
                    if (_cityDict[_city] == "Create")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientCreateCityInfo))
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
                    else if (_cityDict[_city] == "LevelUp")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientLevelUpCityInfo))
                        {
                            _packet.Write(_city.id);
                            _packet.Write(_city.ownerId);
                            _packet.Write(_city.ownerShipRange);
                            _packet.Write(_city.woodResourcesPerTurn);
                            _packet.Write(_city.metalResourcesPerTurn);
                            _packet.Write(_city.foodResourcesPerTurn);
                            _packet.Write(_city.moneyResourcesPerTurn);
                            _packet.Write(_city.populationResourcesPerTurn);
                            _packet.Write(_city.level);
                            _packet.Write(_city.experience);
                            _packet.Write(_city.experienceToNextLevel);
                            _packet.Write(_cityDict[_city]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_cityDict[_city] == "Conquer")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientConqueredCityInfo))
                        {
                            _packet.Write(_city.id);
                            _packet.Write(_city.ownerId);
                            _packet.Write(_cityDict[_city]);

                            SendTCPData(_playerId, _packet);
                        }
                    }
                    else if (_cityDict[_city] == "Update")
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedCityInfo))
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
        }
        using (Packet _packet = new Packet((int)ServerPackets.sendClientUpdatedCityInfo))
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

    /// <summary>
    /// Send to client to have player start their turn
    /// </summary>
    /// <param name="_playerId"> Player to start their turn </param>
    public static void PlayerStartTurn(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startTurn))
        {
            _packet.Write(GameManagerSS.instance.currentTroopId);
            _packet.Write(GameManagerSS.instance.currentCityId);
            _packet.Write(GameManagerSS.instance.turnCount);
            _packet.Write(GameManagerSS.instance.playerIds.Count);

            SendTCPData(_playerId, _packet);
        }
    }

    public static void EndGame()
    {
        using (Packet _packet = new Packet((int)ServerPackets.endGame))
        {
            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}