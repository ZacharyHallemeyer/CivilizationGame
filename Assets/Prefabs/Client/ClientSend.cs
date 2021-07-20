using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientCS.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientCS.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>
    /// Send server client id and client username
    /// </summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(ClientCS.instance.myId);
            _packet.Write(PlayerPrefs.GetString("Username", "Null"));

            SendTCPData(_packet);
        }
    }

    public static void StartGame(string _sceneName)
    {
        using ( Packet _packet = new Packet((int)ClientPackets.startGame))
        {
            _packet.Write(_sceneName);

            SendTCPData(_packet);
        }
    }

    #endregion
}