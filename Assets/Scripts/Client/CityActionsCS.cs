using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityActionsCS : MonoBehaviour
{
    public GameObject quickMenuContainer, mainContainer, troopContainer, constructContainer, statsContainer;
    public CityInfo cityInfo;

    // Buttons
    public Button scoutButton, militiaButton, armyButton, missleButton, defenseButton, stealthButton, snipperButton;
    public Button lumberYardButton, farmButton, mineButton, housingButton, schoolButton, libraryButton, domeButton, marketButton;

    public string currentTroopTraining, currentBuidlingToBuild;

    public TileInfo[] objecstToBeReset;
    public int whatIsInteractableValue, whatIsDefaultValue;
    private string defaultTileTag = "Tile", cityTag = "City", buildingTag = "Building", constructBuildingTag = "ConstructBuilding";

    public void InitCityActions(CityInfo _cityInfo)
    {
        cityInfo = _cityInfo;
        whatIsInteractableValue = LayerMask.NameToLayer("Interactable");
        whatIsDefaultValue = LayerMask.NameToLayer("Default");
    }

    public void ToggleQuickMenu()
    {
        if (quickMenuContainer.activeInHierarchy == false)
            ShowQuickMenu();
        else
            HideQuickMenu();
    }

    public void HideQuickMenu()
    {
        ResetQuickMenu();
        quickMenuContainer.SetActive(false);
    }

    public void ShowQuickMenu()
    {
        quickMenuContainer.SetActive(true);
    }

    public void ResetQuickMenu()
    {
        mainContainer.SetActive(true);
        troopContainer.SetActive(false);
        constructContainer.SetActive(false);
        statsContainer.SetActive(false);
    }

    public void DisplayPossibleBuildings()
    {
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["LumberYard"]))
            lumberYardButton.enabled = true;
        else 
            lumberYardButton.enabled=  false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Farm"]))
            farmButton.enabled = true;
        else
            farmButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Mine"]))
            mineButton.enabled = true;
        else
            mineButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Housing"]))
            housingButton.enabled = true;
        else
            housingButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["School"]))
            schoolButton.enabled = true;
        else
            schoolButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Library"]))
            libraryButton.enabled = true;
        else
            libraryButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Dome"]))
            domeButton.enabled = true;
        else
            domeButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Market"]))
            marketButton.enabled = true;
        else
            marketButton.enabled = false;
    }

    public bool DoesPlayerHaveEnoughResources(PlayerCS _player, Dictionary<string, int> _priceDict)
    {
        return _player.food >= _priceDict["Food"] && _player.metal >= _priceDict["Metal"] && _player.wood >= _priceDict["Wood"] &&
               _player.money >= _priceDict["Money"] && _player.population >= _priceDict["Population"];
    }

    public void StartTrainTroop(string _troopName)
    {
        if (GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied || cityInfo.isTrainingTroops) return;
        cityInfo.isTrainingTroops = true;
        currentTroopTraining = _troopName;
        HideQuickMenu();
    }

    public void SpawnTroop()
    {
        GameManagerCS.instance.SpawnTroop(ClientCS.instance.myId, currentTroopTraining, cityInfo.xIndex, cityInfo.zIndex, 0);
        
        /*Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { cityInfo, "SpawnTroop"} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);*/
    }

    public void SelectBuildingToBuild(string _buildingName)
    {
        currentBuidlingToBuild = _buildingName;
        CreateInteractableTileToBuildOn();
        PlayerCS.instance.currentSelectedCityId = cityInfo.id;
    }

    public void BuildBuilding(TileInfo _tileInfo)
    {
        // Update resource per turn
        cityInfo.foodResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Food"];
        cityInfo.metalResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Metal"];
        cityInfo.woodResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Wood"];
        cityInfo.moneyResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Money"];
        cityInfo.populationResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Population"];
        cityInfo.morale += Constants.buildingResourceGain[currentBuidlingToBuild]["Morale"];
        cityInfo.education += Constants.buildingResourceGain[currentBuidlingToBuild]["Education"];
        PlayerCS.instance.ResetMoraleAndEducation();
        // Subtract resource cost
        PlayerCS.instance.food -= Constants.prices[currentBuidlingToBuild]["Food"];
        PlayerCS.instance.wood -= Constants.prices[currentBuidlingToBuild]["Wood"];
        PlayerCS.instance.metal -= Constants.prices[currentBuidlingToBuild]["Metal"];
        PlayerCS.instance.money -= Constants.prices[currentBuidlingToBuild]["Money"];
        PlayerCS.instance.population -= Constants.prices[currentBuidlingToBuild]["Population"];
        GameManagerCS.instance.SpawnBuilding(currentBuidlingToBuild, _tileInfo);
        ResetAlteredObjects();
    }

    public void CreateInteractableTileToBuildOn()
    {
        int _index = 0;
        objecstToBeReset = new TileInfo[cityInfo.ownerShipRange * 9];

        TileInfo _tile;
        for (int x = cityInfo.xIndex - cityInfo.ownerShipRange; x < cityInfo.xIndex + cityInfo.ownerShipRange + 1; x++)
        {
            for (int z = cityInfo.zIndex - cityInfo.ownerShipRange; z < cityInfo.zIndex + cityInfo.ownerShipRange + 1; z++)
            {
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                    && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1))
                {
                    _tile = GameManagerCS.instance.tiles[x, z];
                    if (cityInfo.ownerId == _tile.ownerId && !_tile.isCity)
                    {
                        if (currentBuidlingToBuild == "Farm")
                        {
                            if(_tile.isFood)
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = constructBuildingTag;
                                _tile.moveUI.SetActive(true);
                            }
                        }
                        else if (currentBuidlingToBuild == "LumberYard")
                        {
                            if (_tile.isWood)
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = constructBuildingTag;
                                _tile.moveUI.SetActive(true);
                            }
                        }
                        else if(currentBuidlingToBuild == "Mine")
                        {
                            if (_tile.isMetal)
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = constructBuildingTag;
                                _tile.moveUI.SetActive(true);
                            }
                        }
                        else
                        {
                            _tile.tile.layer = whatIsInteractableValue;
                            _tile.tile.tag = constructBuildingTag;
                            _tile.moveUI.SetActive(true);
                        }
                        objecstToBeReset[_index] = _tile;
                        _index++;
                    }
                }
            }
        }
    }

    public void ResetAlteredObjects()
    {
        if (objecstToBeReset == null) return;
        foreach (TileInfo _tile in objecstToBeReset)
        {
            if (_tile != null)
            {
                if (_tile.isCity)
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = cityTag;
                    _tile.moveUI.SetActive(false);
                }
                if (_tile.isBuilding)
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = buildingTag;
                    _tile.moveUI.SetActive(false);
                }
                else
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = defaultTileTag;
                    _tile.moveUI.SetActive(false);
                }
                _tile.moveUI.SetActive(false);
            }
        }
        objecstToBeReset = null;
    }
}
