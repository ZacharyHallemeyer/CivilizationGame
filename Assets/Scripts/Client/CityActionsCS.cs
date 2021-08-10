using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityActionsCS : MonoBehaviour
{
    public GameObject quickMenuContainer, mainContainer, troopContainer, constructContainer, statsContainer;
    public CityInfo cityInfo;

    // Buttons
    public Button scoutButton, militiaButton, armyButton, missleButton, defenseButton, stealthButton, snipperButton;
    public Button lumberYardButton, farmButton, mineButton, housingButton, schoolButton, libraryButton, domeButton, marketButton;

    // Resource Cost Text
    public TextMeshProUGUI lumberYardText, schoolText, libraryText, domeText, housingText, farmText, mineText, marketText;
    public TextMeshProUGUI scoutText, militiaText, armyText, missleText, defenseText, stealthText, snipperText;

    public string currentTroopTraining, currentBuidlingToBuild;

    public TileInfo[] objecstToBeReset;
    public int whatIsInteractableValue, whatIsDefaultValue;
    private string defaultTileTag = "Tile", cityTag = "City", buildingTag = "Building", constructBuildingTag = "ConstructBuilding";

    public Dictionary<string, Button> troopButtons = new Dictionary<string, Button>();
    public Dictionary<string, TextMeshProUGUI> troopResourceText = new Dictionary<string, TextMeshProUGUI>();

    public Dictionary<string, Button> buildingButtons = new Dictionary<string, Button>();
    public Dictionary<string, TextMeshProUGUI> buildingResourceText = new Dictionary<string, TextMeshProUGUI>();

    #region Set Up

    public void InitCityActions(CityInfo _cityInfo)
    {
        cityInfo = _cityInfo;
        whatIsInteractableValue = LayerMask.NameToLayer("Interactable");
        whatIsDefaultValue = LayerMask.NameToLayer("Default");
        troopButtons = new Dictionary<string, Button>()
        {
            { "Scout", scoutButton },
            { "Militia", militiaButton },
            { "Army", armyButton },
            { "Missle", missleButton },
            { "Defense", defenseButton },
            { "Stealth", stealthButton },
            { "Snipper", snipperButton },
        };
        troopResourceText = new Dictionary<string, TextMeshProUGUI>()
        {
            { "Scout", scoutText },
            { "Militia", militiaText },
            { "Army", armyText },
            { "Missle", missleText },
            { "Defense", defenseText },
            { "Stealth", stealthText },
            { "Snipper", snipperText },
        };

        buildingButtons = new Dictionary<string, Button>()
        {
            { "Farm", farmButton },
            { "LumberYard", lumberYardButton},
            { "Mine", mineButton },
            { "Housing", housingButton },
            { "School", schoolButton },
            { "Library", libraryButton },
            { "Market", marketButton },
            { "Dome", domeButton },
        };
        buildingResourceText = new Dictionary<string, TextMeshProUGUI>()
        {
            { "Farm", farmText },
            { "LumberYard", lumberYardText },
            { "Mine", mineText },
            { "Housing", housingText },
            { "School", schoolText },
            { "Library", libraryText },
            { "Market", marketText },
            { "Dome", domeText },
        };
    }

    #endregion

    #region Quick Menu

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
        PlayerCS.instance.playerUI.menuButton.SetActive(true);
    }

    public void ShowQuickMenu()
    {
        PlayerCS.instance.HideQuckMenus();
        quickMenuContainer.SetActive(true);
        PlayerCS.instance.playerUI.menuButton.SetActive(false);
    }

    public void ResetQuickMenu()
    {
        mainContainer.SetActive(true);
        troopContainer.SetActive(false);
        constructContainer.SetActive(false);
        statsContainer.SetActive(false);
    }

    public void SetCurrentCityId()
    {
        PlayerCS.instance.currentSelectedCityId = cityInfo.id;
    }

    public void SetBuildingResourceText()
    {
        lumberYardText.text = ResourceText(Constants.prices["LumberYard"]);
        schoolText.text = ResourceText(Constants.prices["School"]);
        libraryText.text = ResourceText(Constants.prices["Library"]);
        domeText.text = ResourceText(Constants.prices["Dome"]);
        housingText.text = ResourceText(Constants.prices["Housing"]);
        farmText.text = ResourceText(Constants.prices["Farm"]);
        mineText.text = ResourceText(Constants.prices["Mine"]);
        marketText.text = ResourceText(Constants.prices["Market"]);
    }

    public void SetTroopResourceText()
    {
        scoutText.text = ResourceText(Constants.prices["Scout"]);
        militiaText.text = ResourceText(Constants.prices["Militia"]);
        armyText.text = ResourceText(Constants.prices["Army"]);
        missleText.text = ResourceText(Constants.prices["Missle"]);
        defenseText.text = ResourceText(Constants.prices["Defense"]);
        stealthText.text = ResourceText(Constants.prices["Stealth"]);
        snipperText.text = ResourceText(Constants.prices["Snipper"]);
    }

    public string ResourceText(Dictionary<string, int> _priceDict)
    {
        return "F: " +_priceDict["Food"]+ " W: " + _priceDict["Wood"] + " Met: " + _priceDict["Metal"] + " Mon: " + _priceDict["Money"] + 
               " P: " + _priceDict["Population"];
    }

    public void InitTroopMenu()
    {
        int _currentXCoord = -750, _currentYCoord = -425, _xIncrementor = 300, _yIncrementor = 125;
        int _maxXCoord = 750;
        Button _button;
        TextMeshProUGUI _text;

        foreach(string _troop in Constants.avaliableTroops)
        {
            _button = troopButtons[_troop];
            _button.gameObject.SetActive(true);
            _text = troopResourceText[_troop];
            _button.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
            _text.text = ResourceText(Constants.prices[_troop]);
            if(_currentXCoord >= _maxXCoord)
            {
                _currentXCoord = -_maxXCoord;
                _currentYCoord += _yIncrementor;
            }
            else
            {
                _currentXCoord += _xIncrementor;
            }
        }

        DisplayPossibleTroops();
    }

    public void InitBuildingMenu()
    {
        int _currentXCoord = -700, _currentYCoord = -425, _xIncrementor = 400, _yIncrementor = 125;
        int _maxXCoord = 500;
        Button _button;
        TextMeshProUGUI _text;

        foreach (string _building in Constants.avaliableBuildings)
        {
            _button = buildingButtons[_building];
            _button.gameObject.SetActive(true);
            _text = buildingResourceText[_building];
            _button.GetComponent<RectTransform>().anchoredPosition = new Vector3(_currentXCoord, _currentYCoord, 0);
            _text.text = ResourceText(Constants.prices[_building]);
            if (_currentXCoord >= _maxXCoord)
            {
                _currentXCoord = -_maxXCoord;
                _currentYCoord += _yIncrementor;
            }
            else
            {
                _currentXCoord += _xIncrementor;
            }
        }
        DisplayPossibleBuildings();
    }

    /// <summary>
    /// Enable construct buildings if player has enough resources to construct it and disable if player does not have resources
    /// Disable all buttons if city is being conquered
    /// </summary>
    public void DisplayPossibleBuildings()
    {
        if(cityInfo.isBeingConquered)
        {
            lumberYardButton.enabled = false;
            farmButton.enabled = false;
            mineButton.enabled = false;
            housingButton.enabled = false;
            schoolButton.enabled = false;
            libraryButton.enabled = false;
            domeButton.enabled = false;
            marketButton.enabled = false;
            return;
        }
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["LumberYard"]))
            lumberYardButton.enabled = true;
        else 
            lumberYardButton.enabled =  false;
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

    /// <summary>
    /// Enable train troops if player has enough resources to train it and disable if player does not have resources
    /// Disable all buttons if city is occupied
    /// </summary>
    public void DisplayPossibleTroops()
    {
        /*
        if(GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied)
        {
            scoutButton.enabled = false;
            militiaButton.enabled = false;
            armyButton.enabled = false;
            missleButton.enabled = false;
            defenseButton.enabled = false;
            stealthButton.enabled = false;
            snipperButton.enabled = false;
            return;
        }
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Scout"]))
            scoutButton.enabled = true;
        else
            scoutButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Militia"]))
            militiaButton.enabled = true;
        else
            militiaButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Army"]))
            armyButton.enabled = true;
        else
            armyButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Missle"]))
            missleButton.enabled = true;
        else
            missleButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Defense"]))
            defenseButton.enabled = true;
        else
            defenseButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Stealth"]))
            stealthButton.enabled = true;
        else
            stealthButton.enabled = false;
        if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices["Snipper"]))
            snipperButton.enabled = true;
        else
            snipperButton.enabled = false;
        */
    }

    /// <summary>
    /// Returns true if player have enough resources to buy object
    /// </summary>
    /// <param name="_player"> Player that contains the resources </param>
    /// <param name="_priceDict"> Dict containing resource costs for the object </param>
    /// <returns></returns>
    public bool DoesPlayerHaveEnoughResources(PlayerCS _player, Dictionary<string, int> _priceDict)
    {
        return _player.food >= _priceDict["Food"] && _player.metal >= _priceDict["Metal"] && _player.wood >= _priceDict["Wood"] &&
               _player.money >= _priceDict["Money"] && _player.population >= _priceDict["Population"];
    }

    #endregion

    #region Level System

    public void CheckLevel()
    {
        if (cityInfo.level == 5) return;
        if(cityInfo.experience >= cityInfo.experienceToNextLevel)
        {
            cityInfo.level++;
            cityInfo.experienceToNextLevel += cityInfo.experienceToNextLevel * 2;
            IncreaseLevel();
        }
    }

    public void IncreaseLevel()
    {
        ChangeLevelModel();
        IncreaseOwnerShipRange();
        IncreaseResourceGain();

        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { cityInfo, "LevelUp" } };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    public void IncreaseOwnerShipRange()
    {
        if (cityInfo.level == 4 || cityInfo.level == 5) return;
        cityInfo.ownerShipRange++;
        GameManagerCS.instance.CreateOwnedTiles(cityInfo);
    }

    public void IncreaseResourceGain()
    {
        cityInfo.foodResourcesPerTurn += cityInfo.foodResourcesPerTurn;
        cityInfo.woodResourcesPerTurn += cityInfo.woodResourcesPerTurn;
        cityInfo.metalResourcesPerTurn += cityInfo.metalResourcesPerTurn;
        cityInfo.moneyResourcesPerTurn += cityInfo.moneyResourcesPerTurn;
        cityInfo.populationResourcesPerTurn += cityInfo.populationResourcesPerTurn;
    }

    public void ChangeLevelModel()
    {
        Destroy(cityInfo.cityModel);
        GameObject _cityModel = null;
        if (cityInfo.level == 2)
        {
            _cityModel = GameManagerCS.instance.cityLevel2Prefab;
        }
        else if (cityInfo.level == 3)
        {
            _cityModel = GameManagerCS.instance.cityLevel3Prefab;
        }
        else if (cityInfo.level == 4)
        {
            _cityModel = GameManagerCS.instance.cityLevel4Prefab;
        }
        else if (cityInfo.level == 5)
        {
            _cityModel = GameManagerCS.instance.cityLevel5Prefab;
        }
        cityInfo.cityModel = Instantiate(_cityModel, new Vector3(cityInfo.city.transform.position.x,
                                                                 _cityModel.transform.position.y,
                                                                 cityInfo.city.transform.position.z),
                                                                _cityModel.transform.localRotation);
    }

    #endregion

    #region Troops

    /// <summary>
    /// Starts training troop
    /// </summary>
    /// <param name="_troopName"> type of troop to spawn </param>
    public void StartTrainTroop(string _troopName)
    {
        if (GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied || cityInfo.isTrainingTroops) return;
        cityInfo.isTrainingTroops = true;
        currentTroopTraining = _troopName;
        PlayerCS.instance.food -= Constants.prices[currentTroopTraining]["Food"];
        PlayerCS.instance.wood -= Constants.prices[currentTroopTraining]["Wood"];
        PlayerCS.instance.metal -= Constants.prices[currentTroopTraining]["Metal"];
        PlayerCS.instance.money -= Constants.prices[currentTroopTraining]["Money"];
        PlayerCS.instance.population -= Constants.prices[currentTroopTraining]["Population"];
        PlayerCS.instance.playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.wood, PlayerCS.instance.metal,
                                                       PlayerCS.instance.money, PlayerCS.instance.population);
        HideQuickMenu();
    }

    /// <summary>
    /// Spawn troop that was being trained last turn
    /// </summary>
    public void SpawnTroop()
    {
        GameManagerCS.instance.SpawnLocalTroop(ClientCS.instance.myId, currentTroopTraining, cityInfo.xIndex, cityInfo.zIndex, 0);
    }

    #endregion

    #region Buildings

    public void SelectBuildingToBuild(string _buildingName)
    {
        currentBuidlingToBuild = _buildingName;
        CreateInteractableTileToBuildOn();
        PlayerCS.instance.currentSelectedCityId = cityInfo.id;
    }

    public void BuildBuilding(TileInfo _tileInfo)
    {
        if (_tileInfo.isFood || _tileInfo.isWood || _tileInfo.isMetal)
            Destroy(_tileInfo.resourceObject);
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
        // Increase experience and check if city should level up
        cityInfo.experience += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Experience"];
        CheckLevel();
        ResetAlteredObjects();
    }

    #endregion

    #region Interactable Tiles

    /// <summary>
    /// Sets owned tiles to iteractable if player wants to build something and the tiles have the requirments for the building wanted to be spawned
    /// </summary>
    public void CreateInteractableTileToBuildOn()
    {
        int _index = 0;
        objecstToBeReset = new TileInfo[Mathf.FloorToInt(Mathf.Pow(cityInfo.ownerShipRange * 2 + 1, 2))];

        TileInfo _tile;
        for (int x = cityInfo.xIndex - cityInfo.ownerShipRange; x < cityInfo.xIndex + cityInfo.ownerShipRange + 1; x++)
        {
            for (int z = cityInfo.zIndex - cityInfo.ownerShipRange; z < cityInfo.zIndex + cityInfo.ownerShipRange + 1; z++)
            {
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                    && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1))
                {
                    _tile = GameManagerCS.instance.tiles[x, z];
                    if (cityInfo.ownerId == _tile.ownerId && !_tile.isCity && !_tile.isBuilding)
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

    /// <summary>
    /// Reset all altered tiles to default layer and tag
    /// </summary>
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
    #endregion
}
