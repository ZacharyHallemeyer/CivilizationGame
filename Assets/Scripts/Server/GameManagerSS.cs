using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSS : MonoBehaviour
{
    public static GameManagerSS instance;

    public int currentTroopId = 0;
    public int currentPlayerTurnId = 0;

    public List<int> playerIds;
    
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
}
