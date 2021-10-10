using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayer
{
    public string username;
    public string tribe;

    public int troopsKilled;
    public int ownedTroopsKilled;
    public int citiesOWned;

    public RemotePlayer(string _username, string _tribe)
    {
        username = _username;
        tribe = _tribe;
    }
}
