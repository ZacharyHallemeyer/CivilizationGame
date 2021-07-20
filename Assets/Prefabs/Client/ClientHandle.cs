using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
    /// <summary>
    /// Connects client to server
    /// </summary>
    /// <param name="_packet">msg and id</param>
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        ClientCS.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        ClientCS.instance.udp.Connect(((IPEndPoint)ClientCS.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary>
    /// Adds client to allClient dictionary and inits lobby with new client
    /// </summary>
    /// <param name="_packet"></param>
    public static void AddClient(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _clientUsername = _packet.ReadString();

        ClientCS.allClients.Add(_clientId, _clientUsername);
        ClientCS.instance.lobby.InitLobbyUI();
    }


    /// <summary>
    /// Recieve which player disconnected from server and remove from dictionries
    /// </summary>
    /// <param name="_packet"> id </param>
    public static void PlayerDisconnected(Packet _packet)
    {
        Debug.Log("Player disconnected called");
        int _id = _packet.ReadInt();

        ClientCS.allClients.Remove(_id);
        ClientCS.instance.lobby.InitLobbyUI();
    }

    public static void WorldCreated(Packet _packet)
    {
        ClientCS.instance.lobby.ToggleStartButtonState();
    }
}
