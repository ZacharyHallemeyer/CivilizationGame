﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSS : MonoBehaviour
{
    public int id;
    public string username;

    public int troopsKilled;
    public int ownedTroopsKilled;
    public int citiesOwned;

    public void InitPlayer(int _id, string _username)
    {
        id = _id;
        username = _username;
    }
}
