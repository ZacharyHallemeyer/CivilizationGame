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
    public PlayerUI playerUI;

    public int currentSelectedTroopId = -1;
    public int currentSelectedCityId  = -1;

    // Stats
    public float morale = 0;
    public float education = 0;
    public int population = 5;
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

    // Status variables
    public bool isAnimInProgress = false;
    public bool isAbleToCommitActions = true;

    // Animations
    public Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();
    public IEnumerator runningCoroutine = null;

    public List<string> skills = new List<string>();

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
        playerUI = FindObjectOfType<PlayerUI>().GetComponent<PlayerUI>();
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
        if (inputMaster.Player.Scrool.ReadValue<Vector2>().y != 0)
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
            else if (differenceRotation.x - originRotation.x < -.01)
            {
                cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x,
                                                   cam.transform.localEulerAngles.y - 1f,
                                                   cam.transform.localEulerAngles.z);
            }
        }

        // Troops can not commit actions while an animation is in progress
        if (isAnimInProgress || !isAbleToCommitActions) return;

        if (inputMaster.Player.Select.triggered)
        {
            Ray _ray = cam.ScreenPointToRay(inputMaster.Player.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out RaycastHit _hit, whatIsIteractable))
            {
                if (_hit.collider.CompareTag("Troop"))
                {
                    ResetAlteredTiles();
                    TroopInfo _troop = _hit.collider.GetComponent<TroopInfo>();
                    if (_troop.ownerId == id)
                    {
                        // Deselect troop if troop that has been already selected is selected again
                        if(currentSelectedTroopId == _troop.id)
                        {
                            _troop.troopActions.HideQuickMenu();
                            currentSelectedTroopId = -1;
                        }
                        // Select troop
                        else
                        {
                            _troop.troopActions.ShowQuickMenu();
                            currentSelectedTroopId = _troop.id;
                            GameManagerCS.instance.troops[_troop.id].troopActions.CreateInteractableTiles();
                        }
                    }
                }
                else if (_hit.collider.CompareTag("MoveableTile"))
                {
                    TileInfo _tileInfo = GameManagerCS.instance.tiles[(int)_hit.transform.position.x,
                                                                      (int)_hit.transform.position.z];
                    if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.MoveToNewTile(_tileInfo);
                }
                else if (_hit.collider.CompareTag("AttackableTile"))
                {
                    TileInfo _tileInfo = GameManagerCS.instance.tiles[(int)_hit.transform.position.x,
                                                                      (int)_hit.transform.position.z];
                    if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.Attack(_tileInfo);
                }
                else if(_hit.collider.CompareTag("City"))
                {
                    ResetAlteredTiles();
                    TileInfo _tileInfo = _hit.collider.GetComponent<TileInfo>();
                    CityInfo _cityInfo = GameManagerCS.instance.cities[_tileInfo.cityId];
                    if (_cityInfo.ownerId == id)
                    {
                        if(currentSelectedCityId == _cityInfo.id)
                        {
                            currentSelectedCityId = -1;
                            GameManagerCS.instance.cities[_cityInfo.id].cityActions.HideQuickMenu();
                        }
                        else
                        {
                            currentSelectedCityId = _cityInfo.id;
                            GameManagerCS.instance.cities[_cityInfo.id].cityActions.ShowQuickMenu();
                        }
                    }
                }
                else if(_hit.collider.CompareTag("MoveableCity"))
                {
                    TileInfo _tileInfo = _hit.collider.GetComponent<TileInfo>();
                    CityInfo _cityInfo = GameManagerCS.instance.cities[_tileInfo.cityId];
                    if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
                        _troop.troopActions.MoveOntoCity(_tileInfo, _cityInfo);
                }
                else if(_hit.collider.CompareTag("ConstructBuilding"))
                {

                    TileInfo _tile = _hit.collider.GetComponent<TileInfo>();
                    if (GameManagerCS.instance.cities.TryGetValue(currentSelectedCityId, out CityInfo _city))
                        _city.cityActions.BuildBuilding(_tile);
                }
            }
        }

        // Rotate current selected troop
        if (inputMaster.Player.Rotate.triggered && currentSelectedTroopId >= 0)       
        {
            if(GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
            {
                _troop.troopActions.Rotate(1);
            }
        }
        // End Turn
        if (inputMaster.Player.EndTurn.triggered)       
        {
            EndTurn();
        }
        // Zoom Camera in and out

        // Handle animations
        // Start next animation if current animation is done and if there are more animations in queue
        if(animationQueue.Count > 0 && runningCoroutine == null)
        {
            runningCoroutine = animationQueue.Dequeue();
            StartCoroutine(runningCoroutine);
        }
    }

    private void FixedUpdate()
    {
        // Move Camera
        MoveCamera(inputMaster.Player.Move.ReadValue<Vector2>());
    }

    public void ResetAlteredTiles()
    {
        if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
            _troop.troopActions.ResetAlteredTiles();
        if (GameManagerCS.instance.cities.TryGetValue(currentSelectedCityId, out CityInfo _city))
            _city.cityActions.ResetAlteredObjects();
    }

    public void HideQuckMenus()
    {
        if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
            _troop.troopActions.HideQuickMenu();
        if (GameManagerCS.instance.cities.TryGetValue(currentSelectedCityId, out CityInfo _city))
            _city.cityActions.HideQuickMenu();
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

    public void ResetMoraleAndEducation()
    {
        morale = 0;
        education = 0;

        foreach (CityInfo _city in GameManagerCS.instance.cities.Values)
        {
            if (_city.ownerId == ClientCS.instance.myId && !_city.isBeingConquered)
            {
                morale += _city.morale;
                education += _city.education;
            }
        }
        playerUI.SetMoraleAmount(morale);
        playerUI.SetEducationText(education);
    }

    public void EndTurn()
    {
        enabled = false;
        ResetAlteredTiles();
        // Turn off any quick menus
        if (GameManagerCS.instance.troops.TryGetValue(currentSelectedTroopId, out TroopInfo _troop))
            _troop.troopActions.HideQuickMenu();
        if (GameManagerCS.instance.cities.TryGetValue(currentSelectedCityId, out CityInfo _city))
            _city.cityActions.HideQuickMenu();
        currentSelectedCityId = -1;
        currentSelectedTroopId = -1;
        ClientSend.SendEndOfTurnData(GameManagerCS.instance.isKingAlive);
        GameManagerCS.instance.DestroyObjectsToDestroyAtEndOfTurn();
        GameManagerCS.instance.ClearModifiedData();
        GameManagerCS.instance.isTurn = false;
    }
}