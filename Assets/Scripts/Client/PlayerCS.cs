using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCS : MonoBehaviour
{
    public int id;
    public string username;

    public static PlayerCS instance;
    public InputMaster inputMaster;
    public LayerMask whatIsIteractable;
    public Camera cam;

    public int _currentSelectedTroopId;

    // Stats
    public float morale = 0;
    public float education = 0;
    public int manPower = 0;
    public int money = 0;
    public int metal = 0;
    public int wood = 0;
    public int food = 0;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        inputMaster = new InputMaster();
        cam = FindObjectOfType<Camera>();
    }

    private void OnEnable()
    {
        inputMaster.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Disable();
    }

    public void InitPlayer(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hello");
        if (inputMaster.Player.Select.triggered)
        {
            Ray _ray = cam.ScreenPointToRay(inputMaster.Player.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out RaycastHit _hit, whatIsIteractable))
            {
                if (_hit.collider.CompareTag("Troop"))
                {
                    int _id = _hit.collider.GetComponent<TroopInfo>().id;
                    _currentSelectedTroopId = _id;
                    //GameManager.ownedTroops[_id].troopActions.CreateInteractableTiles();
                }
                else if (_hit.collider.CompareTag("MoveableTile"))
                {
                    //TileInfo _tileInfo = TileGenerator.tiles[(int)_hit.transform.position.x, (int)_hit.transform.position.z];
                    //GameManager.ownedTroops[_currentSelectedTroopId].troopActions.MoveToNewTile(_tileInfo);
                }
                else if (_hit.collider.CompareTag("AttackableTile"))
                {
                    //TileInfo _tileInfo = TileGenerator.tiles[(int)_hit.transform.position.x, (int)_hit.transform.position.z];
                    //GameManager.ownedTroops[_currentSelectedTroopId].troopActions.Attack(_tileInfo);
                }
            }
        }
        if(inputMaster.Player.Rotate.triggered)
        {
            //GameManager.ownedTroops[_currentSelectedTroopId].troopActions.Rotate(1);
        }
        if(inputMaster.Player.EndTurn.triggered)
        {
            enabled = false;
            ClientSend.EndTurn();
        }
    }
}
