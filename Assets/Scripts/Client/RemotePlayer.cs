using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayer
{
    public string username;
    public string tribe;

    public int id;
    public int troopsKilled;
    public int ownedTroopsKilled;
    public int citiesOwned;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_username"> username of player </param>
    /// <param name="_tribe"> tribe of player </param>
    /// <param name="_clientId"> id of player </param>
    public RemotePlayer(string _username, string _tribe, int _clientId)
    {
        username = _username;
        tribe = _tribe;
        id = _clientId;
    }
}
