using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that allows player to change their tribe
/// </summary>
public class LobbyRow : MonoBehaviour
{
    public void CallLobbyIncrementTribe()
    {
        ClientCS.instance.lobby.IncrementTribeList();
    }

    public void CallLobbyDecrementTribe()
    {
        ClientCS.instance.lobby.DecrementTribeList();
    }
}
