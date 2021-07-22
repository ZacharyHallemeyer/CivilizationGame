using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void WorldCreated()
    {
        using (Packet _packet = new Packet((int)ServerPackets.worldCreated))
        {
            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnPlayer(int _toClient, PlayerSS _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(WorldGeneratorSS.tiles.GetLength(0));
            _packet.Write(WorldGeneratorSS.tiles.GetLength(1));

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendTileInfo(int _toClient, TileInfo _tileInfo, int _xIndex, int _zIndex)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendTileInfo))
        {
            _packet.Write(_tileInfo.id);
            _packet.Write(_tileInfo.ownerId);
            _packet.Write(_tileInfo.movementCost);
            _packet.Write(_tileInfo.occupyingObjectId);
            _packet.Write(_tileInfo.biome);
            _packet.Write(_tileInfo.temperature);
            _packet.Write(_tileInfo.height);
            _packet.Write(_tileInfo.isWater);
            _packet.Write(_tileInfo.isRoad);
            _packet.Write(_tileInfo.isCity);
            _packet.Write(_tileInfo.isOccupied);
            _packet.Write(_tileInfo.position);
            _packet.Write(_xIndex);
            _packet.Write(_zIndex);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendModifiedTroop(int _playerId)
    {
        foreach(TroopInfo _troop in GameManagerSS.instance.modifiedTroopInfo)
        {
            // Remove troop from list and remove component once troop info has been sent to call clients
            if(_troop.ownerId == ClientCS.instance.myId)
            {
                GameManagerSS.instance.modifiedTroopInfo.Remove(_troop);
                GameManagerSS.instance.RemoveModifiedTroop(_troop);
                break;
            }
            else
            {
                using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTroopInfo))
                {
                    _packet.Write(_troop.id);
                    _packet.Write(_troop.ownerId);
                    _packet.Write(_troop.xCoord);
                    _packet.Write(_troop.zCoord);
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

                    SendTCPData(_playerId, _packet);
                }
            }

        }
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTroopInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
    }

    public static void SendModifiedTile(int _playerId)
    {
        foreach (TileInfo _tile in GameManagerSS.instance.modifiedTileInfo)
        {
            // Remove troop from list and remove component once troop info has been sent to call clients
            if (_tile.ownerId == ClientCS.instance.myId)
            {
                GameManagerSS.instance.modifiedTileInfo.Remove(_tile);
                GameManagerSS.instance.RemoveModifiedTile(_tile);
                break;
            }
            else
            {
                using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTileInfo))
                {
                    _packet.Write(_tile.id);
                    _packet.Write(_tile.ownerId);
                    _packet.Write(_tile.isRoad);
                    _packet.Write(_tile.isCity);
                    _packet.Write(_tile.isOccupied);
                    _packet.Write(_tile.occupyingObjectId);

                    SendTCPData(_playerId, _packet);
                }
            }

        }
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedTileInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
    }

    public static void SendModifiedCity(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendModifiedCityInfo))
        {
            _packet.Write(-1);

            SendTCPData(_playerId, _packet);
        }
    }

    public static void PlayerStartTurn(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startTurn))
        {
            _packet.Write(GameManagerSS.instance.currentTroopId);

            SendTCPData(_playerId, _packet);
        }
    }
    #endregion
}