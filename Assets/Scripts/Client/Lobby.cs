using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;

public class Lobby : MonoBehaviour
{
    public class LobbyRow
    {
        public GameObject lobbyRowGameObject;
        public TextMeshProUGUI username;
        public TextMeshProUGUI tribe;
        public TextMeshProUGUI tribeSkill;
    }

    private int currentTribeIndex;

    public GameObject lobbyParent;
    public GameObject localLobbyRowPrefab;
    public GameObject remoteLobbyRowPrefab;
    public LobbyRow thisClientLobbyRow;
    public LobbyRow[] lobbyRows;

    public TextMeshProUGUI publicIP;
    public TextMeshProUGUI localIP;

    public Button startButton;

    private void Awake()
    {
        startButton.enabled = false;
    }

    /// <summary>
    /// Adds a row of client info for each client
    /// </summary>
    public void InitLobbyUI()
    {
        GameObject _lobbyRow = null;
        float _yPos = 230f;
        if (lobbyRows != null)
        {
            foreach (LobbyRow _existingLobbyRow in lobbyRows)
            {
                Destroy(_existingLobbyRow.lobbyRowGameObject);
            }
        }
        lobbyRows = new LobbyRow[ClientCS.allClients.Count];
        foreach(int _clientId in ClientCS.allClients.Keys)
        {
            if(_clientId == ClientCS.instance.myId)
                _lobbyRow = Instantiate(localLobbyRowPrefab, lobbyParent.transform);
            else
                _lobbyRow = Instantiate(remoteLobbyRowPrefab, lobbyParent.transform);
            _lobbyRow.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, _yPos, 0);

            _yPos -= 75;

            lobbyRows[_clientId - 1] = new LobbyRow();
            lobbyRows[_clientId - 1].lobbyRowGameObject = _lobbyRow;
            lobbyRows[_clientId - 1].username = _lobbyRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            lobbyRows[_clientId - 1].username.text = ClientCS.allClients[_clientId]["Username"];
            lobbyRows[_clientId - 1].tribe = _lobbyRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            lobbyRows[_clientId - 1].tribe.text = ClientCS.allClients[_clientId]["Tribe"];
            lobbyRows[_clientId - 1].tribeSkill = _lobbyRow.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            lobbyRows[_clientId - 1].tribeSkill.text = "Skill: " + Constants.tribeSkills[ClientCS.allClients[_clientId]["Tribe"]];
            if (_clientId == ClientCS.instance.myId)
                thisClientLobbyRow = lobbyRows[_clientId - 1];
        }

        /*
        publicIP.text = GetPublicIPAddress();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(string.Empty);
        localIP.text = Dns.GetHostEntry(string.Empty).AddressList[5].ToString();
        */
    } 

    public void IncrementTribeList()
    {
        currentTribeIndex++;
        if (currentTribeIndex >= GameManagerCS.instance.avaliableTribes.Count)
            currentTribeIndex = 0;

        string _newTribe = GameManagerCS.instance.avaliableTribes[currentTribeIndex];
        ClientSend.UpdateTribe(ClientCS.allClients[ClientCS.instance.myId]["Tribe"], _newTribe);

    }
    public void DecrementTribeList()
    {
        currentTribeIndex--;
        if (currentTribeIndex < 0)
            currentTribeIndex = GameManagerCS.instance.avaliableTribes.Count - 1;

        string _newTribe = GameManagerCS.instance.avaliableTribes[currentTribeIndex];
        ClientSend.UpdateTribe(ClientCS.allClients[ClientCS.instance.myId]["Tribe"], _newTribe);
    }

    static string GetPublicIPAddress()
    {
        string address = "";
        WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
        using (WebResponse response = request.GetResponse())
        using (StreamReader stream = new StreamReader(response.GetResponseStream()))
        {
            address = stream.ReadToEnd();
        }

        int first = address.IndexOf("Address: ") + 9;
        int last = address.LastIndexOf("</body>");
        address = address.Substring(first, last - first);

        return address;
    }

    /// <summary>
    /// Starts current gamemode
    /// </summary>
    public void StartGame()
    {
        string _sceneName = SceneManager.GetActiveScene().name;
        _sceneName = _sceneName.Substring(6);
        ClientSend.StartGame(_sceneName);
    }

    /// <summary>
    /// Unloads current scenes and load main menu
    /// </summary>
    public void ExitGame()
    {
        ClientCS.allClients = new Dictionary<int, Dictionary<string, string>>();
        ClientSS.allClients = new Dictionary<int, ClientSS>();
        // If player is host than close server and network manager
        if (Server.clients.Count > 0)
        {
            Debug.Log(Server.clients.Count > 0);
            Server.clients = new Dictionary<int, ClientSS>();
            Server.Stop();
            Destroy(FindObjectOfType<NetworkManager>().gameObject);
            ClientCS.instance.Disconnect();
        }

        Destroy(FindObjectOfType<ClientCS>().gameObject);
        Destroy(FindObjectOfType<EventSystem>().gameObject);
        SceneManager.LoadScene("ClientMainMenu");
    }

    public void ToggleStartButtonState()
    {
        startButton.enabled = true;
    }
}
