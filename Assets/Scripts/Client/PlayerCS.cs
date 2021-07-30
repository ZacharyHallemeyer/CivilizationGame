﻿using System.Collections;
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

    public int _currentSelectedTroopId = -1;
    public int _currentSelectedCityId = -1;

    // Stats
    public float morale = 1;
    public float education = 1;
    public int population = 1;
    public int money = 100;
    public int metal = 1;
    public int wood = 1;
    public int food = 1;

    // Camera movement
    public Mouse mouse;
    public bool isRotating = false;
    public Vector3 originRotation;
    public Vector3 differenceRotation;
    public float camForce, camCounterForce;

    // Set instance and needed variables
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

    /// <summary>
    /// Init player info including id and username
    /// </summary>
    public void InitPlayer(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    // Read player input and start actions based on this input
    void Update()
    {
        // TESTING
        if (inputMaster.Player.Testing.triggered)
        {
            //GameManagerCS.instance.InstantiateTroop(id, "Scout", Random.Range(0, 10), Random.Range(0, 10), 0);
            GameManagerCS.instance.SpawnCity(Random.Range(0, 10), Random.Range(0, 10));
        }

        // TESTING END

        if (inputMaster.Player.Select.triggered)
        {
            Ray _ray = cam.ScreenPointToRay(inputMaster.Player.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out RaycastHit _hit, whatIsIteractable))
            {
                if (_hit.collider.CompareTag("Troop"))
                {
                    ResetAlteredTiles();
                    TroopInfo _troop = _hit.collider.GetComponent<TroopInfo>();
                    Debug.Log(_hit.collider.name);
                    if (_troop.ownerId == id)
                    {
                        _troop.troopActions.ShowQuickMenu();
                        _currentSelectedTroopId = _troop.id;
                        GameManagerCS.instance.troops[_troop.id].troopActions.CreateInteractableTiles();
                    }
                }
                else if (_hit.collider.CompareTag("MoveableTile"))
                {
                    TileInfo _tileInfo = GameManagerCS.instance.tiles[(int)_hit.transform.position.x,
                                                                      (int)_hit.transform.position.z];
                    if (GameManagerCS.instance.troops.TryGetValue(_currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.MoveToNewTile(_tileInfo);
                }
                else if (_hit.collider.CompareTag("AttackableTile"))
                {
                    TileInfo _tileInfo = GameManagerCS.instance.tiles[(int)_hit.transform.position.x,
                                                                      (int)_hit.transform.position.z];
                    if (GameManagerCS.instance.troops.TryGetValue(_currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.Attack(_tileInfo);
                }
                else if(_hit.collider.CompareTag("City"))
                {
                    ResetAlteredTiles();
                    TileInfo _tileInfo = _hit.collider.GetComponent<TileInfo>();
                    CityInfo _cityInfo = GameManagerCS.instance.cities[_tileInfo.cityId];
                    if (_cityInfo.ownerId == id  && _currentSelectedTroopId == -1)
                    {
                        _currentSelectedCityId = _cityInfo.id;
                        GameManagerCS.instance.cities[_cityInfo.id].cityActions.ToggleQuickMenu();
                    }
                }
                else if(_hit.collider.CompareTag("MoveableCity"))
                {
                    TileInfo _tileInfo = _hit.collider.GetComponent<TileInfo>();
                    CityInfo _cityInfo = GameManagerCS.instance.cities[_tileInfo.cityId];
                    if (GameManagerCS.instance.troops.TryGetValue(_currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.MoveOntoCity(_tileInfo, _cityInfo);
                }
                else
                {
                    ResetAlteredTiles();
                    _currentSelectedTroopId = -1;
                    _currentSelectedCityId = -1;
                }
            }
            else
            {
                ResetAlteredTiles();
                _currentSelectedTroopId = -1;
                _currentSelectedCityId = -1;
            }
        }

        if (inputMaster.Player.Rotate.triggered && _currentSelectedTroopId >= 0)        // Rotate current selected troop
        {
            if(GameManagerCS.instance.troops.TryGetValue(_currentSelectedTroopId, out TroopInfo _troop))
            {
                _troop.troopActions.Rotate(1);
            }
        }
        if (inputMaster.Player.EndTurn.triggered)        // End Turn
        {
            enabled = false;
            ResetAlteredTiles();
            _currentSelectedCityId = -1;
            _currentSelectedTroopId = -1;
            ClientSend.SendEndOfTurnData();
            GameManagerCS.instance.ResetTroops();
            GameManagerCS.instance.ResetCities();
            GameManagerCS.instance.ClearModifiedData();
            GameManagerCS.instance.DestroyObjectsToDestroyAtEndOfTurn();
            GameManagerCS.instance.isTurn = false;
        }
        if(inputMaster.Player.Scrool.ReadValue<Vector2>().y != 0)       // Zoom Camera in and out
            ModifyCameraZoom(inputMaster.Player.Scrool.ReadValue<Vector2>().y);

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

        // Rotate camera if player wants to
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

    private void FixedUpdate()
    {
        // Move Camera
        MoveCamera(inputMaster.Player.Move.ReadValue<Vector2>());
    }

    public void ResetAlteredTiles()
    {
        if (GameManagerCS.instance.troops.TryGetValue(_currentSelectedTroopId, out TroopInfo _troop))
            _troop.troopActions.ResetAlteredTiles();
    }

    /// <summary>
    /// Zoom in and out depending on value
    /// </summary>
    /// <param name="value"> value to zoom in and out </param>
    public void ModifyCameraZoom(float value)
    {
        value /= 500;
        value = Mathf.Clamp(value, -1, 1);
        float _newX = cam.transform.position.x;
        float _newY = Mathf.Clamp(value + cam.transform.position.y, 3, 50);
        float _newZ = cam.transform.position.z;
        cam.transform.position = new Vector3(_newX, _newY, _newZ);
    }

    /// <summary>
    /// Move camera using physics and provide counter movement if velocity is not 0
    /// </summary>
    /// <param name="_direction"></param>
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