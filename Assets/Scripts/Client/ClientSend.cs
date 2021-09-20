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

    public static void UpdateTribe(string _oldTribe, string _newTribe)
    {
        using (Packet _packet = new Packet((int)ClientPackets.changeTribe))
        {
            _packet.Write(_oldTribe);
            _packet.Write(_newTribe);

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
    public static void SendEndOfTurnData(bool _isKingAlive)
    {
        // Send all modfified troop info to Server
        foreach (Dictionary<TroopInfo, string> _troopDict in GameManagerCS.instance.modifiedTroopInfo)
        {

            foreach(TroopInfo _troop in _troopDict.Keys)
            {
                // Check which command and send data appropriately
                if (_troopDict[_troop] == "Spawn")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendSpawnTroopInfo))
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

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Move")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendMoveTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.xIndex);
                        _packet.Write(_troop.zIndex);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Rotate")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendRotateTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.rotation);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Attack")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendAttackTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.lastTroopAttackedId);
                        _packet.Write(_troop.attackRotation);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Hurt")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendHurtTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.health);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Die")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendDieTroopInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "SwitchModel")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendSwitchLandOrSeaModelInfo))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.isBoat);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "ChangeShipModel")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendChangeShipModel))
                    {
                        _packet.Write(_troop.id);
                        _packet.Write(_troop.shipName);
                        _packet.Write(_troopDict[_troop]);

                        SendTCPData(_packet);
                    }
                }
                else if (_troopDict[_troop] == "Update")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedTroopInfo))
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

                        SendTCPData(_packet);
                    }
                }
            }
        }
        // Write -1 for id so server knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedTroopInfo))
        {
            _packet.Write(-1);
            SendTCPData(_packet);
        }

        // Send all modfified tile info to Server
        foreach (Dictionary<TileInfo, string> _tileDict in GameManagerCS.instance.modifiedTileInfo)
        {
            foreach(TileInfo _tile in _tileDict.Keys)
            {
                // Check which command and send data appropriately
                if (_tileDict[_tile] == "OccupyChange")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendOccupyChangeTileInfo))
                    {
                        _packet.Write(_tile.id);
                        _packet.Write(_tile.xIndex);
                        _packet.Write(_tile.zIndex);
                        _packet.Write(_tile.isOccupied);
                        _packet.Write(_tile.occupyingObjectId);
                        _packet.Write(_tileDict[_tile]);

                        SendTCPData(_packet);
                    }
                }
                else if (_tileDict[_tile] == "OwnershipChange")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendOwnershipChangeTileInfo))
                    {
                        _packet.Write(_tile.id);
                        _packet.Write(_tile.xIndex);
                        _packet.Write(_tile.zIndex);
                        _packet.Write(_tile.ownerId);
                        _packet.Write(_tileDict[_tile]);

                        SendTCPData(_packet);
                    }
                }
                else if (_tileDict[_tile] == "BuildBuilding")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendBuildBuildingTileInfo))
                    {
                        _packet.Write(_tile.id);
                        _packet.Write(_tile.xIndex);
                        _packet.Write(_tile.zIndex);
                        _packet.Write(_tile.isRoad);
                        _packet.Write(_tile.isWall);
                        _packet.Write(_tile.isBuilding);
                        _packet.Write(_tile.buildingName);
                        _packet.Write(_tileDict[_tile]);

                        SendTCPData(_packet);
                    }
                }
                else if (_tileDict[_tile] == "Update")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedTileInfo))
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

                        SendTCPData(_packet);
                    }
                }
            }
        }
        // Write -1 for id so server knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedTileInfo))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        // Send all modfified city info to Server
        foreach (Dictionary<CityInfo, string> _cityDict in GameManagerCS.instance.modifiedCityInfo)
        {
            foreach (CityInfo _city in _cityDict.Keys)
            {
                // Check which command and send data appropriately
                if (_cityDict[_city] == "Create")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendCreateCityInfo))
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

                        SendTCPData(_packet);
                    }
                }
                else if(_cityDict[_city] == "LevelUp")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendLevelUpCityInfo))
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

                        SendTCPData(_packet);
                    }
                }
                else if(_cityDict[_city] == "Conquer")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendConqueredCityInfo))
                    {
                        _packet.Write(_city.id);
                        _packet.Write(_city.ownerId);
                        _packet.Write(_cityDict[_city]);

                        SendTCPData(_packet);
                    }
                }
                else if(_cityDict[_city] == "Update")
                {
                    using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedCityInfo))
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

                        SendTCPData(_packet);
                    }
                }
            }
        }
        // Write -1 for id so server knows when all data has been recieved
        using (Packet _packet = new Packet((int)ClientPackets.sendUpdatedCityInfo))
        {
            _packet.Write(-1);

            SendTCPData(_packet);
        }

        EndTurn(_isKingAlive);
    }

    /// <summary>
    /// Tells the server to end turn for current player
    /// </summary>
    public static void EndTurn(bool _isKingAlive)
    {
        using (Packet _packet = new Packet((int)ClientPackets.endTurn))
        {
            _packet.Write(GameManagerCS.instance.currentTroopIndex);
            _packet.Write(GameManagerCS.instance.currentCityIndex);
            _packet.Write(_isKingAlive);

            SendTCPData(_packet);
        }
    }

    #endregion
}