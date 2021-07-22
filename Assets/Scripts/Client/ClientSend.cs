﻿using System.Collections;
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

    public static void SendEndOfTurnData()
    {
        foreach(TroopInfo _troop in GameManagerCS.instance.modifiedTroopInfo)
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

                SendTCPData(_packet);
            }
        }
        using (Packet _packet = new Packet((int)ClientPackets.endTurnTroopData))
        {
            _packet.Write(-1);
            SendTCPData(_packet);
        }


        foreach (TileInfo _tile in GameManagerCS.instance.modifiedTileInfo)
        {
            using (Packet _packet = new Packet((int)ClientPackets.endTurnTileData))
            {
                _packet.Write(_tile.id);
                _packet.Write(_tile.ownerId);
                _packet.Write(_tile.isRoad);
                _packet.Write(_tile.isCity);
                _packet.Write(_tile.isOccupied);
                _packet.Write(_tile.occupyingObjectId);

                SendTCPData(_packet);
            }
        }
        using (Packet _packet = new Packet((int)ClientPackets.endTurnTileData))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        foreach (CityInfo _city in GameManagerCS.instance.modifiedCityInfo)
        {
            using (Packet _packet = new Packet((int)ClientPackets.endTurnCityData))
            {
                _packet.Write(_city.id);
                _packet.Write(_city.ownerId);
                _packet.Write(_city.isBeingConquered);
                _packet.Write(_city.isOccupied);
                _packet.Write(_city.isConstructingBuilding);
                _packet.Write(_city.isTrainingTroops);
                _packet.Write(_city.morale);
                _packet.Write(_city.education);
                _packet.Write(_city.manPower);
                _packet.Write(_city.money);
                _packet.Write(_city.metal);
                _packet.Write(_city.wood);
                _packet.Write(_city.food);
                _packet.Write(_city.ownerShipRange);

                SendTCPData(_packet);
            }
        }
        using (Packet _packet = new Packet((int)ClientPackets.endTurnCityData))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        // TODO City and tile info

        EndTurn();
    }

    public static void EndTurn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.endTurn))
        {
            _packet.Write(GameManagerCS.instance.currentTroopIndex);

            SendTCPData(_packet);
        }
    }

    #endregion
}