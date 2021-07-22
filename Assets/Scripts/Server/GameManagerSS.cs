using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSS : MonoBehaviour
{
    public static GameManagerSS instance;

    public int currentTroopId = 0;
    public int currentPlayerTurnId = 0;

    public List<int> playerIds;

    public bool isAllTroopInfoReceived = false, isAllTileInfoReceived = false, isAllCityInfoReceived = false;
    public List<Dictionary<TroopInfo, string>> modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
    public List<Dictionary<TileInfo, string>> modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
    public List<Dictionary<CityInfo, string>> modifiedCityInfo = new List<Dictionary<CityInfo, string>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void InitPlayerTurnArray()
    {
        foreach(ClientSS _client in ClientSS.allClients.Values)
        {
            playerIds.Add(_client.id);
        }
    }

    public void WaitAndEndTurn(int _fromClient, Packet _packet)
    {
        StartCoroutine(WaitAndCallEndTurn(_fromClient, _packet));
    }

    private IEnumerator WaitAndCallEndTurn(int _fromClient, Packet _packet)
    {
        yield return new WaitForSeconds(.1f);
        ServerHandle.EndTurn(_fromClient, _packet);
    }

    public void RemoveModifiedTroop(TroopInfo _troop)
    {
        Destroy(_troop);
    }

    public void RemoveModifiedTile(TileInfo _tile)
    {
        Destroy(_tile);
    }

    public void RemoveModifiedCity(TroopInfo _troop)
    {

    }
}
