using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCS : MonoBehaviour
{
    public static GameManagerCS instance;

    public int currentTroopIndex = 0;
    public Dictionary<int, TroopInfo> troops = new Dictionary<int, TroopInfo>();
    public Dictionary<int, CityInfo> cities = new Dictionary<int, CityInfo>();

    public GameObject troopPrefab;

    public static string[] troopNames;
    public static string[] biomeOptions;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        int _index = 0;
        biomeOptions = new string[Constants.biomeInfo.Count];
        foreach (string _biomeName in Constants.biomeInfo.Keys)
        {
            biomeOptions[_index] = _biomeName;
            _index++;
        }
    }

    public void InstantiateTroop(int _ownerId, int _id, string _troopName,int _xCoord, int _zCoord, int _rotation)
    {
        GameObject _troop = Instantiate(troopPrefab, new Vector3(_xCoord, 1, _zCoord), Quaternion.identity);
        TroopActionsCS _troopActions = new TroopActionsCS();
        _troop.AddComponent<TroopInfo>().FillTroopInfo(_troopName, _troop, _troopActions, _id, _ownerId, _xCoord, _zCoord);
       
        troops.Add(currentTroopIndex, _troop.GetComponent<TroopInfo>());
        currentTroopIndex++;
    }

    public void AddCityResources()
    {
        foreach(CityInfo _city in cities.Values)
        {
            if(_city.ownerId == ClientCS.instance.myId)
            {
                PlayerCS.instance.wood += _city.woodResourcesPerTurn;
                PlayerCS.instance.metal += _city.metalResourcesPerTurn;
                PlayerCS.instance.food += _city.foodResourcesPerTurn;
            }
        }
    }
}
