using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
