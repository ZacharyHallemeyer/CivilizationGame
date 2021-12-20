using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;

/// <summary>
/// Script handles all things lobby related
/// </summary>
public class Lobby : MonoBehaviour
{
    public class LobbyRow
    {
        public GameObject lobbyRowGameObject;
        public TextMeshProUGUI username;
        public GameObject tribeContainer;
        public TextMeshProUGUI tribe;
        public TextMeshProUGUI tribeSkill;
        public TextMeshProUGUI tribeEnvironment;
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
    public Button createWorldButton;

    private void Awake()
    {
        startButton.enabled = false;

        // Check if player is not host
        if(GameManagerSS.instance == null)
        {
            // Do not allow player to create world
            createWorldButton.enabled = false;
        }
    }

    /// <summary>
    /// Adds a row of client info for each client
    /// </summary>
    public void InitLobbyUI()
    {
        GameObject _lobbyRow = null;
        int _yPos = 300, _tribeContainerXPos = -1200, _index = 0;
        bool _rightSide = true;

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

            lobbyRows[_index] = new LobbyRow();
            lobbyRows[_index].lobbyRowGameObject = _lobbyRow;
            lobbyRows[_index].username = _lobbyRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            lobbyRows[_index].username.text = ClientCS.allClients[_clientId].username;
            lobbyRows[_index].tribeContainer = _lobbyRow.transform.GetChild(1).gameObject;
            lobbyRows[_index].tribe = lobbyRows[_clientId - 1].tribeContainer.transform.GetChild(0)
                                                                                    .GetComponent<TextMeshProUGUI>();
            lobbyRows[_index].tribe.text = ClientCS.allClients[_clientId].tribe;
            lobbyRows[_index].tribeSkill = lobbyRows[_clientId - 1].tribeContainer.transform.GetChild(1)
                                                                                         .GetComponent<TextMeshProUGUI>();
            lobbyRows[_index].tribeSkill.text = "Skill: " + Constants.tribeSkills[ClientCS.allClients[_clientId].tribe];
            lobbyRows[_index].tribeEnvironment = lobbyRows[_clientId - 1].tribeContainer.transform.GetChild(2)
                                                                                         .GetComponent<TextMeshProUGUI>();
            lobbyRows[_index].tribeEnvironment.text = Constants.tribeNativeEnvironment[
                                                                ClientCS.allClients[_clientId].tribe];

            if (!_rightSide)
            {
                lobbyRows[_index].tribeContainer.GetComponent<RectTransform>().anchoredPosition
                    = new Vector3(_tribeContainerXPos, 0, 0);
            }
            _rightSide = !_rightSide;

            if (_clientId == ClientCS.instance.myId)
                thisClientLobbyRow = lobbyRows[_index];
            
            _index++;
        }

        /*
        publicIP.text = GetPublicIPAddress();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(string.Empty);
        localIP.text = Dns.GetHostEntry(string.Empty).AddressList[5].ToString();
        */
    } 

    /// <summary>
    /// Moves up tribe list by one
    /// </summary>
    public void IncrementTribeList()
    {
        currentTribeIndex++;
        if (currentTribeIndex >= GameManagerCS.instance.avaliableTribes.Count)
            currentTribeIndex = 0;

        string _newTribe = GameManagerCS.instance.avaliableTribes[currentTribeIndex];
        ClientSend.UpdateTribe(ClientCS.allClients[ClientCS.instance.myId].tribe, _newTribe);

    }
    
    /// <summary>
    /// Moves down tribe list by one
    /// </summary>
    public void DecrementTribeList()
    {
        currentTribeIndex--;
        if (currentTribeIndex < 0)
            currentTribeIndex = GameManagerCS.instance.avaliableTribes.Count - 1;

        string _newTribe = GameManagerCS.instance.avaliableTribes[currentTribeIndex];
        ClientSend.UpdateTribe(ClientCS.allClients[ClientCS.instance.myId].tribe, _newTribe);
    }

    /// <summary>
    /// Gets public ip address
    /// </summary>
    /// <returns></returns>
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

    public void SendCreateWorldToServer()
    {
        ClientSend.SendCreateWorld();
        createWorldButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
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
        ClientCS.allClients = new Dictionary<int, RemotePlayer>();
        ClientSS.allClients = new Dictionary<int, ClientSS>();
        // If player is host than close server and network manager
        if (Server.clients.Count > 0)
        {
            Server.clients = new Dictionary<int, ClientSS>();
            Server.Stop();
            Destroy(FindObjectOfType<NetworkManager>().gameObject);
            ClientCS.instance.Disconnect();
        }

        Destroy(FindObjectOfType<ClientCS>().gameObject);
        Destroy(FindObjectOfType<EventSystem>().gameObject);
        SceneManager.LoadScene("ClientMainMenu");
    }

    /// <summary>
    /// Enables start button
    /// </summary>
    public void ToggleStartButtonState()
    {
        startButton.enabled = true;
    }
}
