using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInfo : MonoBehaviour
{
    public GameObject city;
    public GameObject cityModel;
    public CityActionsCS cityActions;

    // resource variables
    public int maxMorale;
    public int maxEducation;
    public int morale;
    public int education;
    public int ownerShipRange = 1;

    // resource increase variables
    public int woodResourcesPerTurn;
    public int metalResourcesPerTurn;
    public int foodResourcesPerTurn;
    public int moneyResourcesPerTurn;
    public int populationResourcesPerTurn;

    // Status variables
    public bool isBeingConquered = false;           // tile is being occupied by troop not owned by city owner
    public bool isAbleToBeConquered = false;
    public bool isConstructingBuilding = false;
    public bool isTrainingTroops = false;
    public int occupyingObjectId;
    public int xIndex;                      // x index for tile that city is on
    public int zIndex;                      // z index for tile that city is on
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 10;
    public bool isFeed;

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
        maxMorale = Mathf.FloorToInt(Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingMorale"]));
        maxEducation = Mathf.FloorToInt(Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingEducation"]));
        morale = maxMorale;
        education = maxEducation;

        woodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingWoodResourcesPerTurn"]);
        metalResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMetalResourcesPerTurn"]);
        foodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingFoodResourcesPerTurn"]);
        moneyResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMoneyResourcesPerTurn"]);
        populationResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingPopulationResourcesPerTurn"]);

        xIndex = _xIndex;
        zIndex = _zIndex;

        cityActions = _cityActions;
    }

    public void InitCityServerSide(string _biomeName, int _id, int _ownerId, int _xIndex, int _zIndex)
    {
        id = _id;
        ownerId = _ownerId;
        morale = Mathf.FloorToInt(Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingMorale"]));
        education = Mathf.FloorToInt(Random.Range(1, Constants.biomeInfo[_biomeName]["MaxStartingEducation"]));

        woodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingWoodResourcesPerTurn"]);
        metalResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingMetalResourcesPerTurn"]);
        foodResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingFoodResourcesPerTurn"]);
        moneyResourcesPerTurn = Random.Range(25, (int)Constants.biomeInfo[_biomeName]["MaxStartingMoneyResourcesPerTurn"]);
        populationResourcesPerTurn = Random.Range(1, (int)Constants.biomeInfo[_biomeName]["MaxStartingPopulationResourcesPerTurn"]);

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

        woodResourcesPerTurn = _city.woodResourcesPerTurn;
        metalResourcesPerTurn = _city.metalResourcesPerTurn;
        foodResourcesPerTurn = _city.foodResourcesPerTurn;
        moneyResourcesPerTurn = _city.moneyResourcesPerTurn;
        populationResourcesPerTurn = _city.populationResourcesPerTurn;

        xIndex = _city.xIndex;
        zIndex = _city.zIndex;

        level = _city.level;
        experience = _city.experience;
        experienceToNextLevel = _city.experienceToNextLevel;
    }

    public void UpdateCityInfo(CityInfo _cityToCopy)
    {
        id = _cityToCopy.id;
        ownerId = _cityToCopy.ownerId;
        ownerShipRange = _cityToCopy.ownerShipRange;

        isBeingConquered = _cityToCopy.isBeingConquered;
        isConstructingBuilding = _cityToCopy.isConstructingBuilding;
        occupyingObjectId = _cityToCopy.occupyingObjectId;

        morale = _cityToCopy.morale;
        education = _cityToCopy.education;

        woodResourcesPerTurn = _cityToCopy.woodResourcesPerTurn;
        metalResourcesPerTurn = _cityToCopy.metalResourcesPerTurn;
        foodResourcesPerTurn = _cityToCopy.foodResourcesPerTurn;
        moneyResourcesPerTurn = _cityToCopy.moneyResourcesPerTurn;
        populationResourcesPerTurn = _cityToCopy.populationResourcesPerTurn;

        level = _cityToCopy.level;
        experience = _cityToCopy.experience;
        experienceToNextLevel = _cityToCopy.experienceToNextLevel;
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

        woodResourcesPerTurn = _cityToCopy.woodResourcesPerTurn;
        metalResourcesPerTurn = _cityToCopy.metalResourcesPerTurn;
        foodResourcesPerTurn = _cityToCopy.foodResourcesPerTurn;

        level = _cityToCopy.level;
        experience = _cityToCopy.experience;
        experienceToNextLevel = _cityToCopy.experienceToNextLevel;
    }

    public void InitConqueredCity()
    {
        cityActions = gameObject.GetComponent<CityActionsCS>();
        cityActions.InitCityActions(this);
    }
}