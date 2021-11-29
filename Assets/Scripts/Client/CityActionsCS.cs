using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script handles city quick menu and the according actions
/// </summary>
public class CityActionsCS : MonoBehaviour
{
    public GameObject quickMenuContainer, mainContainer, troopContainer, constructContainer, statsContainer;
    public CityInfo cityInfo;

    // Buttons
    public Button lumberYardButton, farmButton, mineButton, housingButton, schoolButton, libraryButton, domeButton, marketButton, 
                  portButton, wallsButton, roadsButton;
    public Button scoutButton, militiaButton, armyButton, missleButton, defenseButton, stealthButton, snipperButton, heavyHitterButton,
                  watchTowerButton;

    // Resource Cost Text
    public TextMeshProUGUI lumberYardText, schoolText, libraryText, domeText, housingText, farmText, mineText, marketText, portText,
                           wallsText, roadsText;
    public TextMeshProUGUI scoutText, militiaText, armyText, missleText, defenseText, stealthText, snipperText, heavyHitterText, 
                           watchTowerText;

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
            { "HeavyHitter", heavyHitterButton },
            { "WatchTower", watchTowerButton },
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
            { "HeavyHitter", heavyHitterText },
            { "WatchTower", watchTowerText },
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
            { "Port", portButton },
            { "Walls", wallsButton },
            { "Roads", roadsButton },
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
            { "Port", portText },
            { "Walls", wallsText },
            { "Roads", roadsText },
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
        PlayerCS.instance.isAbleToCommitActions = true;
        quickMenuContainer.SetActive(false);
        PlayerCS.instance.playerUI.menuButton.SetActive(true);
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    public void ShowQuickMenu()
    {
        PlayerCS.instance.isAbleToCommitActions = false;
        PlayerCS.instance.HideQuckMenus();
        quickMenuContainer.SetActive(true);
        PlayerCS.instance.playerUI.menuButton.SetActive(false);
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    public void ResetQuickMenu()
    {
        mainContainer.SetActive(true);
        troopContainer.SetActive(false);
        constructContainer.SetActive(false);
        statsContainer.SetActive(false);
        PlayerCS.instance.isAbleToCommitActions = false;
        AudioManager.instance.Play(Constants.uiClickAudio);
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

    /// <summary>
    /// Places buttons for all possible troops to be trained
    /// </summary>
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
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Places buttons for all possible buildings able to be constructed
    /// </summary>
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
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Enable construct buildings if player has enough resources to construct it and disable if player does not have resources
    /// Disable all buttons if city is being conquered
    /// </summary>
    public void DisplayPossibleBuildings()
    {
        if(cityInfo.isBeingConquered)
        {
            foreach (string _buildingKey in buildingButtons.Keys)
            {
                buildingButtons[_buildingKey].enabled = false;
            }
            return;
        }

        foreach(string _buildingKey in buildingButtons.Keys)
        {
            if (DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices[_buildingKey]))
                buildingButtons[_buildingKey].enabled = true;
            else
                buildingButtons[_buildingKey].enabled = false;
        }

        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Enable train troops if player has enough resources to train it and disable if player does not have resources
    /// Disable all buttons if city is occupied
    /// </summary>
    public void DisplayPossibleTroops()
    {
        if(GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied)
        {
            foreach( string _troopKey in troopButtons.Keys )
            {
                troopButtons[_troopKey].enabled = false;
            }
            return;
        }

        foreach (string _troopKey in troopButtons.Keys)
        {
            if(DoesPlayerHaveEnoughResources(PlayerCS.instance, Constants.prices[_troopKey]))
                troopButtons[_troopKey].enabled = true;
            else
                troopButtons[_troopKey].enabled = false;
        }

        AudioManager.instance.Play(Constants.uiClickAudio);
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

    /// <summary>
    /// Checks if city should level up. If so, increases level
    /// </summary>
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

    /// <summary>
    /// Increases owenership range, changes city model, and increases resource gain
    /// </summary>
    public void IncreaseLevel()
    {
        ChangeLevelModel();
        IncreaseOwnerShipRange();
        IncreaseResourceGain();

        CityInfo _cityInfo = GameManagerCS.instance.dataStoringObject.AddComponent<CityInfo>();
        _cityInfo.CopyCityInfo(cityInfo);
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { _cityInfo, "LevelUp" } };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }

    /// <summary>
    /// Increases city ownership range
    /// </summary>
    public void IncreaseOwnerShipRange()
    {
        if (cityInfo.level == 4 || cityInfo.level == 5) return;
        cityInfo.ownerShipRange++;
        GameManagerCS.instance.CreateOwnedTiles(cityInfo);
    }

    /// <summary>
    /// Increases resource gain per turn
    /// </summary>
    public void IncreaseResourceGain()
    {
        cityInfo.foodResourcesPerTurn += cityInfo.foodResourcesPerTurn;
        cityInfo.woodResourcesPerTurn += cityInfo.woodResourcesPerTurn;
        cityInfo.metalResourcesPerTurn += cityInfo.metalResourcesPerTurn;
        cityInfo.moneyResourcesPerTurn += cityInfo.moneyResourcesPerTurn;
        cityInfo.populationResourcesPerTurn += cityInfo.populationResourcesPerTurn;
    }

    /// <summary>
    /// Changes city model to reflect city level
    /// </summary>
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
    /// Subtracts troop resource cost from player and spawn troop that can not be used until next turn
    /// </summary>
    /// <param name="_troopName"> type of troop to spawn </param>
    public void StartTrainTroop(string _troopName)
    {
        if (GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied || cityInfo.isTrainingTroops) return;
        currentTroopTraining = _troopName;
        PlayerCS.instance.food -= Constants.prices[currentTroopTraining]["Food"];
        PlayerCS.instance.wood -= Constants.prices[currentTroopTraining]["Wood"];
        PlayerCS.instance.metal -= Constants.prices[currentTroopTraining]["Metal"];
        PlayerCS.instance.money -= Constants.prices[currentTroopTraining]["Money"];
        PlayerCS.instance.population -= Constants.prices[currentTroopTraining]["Population"];
        PlayerCS.instance.playerUI.SetAllIntResourceUI(PlayerCS.instance.food, PlayerCS.instance.wood, PlayerCS.instance.metal,
                                                       PlayerCS.instance.money, PlayerCS.instance.population);
        GameManagerCS.instance.SpawnLocalTroop(ClientCS.instance.myId, currentTroopTraining, cityInfo.xIndex, cityInfo.zIndex, 0, false);
        HideQuickMenu();
    }

    #endregion

    #region Buildings

    /// <summary>
    /// Selects which building to build
    /// </summary>
    /// <param name="_buildingName"> Building requested </param>
    public void SelectBuildingToBuild(string _buildingName)
    {
        currentBuidlingToBuild = _buildingName;
        CreateInteractableTileToBuildOn();
        PlayerCS.instance.currentSelectedCityId = cityInfo.id;
    }

    /// <summary>
    /// Only used for when building a road as it is a different process from building all other buildings
    /// </summary>
    public void SelectedRoadToBuild()
    {
        currentBuidlingToBuild = "Roads";
        CreateInteractableTileToBuildRoadsOn();
        PlayerCS.instance.currentSelectedCityId = cityInfo.id;
    }

    /// <summary>
    /// Increases city resource gain based on building values and 'builds' building
    /// </summary>
    /// <param name="_tileInfo"></param>
    public void BuildBuilding(TileInfo _tileInfo)
    {
        if (_tileInfo.isFood || _tileInfo.isWood || _tileInfo.isMetal)
            Destroy(_tileInfo.resourceObject);
        if (currentBuidlingToBuild == "Walls")
            _tileInfo.isWall = true;

        // Update resource per turn
        cityInfo.foodResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Food"];
        cityInfo.metalResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Metal"];
        cityInfo.woodResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Wood"];
        cityInfo.moneyResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Money"];
        cityInfo.populationResourcesPerTurn += (int)Constants.buildingResourceGain[currentBuidlingToBuild]["Population"];
        cityInfo.maxMorale += Constants.buildingResourceGain[currentBuidlingToBuild]["Morale"];
        cityInfo.morale += Constants.buildingResourceGain[currentBuidlingToBuild]["Morale"];
        cityInfo.education += Constants.buildingResourceGain[currentBuidlingToBuild]["Education"];
        cityInfo.maxEducation += Constants.buildingResourceGain[currentBuidlingToBuild]["Education"];
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

    /// <summary>
    /// 'Builds' a road 
    /// </summary>
    /// <param name="_tile"></param>
    public void BuildRoad(TileInfo _tile)
    {
        _tile.isRoad = true;
        PlayerCS.instance.food -= Constants.prices[currentBuidlingToBuild]["Food"];
        PlayerCS.instance.wood -= Constants.prices[currentBuidlingToBuild]["Wood"];
        PlayerCS.instance.metal -= Constants.prices[currentBuidlingToBuild]["Metal"];
        PlayerCS.instance.money -= Constants.prices[currentBuidlingToBuild]["Money"];
        PlayerCS.instance.playerUI.SetAllIntResourceUI();
        GameManagerCS.instance.UpdateRoadModels(_tile.xIndex, _tile.zIndex);
        GameManagerCS.instance.StoreModifiedTileInfo(_tile, "BuildRoad");
        ResetAlteredObjects();
        SelectedRoadToBuild();
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
                    if (cityInfo.ownerId == _tile.ownerId && !_tile.isCity && !_tile.isBuilding && !_tile.isObstacle)
                    {
                        if (_tile.isWater)
                        {
                            // Only let player build port on water tiles
                            if (currentBuidlingToBuild == "Port")
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = constructBuildingTag;
                                _tile.moveUI.SetActive(true);
                                _tile.boxCollider.enabled = true;
                            }
                        }
                        else
                        {
                            if (currentBuidlingToBuild == "Farm")
                            {
                                if(_tile.isFood)
                                {
                                    _tile.tile.layer = whatIsInteractableValue;
                                    _tile.tile.tag = constructBuildingTag;
                                    _tile.moveUI.SetActive(true);
                                    _tile.boxCollider.enabled = true;
                                }
                            }
                            else if (currentBuidlingToBuild == "LumberYard")
                            {
                                if (_tile.isWood)
                                {
                                    _tile.tile.layer = whatIsInteractableValue;
                                    _tile.tile.tag = constructBuildingTag;
                                    _tile.moveUI.SetActive(true);
                                    _tile.boxCollider.enabled = true;
                                }
                            }
                            else if(currentBuidlingToBuild == "Mine")
                            {
                                if (_tile.isMetal)
                                {
                                    _tile.tile.layer = whatIsInteractableValue;
                                    _tile.tile.tag = constructBuildingTag;
                                    _tile.moveUI.SetActive(true);
                                    _tile.boxCollider.enabled = true;
                                }
                            }
                            else
                            {
                                _tile.tile.layer = whatIsInteractableValue;
                                _tile.tile.tag = constructBuildingTag;
                                _tile.moveUI.SetActive(true);
                                _tile.boxCollider.enabled = true;
                            }
                        }
                        objecstToBeReset[_index] = _tile;
                        _index++;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Roads can be built if tile is connected to a city or other road
    /// </summary>
    public void CreateInteractableTileToBuildRoadsOn()
    {
        TileInfo _tile;
        int _index = 0;
        objecstToBeReset = new TileInfo[8];

        for (int x = cityInfo.xIndex - 1; x <= cityInfo.xIndex + 1; x++)
        {
            for (int z = cityInfo.zIndex - 1; z <= cityInfo.zIndex + 1; z++)
            {
                // Check if tile exists
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                    && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1)
                    && (x != cityInfo.xIndex || z != cityInfo.zIndex))
                {
                    if (_index >= objecstToBeReset.Length)
                    {
                        ResizeAlteredObjectsArray();
                    }
                    _tile = GameManagerCS.instance.tiles[x, z];
                    if(_tile.isRoad && !_tile.roadCounted)
                    {
                        _tile.roadCounted = true;
                        objecstToBeReset[_index] = _tile;
                        _index++;
                        _index = CreateInteractableTileToBuildRoadsOnHelper(_tile, _index);
                    }
                    else if(!_tile.isWater && !_tile.isObstacle && !_tile.isWall 
                            && (_tile.ownerId == ClientCS.instance.myId  || _tile.ownerId == -1)
                            && !_tile.isCity && !_tile.isBuilding)
                    {
                        _tile.tile.layer = whatIsInteractableValue;
                        _tile.tile.tag = constructBuildingTag;
                        _tile.moveUI.SetActive(true);
                        _tile.boxCollider.enabled = true;

                        objecstToBeReset[_index] = _tile;
                        _index++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creastes interactable tiles that roads can be built on
    /// Follows roads through recoursion
    /// </summary>
    /// <param name="_tile"> Tile to actions preform on </param>
    /// <param name="_index"> current index of objects to be reset array </param>
    /// <returns></returns>
    private int CreateInteractableTileToBuildRoadsOnHelper(TileInfo _tile, int _index)
    {
        TileInfo _currentTile;
        for (int x = _tile.xIndex - 1; x <= _tile.xIndex + 1; x++)
        {
            for (int z = _tile.zIndex - 1; z <= _tile.zIndex + 1; z++)
            {
                // Check if tile exists
                if (x >= 0 && x < GameManagerCS.instance.tiles.GetLength(0)
                    && z >= 0 && z < GameManagerCS.instance.tiles.GetLength(1)
                    && (x != _tile.xIndex || z != _tile.zIndex))
                {
                    if (_index >= objecstToBeReset.Length)
                    {
                        ResizeAlteredObjectsArray();
                    }
                    _currentTile = GameManagerCS.instance.tiles[x, z];
                    if (_currentTile.isRoad && !_currentTile.roadCounted)
                    {
                        _currentTile.roadCounted = true;
                        objecstToBeReset[_index] = _currentTile;
                        _index++;
                        _index = CreateInteractableTileToBuildRoadsOnHelper(_currentTile, _index);
                    }
                    else if (!_currentTile.isWater && !_currentTile.isObstacle && !_currentTile.isWall 
                             && (_tile.ownerId == ClientCS.instance.myId || _tile.ownerId == -1)
                             && !_currentTile.isCity && !_currentTile.isBuilding)
                    {
                        _currentTile.tile.layer = whatIsInteractableValue;
                        _currentTile.tile.tag = constructBuildingTag;
                        _currentTile.moveUI.SetActive(true);
                        _currentTile.boxCollider.enabled = true;

                        objecstToBeReset[_index] = _currentTile;
                        _index++;
                    }
                }
            }
        }

        return _index;
    }

    /// <summary>
    /// Resizes altered objects array by doubling capacity
    /// </summary>
    private void ResizeAlteredObjectsArray()
    {
        int _index;
        TileInfo[] _tempArray = new TileInfo[objecstToBeReset.Length];

        for (_index = 0; _index < _tempArray.Length; _index++)
        {
            _tempArray[_index] = objecstToBeReset[_index];
        }

        // Resize array
        objecstToBeReset = new TileInfo[objecstToBeReset.Length * 2];
        for(_index = 0; _index < _tempArray.Length; _index++)
        {
            objecstToBeReset[_index] = _tempArray[_index];
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
                    _tile.tile.layer = whatIsInteractableValue;
                    _tile.tile.tag = cityTag;
                }
                if (_tile.isBuilding)
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = buildingTag;
                }
                else
                {
                    _tile.tile.layer = whatIsDefaultValue;
                    _tile.tile.tag = defaultTileTag;
                }
                _tile.moveUI.SetActive(false);
                _tile.roadCounted = false;
                _tile.boxCollider.enabled = false;
            }
        }
        objecstToBeReset = null;
    }
    #endregion
}
