using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInfo : MonoBehaviour
{
    public GameObject city;
    public CityActionsCS cityActions;

    // resource variables
    public float morale;
    public float education;
    public int manPower;
    public int money;
    public int metal;
    public int wood;
    public int food;
    public int ownerShipRange = 1;

    // resource increase variables
    public int woodResourcesPerTurn;
    public int metalResourcesPerTurn;
    public int foodResourcesPerTurn;

    // Status variables
    public bool isBeingConquered = false;           // tile is being occupied by troop not owned by city owner
    public bool isAbleToBeConquered = false;
    //public bool isOccupied;                 // tile is being occupied by troop owned by city owner
    public bool isConstructingBuilding = false;
    public bool isTrainingTroops = false;
    public int occupyingObjectId;
    public int xIndex;                      // x index for tile that city is on
    public int zIndex;                      // z index for tile that city is on

    // Identity variables
    public int id;
    public int ownerId;
    public int idOfPlayerThatSentInfo;

    public void InitCity(string _biomeName, GameObject _cityObject, int _id, int _ownerId, int _xIndex, int _zIndex, 
                         CityActionsCS _cityActions)
    {
        city = _cityObject;

        id = _id;
        ownerId = _ownerId;
        morale = Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingMorale"]);
        education = Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingEducation"]);
        manPower = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingManPower"]);
        money = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMoney"]);
        food = (int)Constants.biomeInfo[_biomeName]["Food"];
        metal = (int)Constants.biomeInfo[_biomeName]["Metal"];
        wood = (int)Constants.biomeInfo[_biomeName]["Wood"];

        woodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingWoodResourcesPerTurn"]);
        metalResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMetalResourcesPerTurn"]);
        foodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingFoodResourcesPerTurn"]);

        xIndex = _xIndex;
        zIndex = _zIndex;

        cityActions = _cityActions;
    }

    public void InitCityServerSide(string _biomeName, int _id, int _ownerId, int _xIndex, int _zIndex)
    {
        id = _id;
        ownerId = _ownerId;
        morale = Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingMorale"]);
        education = Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingEducation"]);
        manPower = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingManPower"]);
        money = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMoney"]);
        food = (int)Constants.biomeInfo[_biomeName]["Food"];
        metal = (int)Constants.biomeInfo[_biomeName]["Metal"];
        wood = (int)Constants.biomeInfo[_biomeName]["Wood"];

        woodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingWoodResourcesPerTurn"]);
        metalResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMetalResourcesPerTurn"]);
        foodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingFoodResourcesPerTurn"]);

        xIndex = _xIndex;
        zIndex = _zIndex;
    }

    public void InitExistingCity(CityInfo _city, GameObject _cityObject)
    {
        city = _cityObject;

        id = _city.id;
        ownerId = _city.ownerId;
        ownerShipRange = _city.ownerShipRange;

        isBeingConquered = _city.isBeingConquered;
        isConstructingBuilding = _city.isConstructingBuilding;
        isTrainingTroops = _city.isTrainingTroops;
        occupyingObjectId = _city.occupyingObjectId;

        morale = _city.morale;
        education = _city.education;
        manPower = _city.manPower;
        money = _city.money;
        food = _city.food;
        metal = _city.metal;
        wood = _city.wood;

        woodResourcesPerTurn = _city.woodResourcesPerTurn;
        metalResourcesPerTurn = _city.metalResourcesPerTurn;
        foodResourcesPerTurn = _city.foodResourcesPerTurn;

        xIndex = _city.xIndex;
        zIndex = _city.zIndex;
    }

    public void CopyCityInfo(CityInfo _cityToCopy)
    {
        id = _cityToCopy.id;
        ownerId = _cityToCopy.ownerId;
        ownerShipRange = _cityToCopy.ownerShipRange;

        isBeingConquered = _cityToCopy.isBeingConquered;
        isConstructingBuilding = _cityToCopy.isConstructingBuilding;
        isTrainingTroops= _cityToCopy.isTrainingTroops;
        occupyingObjectId = _cityToCopy.occupyingObjectId;

        morale = _cityToCopy.morale;
        education = _cityToCopy.education;
        manPower = _cityToCopy.manPower;
        money = _cityToCopy.money;
        food = _cityToCopy.food;
        metal = _cityToCopy.metal;
        wood = _cityToCopy.wood;

        woodResourcesPerTurn = _cityToCopy.woodResourcesPerTurn;
        metalResourcesPerTurn = _cityToCopy.metalResourcesPerTurn;
        foodResourcesPerTurn = _cityToCopy.foodResourcesPerTurn;
    }

    public void InitConqueredCity()
    {
        cityActions = gameObject.GetComponent<CityActionsCS>();
        cityActions.InitCityActions(this);
    }
}