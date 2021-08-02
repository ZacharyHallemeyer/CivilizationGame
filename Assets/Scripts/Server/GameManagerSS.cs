using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSS : MonoBehaviour
{
    public static GameManagerSS instance;

    public int currentTroopId = 0, currentCityId = 0;
    public int currentPlayerTurnId = 0;
    public int turnCount = 1;

    public List<int> playerIds;

    public bool isAllTroopInfoReceived = false, isAllTileInfoReceived = false, isAllCityInfoReceived = false;
    public List<Dictionary<TroopInfo, string>> modifiedTroopInfo = new List<Dictionary<TroopInfo, string>>();
    public List<Dictionary<TileInfo, string>> modifiedTileInfo = new List<Dictionary<TileInfo, string>>();
    public List<Dictionary<CityInfo, string>> modifiedCityInfo = new List<Dictionary<CityInfo, string>>();

    // Set instance or destroy if instance already exist
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

    /// <summary>
    /// Creates player turn order in the form of a list
    /// </summary>
    public void InitPlayerTurnList()
    {
        foreach(ClientSS _client in ClientSS.allClients.Values)
        {
            playerIds.Add(_client.id);
        }
    }

    /// <summary>
    /// Wait and call end turn
    /// </summary>
    /// <param name="_fromClient"> client that data is being recieved from </param>
    /// <param name="_packet"> Packet sent from client </param>
    public void WaitAndEndTurn(int _fromClient, Packet _packet)
    {
        StartCoroutine(WaitAndCallEndTurn(_fromClient, _packet));
    }

    private IEnumerator WaitAndCallEndTurn(int _fromClient, Packet _packet)
    {
        yield return new WaitForSeconds(.1f);
        ServerHandle.EndTurn(_fromClient, _packet);
    }

    /// <summary>
    /// Remove Troop info from GameManager instance
    /// </summary>
    /// <param name="_troop"> troop to remove </param>
    public void RemoveModifiedTroop(TroopInfo _troop)
    {
        Destroy(_troop);
    }

    /// <summary>
    /// Remove Tile info from GameManager instance
    /// </summary>
    /// <param name="_tile"> tile to remove </param>
    public void RemoveModifiedTile(TileInfo _tile)
    {
        Destroy(_tile);
    }

    /// <summary>
    /// Remove City info from GameManager instance
    /// </summary>
    /// <param name="_city"> city to remove </param>
    public void RemoveModifiedCity(CityInfo _city)
    {

    }
}
