using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityActionsCS : MonoBehaviour
{
    public GameObject quickMenuContainer, mainContainer, troopContainer, constructContainer, statsContainer;
    public CityInfo cityInfo;

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
