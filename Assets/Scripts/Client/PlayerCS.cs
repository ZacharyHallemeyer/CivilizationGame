using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCS : MonoBehaviour
{
    public int id;
    public string username;

    public static PlayerCS instance;
    public InputMaster inputMaster;
    public LayerMask whatIsIteractable;
    public Camera cam;
    public Rigidbody camRB;

    public int _currentSelectedTroopId;

    // Stats
    public float morale = 0;
    public float education = 0;
    public int manPower = 0;
    public int money = 0;
    public int metal = 0;
    public int wood = 0;
    public int food = 0;

    // Camera movement
    public Mouse mouse;
    public bool isRotating = false;
    public Vector3 originRotation;
    public Vector3 differenceRotation;
    public float camForce, camCounterForce;

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
        camRB = cam.GetComponent<Rigidbody>();
        mouse = Mouse.current;
        Cursor.lockState = CursorLockMode.Confined;
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
        if (inputMaster.Player.Select.ReadValue<float>() != 0)
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

        if (inputMaster.Player.Rotate.triggered) ;
            //GameManager.ownedTroops[_currentSelectedTroopId].troopActions.Rotate(1);
        if(inputMaster.Player.EndTurn.triggered)
        {
            enabled = false;
            ClientSend.SendEndOfTurnData();
            GameManagerCS.instance.ClearModifiedData();
        }
        if(inputMaster.Player.Scrool.ReadValue<Vector2>().y != 0)
            ModifyCameraZoom(inputMaster.Player.Scrool.ReadValue<Vector2>().y);

        // Move Camera
        MoveCamera(inputMaster.Player.Move.ReadValue<Vector2>());

        // Rotate Camera
        if (inputMaster.Player.RightClick.ReadValue<float>() != 0 && camRB.velocity.magnitude < 1f)
        {
            differenceRotation = cam.ScreenToViewportPoint(mouse.position.ReadValue()) - cam.transform.position;
            if (!isRotating)
            {
                isRotating = true;
                originRotation = cam.ScreenToViewportPoint(mouse.position.ReadValue()) - cam.transform.position;
            }
        }
        else
            isRotating = false;

        if (isRotating)
        {
            if (differenceRotation.x - originRotation.x > .01)
            {
                cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x,
                                                               cam.transform.localEulerAngles.y + 1f,
                                                               cam.transform.localEulerAngles.z);
            }
            else if(differenceRotation.x - originRotation.x < -.01)
            {
                cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x,
                                                   cam.transform.localEulerAngles.y - 1f,
                                                   cam.transform.localEulerAngles.z);
            }
        }
    }

    public void ModifyCameraZoom(float value)
    {
        value /= 500;
        value = Mathf.Clamp(value, -1, 1);
        float _newX = cam.transform.position.x;
        float _newY = Mathf.Clamp(value + cam.transform.position.y, 3, 50);
        float _newZ = cam.transform.position.z;
        cam.transform.position = new Vector3(_newX, _newY, _newZ);
    }

    public void MoveCamera(Vector2 _direction)
    {
        Vector3 _camOrientationRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);
        Vector3 _camOrientationForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        camRB.AddForce(_camOrientationRight * _direction.x * camForce * Time.deltaTime, ForceMode.Impulse);
        camRB.AddForce(_camOrientationForward * _direction.y * camForce * Time.deltaTime, ForceMode.Impulse);

        if(camRB.velocity.magnitude != 0)
        {
            camRB.AddForce(-camRB.velocity * camCounterForce * Time.deltaTime);
        }
    }
}


/*
else
{
    differencePosition = cam.ScreenToViewportPoint(mouse.position.ReadValue()) - cam.transform.position;
    if (!isDragging)
    {
        isDragging = true;
        originPosition = cam.ScreenToViewportPoint(mouse.position.ReadValue());
    }
}

        if (isDragging)
        {
            Vector3 _move = originPosition - differencePosition;
            cam.transform.position = new Vector3(_move.x, cam.transform.position.y, _move.x);
        }
*/