using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientCS.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientCS.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>
    /// Send server client id and client username
    /// </summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(ClientCS.instance.myId);
            _packet.Write(PlayerPrefs.GetString("Username", "Null"));

            SendTCPData(_packet);
        }
    }

    public static void StartGame(string _sceneName)
    {
        using ( Packet _packet = new Packet((int)ClientPackets.startGame))
        {
            _packet.Write(_sceneName);

            SendTCPData(_packet);
        }
    }

    /// <summary>
    /// Sends all end of turn data to server. This includes modified troop, tile, and city info
    /// </summary>
    public static void SendEndOfTurnData()
    {
        // Send all modfified troop info to Server
        foreach (Dictionary<TroopInfo, string> _troopDict in GameManagerCS.instance.modifiedTroopInfo)
        {
            foreach(TroopInfo _troop in _troopDict.Keys)
            {
                using (Packet _packet = new Packet((int)ClientPackets.endTurnTroopData))
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
                    _packet.Write(_troop.lastTroopAttackedId);
                    _packet.Write(_troop.lastHurtById);
                    _packet.Write(_troop.canMoveNextTurn);
                    _packet.Write(_troop.canMultyKill);
                    _packet.Write(_troopDict[_troop]);

                    SendTCPData(_packet);
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.endTurnTroopData))
        {
            _packet.Write(-1);
            SendTCPData(_packet);
        }

        // Send all modfified tile info to Server
        foreach (Dictionary<TileInfo, string> _tileDict in GameManagerCS.instance.modifiedTileInfo)
        {
            foreach(TileInfo _tile in _tileDict.Keys)
            {
                //Debug.Log("Sending Tile " + _tile.id+ " to server");
                using (Packet _packet = new Packet((int)ClientPackets.endTurnTileData))
                {
                    _packet.Write(_tile.id);
                    _packet.Write(_tile.ownerId);
                    _packet.Write(_tile.isRoad);
                    _packet.Write(_tile.isCity);
                    _packet.Write(_tile.isOccupied);
                    _packet.Write(_tile.occupyingObjectId);
                    _packet.Write(_tile.xIndex);
                    _packet.Write(_tile.yIndex);
                    _packet.Write(_tileDict[_tile]);

                    SendTCPData(_packet);
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.endTurnTileData))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        // Send all modfified city info to Server
        foreach (Dictionary<CityInfo, string> _cityDict in GameManagerCS.instance.modifiedCityInfo)
        {
            foreach (CityInfo _city in _cityDict.Keys)
            {
                using (Packet _packet = new Packet((int)ClientPackets.endTurnCityData))
                {
                    _packet.Write(_city.id);
                    _packet.Write(_city.ownerId);
                    _packet.Write(_city.morale);
                    _packet.Write(_city.education);
                    _packet.Write(_city.manPower);
                    _packet.Write(_city.money);
                    _packet.Write(_city.metal);
                    _packet.Write(_city.wood);
                    _packet.Write(_city.food);
                    _packet.Write(_city.ownerShipRange);
                    _packet.Write(_city.woodResourcesPerTurn);
                    _packet.Write(_city.metalResourcesPerTurn);
                    _packet.Write(_city.foodResourcesPerTurn);
                    _packet.Write(_city.isBeingConquered);
                    _packet.Write(_city.isOccupied);
                    _packet.Write(_city.isConstructingBuilding);
                    _packet.Write(_city.occupyingObjectId);
                    _packet.Write(_city.xIndex);
                    _packet.Write(_city.zIndex);
                    _packet.Write(_cityDict[_city]);

                    SendTCPData(_packet);
                }
            }
        }
        // Write -1 for id so client knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.endTurnCityData))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        // TODO City and tile info
        EndTurn();
    }

    /// <summary>
    /// Tells the server to end turn for current player
    /// </summary>
    public static void EndTurn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.endTurn))
        {
            _packet.Write(GameManagerCS.instance.currentTroopIndex);
            _packet.Write(GameManagerCS.instance.currentCityIndex);

            SendTCPData(_packet);
        }
    }

    #endregion
}