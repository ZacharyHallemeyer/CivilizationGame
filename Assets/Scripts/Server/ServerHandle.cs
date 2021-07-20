using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ServerHandle
{
    /// <summary>
    /// Send client to lobby
    /// </summary>
    /// <param name="_fromClient"> client that just connected to server </param>
    /// <param name="_packet"> client id and client username </param>
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            return;
        }
        
        
        ClientSS _client = new ClientSS(_fromClient);
        _client.userName = _username;
        ClientSS.allClients.Add(_fromClient, _client);
        Server.clients[_fromClient].SendPlayerIntoGame();
    }

    /// <summary>
    /// Sends all client in lobby into game
    /// </summary>
    /// <param name="_fromClient"> client that called this method </param>
    /// <param name="_packet"> game mode name to send clients into </param>
    public static void SendLobbyIntoGame(int _fromClient, Packet _packet)
    {
        string gameModeName = _packet.ReadString();

        foreach (ClientSS _client in ClientSS.allClients.Values)
        {
            switch (gameModeName)
            {
                case "Domination":
                    _client.SendPlayerIntoGame();
                    break;
                default:
                    break;
            }
        }
    }

    public static void EndTurn(int _fromClient, Packet _packet)
    {

    }
}