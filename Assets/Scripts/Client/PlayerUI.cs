using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;

    public GameObject playerUIContainer, mainContainer, skillTreeContainer, quickMenuContainer, menuButton, feedButton,
                      settingsMenuContainer, statsMenuContainer;

    // Resource
    public TextMeshProUGUI foodText, woodText, metalText, moneyText, moraleText, educationText, populationText;

    // SKill Tree
    public Button roadSkillButton, wallSkillButton, armySkillButton, snipperSkillButton, missleSkillButton, defenseSkillButton,
                  stealthSkillButton, heavyHitterButton, watchTowerButton, marketSkillButton, housingSkillButton, 
                  librarySkillButton, schoolSkillButton, domeSkillButton, portSkillButton, warshipSkillButton, 
                  farmSkillButton, mineSkillButton, lumberYardSkillButton;

    public TextMeshProUGUI roadSkillText, wallSkillText, armySkillText, snipperSkillText, missleSkillText, defenseSkillText,
                  stealthSkillText, heavyHitterText, watchTowerText, marketSkillText, housingSkillText, librarySkillText, 
                  schoolSkillText, domeSkillText, portSkillText, warshipSkillText, farmSkillText, mineSkillText, 
                  lumberYardSkillText;

    // Purchase Indicator = PI
    public RawImage roadSkillPI, wallSkillPI, armySkillPI, snipperSkillPI, missleSkillPI, defenseSkillPI,
                  stealthSkillPI, heavyHitterPI, watchTowerPI, marketSkillPI, housingSkillPI, librarySkillPI,
                  schoolSkillPI, domeSkillPI, portSkillPI, warshipSkillPI, farmSkillPI, mineSkillPI,
                  lumberYardSkillPI;

    // Settings Menu
    public Slider musicSlider, soundEffectSlider, dragSpeedSlider,   rotationSpeedSlider;

    // Stats Menu
    public class PlayerStatObject
    {
        public GameObject container;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI troopsKilled;
        public TextMeshProUGUI troopsDied;
        public TextMeshProUGUI citiesOwned;

        public int clientId;
        public int troopsKilledInt;
        public int troopsDiedInt;
        public int citiesOwnedInt;
    }
    public GameObject playerStatsPrefab;
    public PlayerStatObject[] playerStatsObjects;


    public Dictionary<string, Button> skillButtons = new Dictionary<string, Button>();
    public Dictionary<string, TextMeshProUGUI> skillText = new Dictionary<string, TextMeshProUGUI>();
    public Dictionary<string, RawImage> skillPurchaseIndicators = new Dictionary<string, RawImage>();

    public void Start()
    {
        // Fill in dict data
        skillButtons = new Dictionary<string, Button>()
        {
            { "Army", armySkillButton },
            { "Snipper", snipperSkillButton },
            { "Missle", missleSkillButton },
            { "Defense", defenseSkillButton },
            { "Stealth", stealthSkillButton },
            { "HeavyHitter", heavyHitterButton },
            { "WatchTower", watchTowerButton },
            { "Port", portSkillButton },
            { "Warship", warshipSkillButton },
            { "Walls", wallSkillButton },
            { "Dome", domeSkillButton },
            { "Library", librarySkillButton },
            { "School", schoolSkillButton },
            { "Housing", housingSkillButton },
            { "Roads", roadSkillButton },
            { "Market", marketSkillButton },
            { "Farm", farmSkillButton },
            { "Mine", mineSkillButton },
            { "LumberYard", lumberYardSkillButton },
        };
        skillText = new Dictionary<string, TextMeshProUGUI>()
        {
            { "Army", armySkillText },
            { "Snipper", snipperSkillText },
            { "Missle", missleSkillText },
            { "Defense", defenseSkillText },
            { "Stealth", stealthSkillText },
            { "HeavyHitter", heavyHitterText },
            { "WatchTower", watchTowerText },
            { "Port", portSkillText },
            { "Warship", warshipSkillText },
            { "Walls", wallSkillText },
            { "Dome", domeSkillText },
            { "Library", librarySkillText },
            { "School", schoolSkillText },
            { "Housing", housingSkillText },
            { "Roads", roadSkillText },
            { "Market", marketSkillText },
            { "Farm", farmSkillText },
            { "Mine", mineSkillText },
            { "LumberYard", lumberYardSkillText },
        };
        skillPurchaseIndicators = new Dictionary<string, RawImage>()
        {
            { "Army", armySkillPI },
            { "Snipper", snipperSkillPI},
            { "Missle", missleSkillPI },
            { "Defense", defenseSkillPI },
            { "Stealth", stealthSkillPI },
            { "HeavyHitter", heavyHitterPI },
            { "WatchTower", watchTowerPI },
            { "Port", portSkillPI },
            { "Warship", warshipSkillPI },
            { "Walls", wallSkillPI },
            { "Dome", domeSkillPI },
            { "Library", librarySkillPI },
            { "School", schoolSkillPI },
            { "Housing", housingSkillPI },
            { "Roads", roadSkillPI },
            { "Market", marketSkillPI },
            { "Farm", farmSkillPI },
            { "Mine", mineSkillPI },
            { "LumberYard", lumberYardSkillPI },
        };
        InitSkillTree();
    }

    /// <summary>
    /// Set player resource UI
    /// </summary>
    public void SetAllResourceUI(int _foodAmount, int _woodAmount, int _metalAmount, int _moneyAmount, int _moraleAmount,
                                 int _educationAmount, int _populationAmount)
    {
        SetFoodAmountUI(_foodAmount);
        SetWoodAmountUI(_woodAmount);
        SetMetalAmountUI(_metalAmount);
        SetMoneyAmount(_moneyAmount);
        SetMoraleAmount(_moraleAmount);
        SetEducationText(_educationAmount);
        SetPopulationText(_populationAmount);
    }

    /// <summary>
    /// Set player resource UI for all int values
    /// Note: Overload Method
    /// </summary>
    public void SetAllIntResourceUI(int _foodAmount, int _woodAmount, int _metalAmount, int _moneyAmount, int _populationAmount)
    {
        SetFoodAmountUI(_foodAmount);
        SetWoodAmountUI(_woodAmount);
        SetMetalAmountUI(_metalAmount);
        SetMoneyAmount(_moneyAmount);
        SetPopulationText(_populationAmount);
    }

    /// <summary>
    /// Set player resource UI for all int values
    /// Note: Overload Method
    /// </summary>
    public void SetAllIntResourceUI()
    {
        SetFoodAmountUI(PlayerCS.instance.food);
        SetWoodAmountUI(PlayerCS.instance.wood);
        SetMetalAmountUI(PlayerCS.instance.metal);
        SetMoneyAmount(PlayerCS.instance.money);
        SetPopulationText(PlayerCS.instance.population);
    }

    public void SetFoodAmountUI(int _foodAmount)
    {
        foodText.text = "Food: " + _foodAmount + " + " + GameManagerCS.instance.GetCityResourcesAddedNextTurn("Food");
    }

    public void SetWoodAmountUI(int _woodAmount)
    {
        woodText.text = "Wood: " + _woodAmount + " + " + GameManagerCS.instance.GetCityResourcesAddedNextTurn("Wood");
    }

    public void SetMetalAmountUI(int _metalAmount)
    {
        metalText.text = "Metal: " + _metalAmount + " + " + GameManagerCS.instance.GetCityResourcesAddedNextTurn("Metal");
    }

    public void SetMoneyAmount(int _moneyAmount)
    {
        moneyText.text = "Money: " + _moneyAmount + " + " + GameManagerCS.instance.GetCityResourcesAddedNextTurn("Money");
    }

    public void SetMoraleAmount(int _moraleAmount)
    {
        moraleText.text = "Morale: " + _moraleAmount;
    }

    public void SetEducationText(int _educationAmount)
    {
        educationText.text = "Education: " + _educationAmount;
    }

    public void SetPopulationText(float _populationAmount)
    {
        populationText.text = "Population: " + _populationAmount + " + " 
                               + GameManagerCS.instance.GetCityResourcesAddedNextTurn("Population");
    }

    public void OpenMenu()
    {
        AudioManager.instance.Play(Constants.uiClickAudio);
        quickMenuContainer.SetActive(true);
        feedButton.SetActive(false);
        // Not optimized but should not matter
        foreach(CityInfo _city in GameManagerCS.instance.cities.Values)
        {
            if (_city.ownerId == ClientCS.instance.myId && !_city.isFeed)
                feedButton.SetActive(true);
        }
        menuButton.SetActive(false);
        PlayerCS.instance.isAbleToCommitActions = false;
        Time.timeScale = 0;
    }

    public void CloseMenu()
    {
        AudioManager.instance.Play(Constants.uiClickAudio);
        quickMenuContainer.SetActive(false);
        menuButton.SetActive(true);
        PlayerCS.instance.isAbleToCommitActions = true;
        Time.timeScale = 1;
    }

    public void PurchaseSkill(string _skill)
    {
        PlayerCS.instance.money -= Constants.allSkills[_skill];
        PlayerCS.instance.skills.Add(_skill);
        SetMoneyAmount(PlayerCS.instance.money);

        if (_skill == "Army" || _skill == "Snipper" || _skill == "Missle" || _skill == "Defense" || _skill == "Stealth" 
            || _skill == "Stealh" || _skill == "HeavyHitter" || _skill == "WatchTower" )
            Constants.avaliableTroops.Add(_skill);
        else if (_skill == "Dome" || _skill == "Library" || _skill == "School" || _skill == "Housing" || _skill == "Market" || 
                 _skill == "Port" || _skill == "Walls" || _skill == "Roads")
            Constants.avaliableBuildings.Add(_skill);

        InitSkillTree();
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    public void InitSkillTree()
    {
        bool _hasPreviousSkills;
        string[] _neededSkills;
        foreach(string _key in skillButtons.Keys)
        {
            _hasPreviousSkills = true;
            // Check if skill has been purchased
            if(PlayerCS.instance.skills.Contains(_key))
            {
                skillButtons[_key].enabled = false;
                skillPurchaseIndicators[_key].gameObject.SetActive(true);
                skillText[_key].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                _neededSkills = Constants.neededSkillsForCertainSkills[_key];
                foreach(string _neededSkill in _neededSkills)
                {
                    if (!PlayerCS.instance.skills.Contains(_neededSkill))
                        _hasPreviousSkills = false;
                }
                // Check if player has enough money to purchase skill
                if (PlayerCS.instance.money >= Constants.allSkills[_key] && _hasPreviousSkills)
                {
                    skillButtons[_key].enabled = true;
                    skillText[_key].color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    skillButtons[_key].enabled = false;
                    skillText[_key].color = new Color(1f, 1f, 1f, .5f);
                }
            }
        }
    }

    public void DisplaySkillTree()
    {
        skillTreeContainer.SetActive(true);
        CloseMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
        Time.timeScale = 0;
    }

    public void HideSkillTree()
    {
        skillTreeContainer.SetActive(false);
        OpenMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    public void FeedCities()
    {
        bool _feedCompleted = true;
        int _cityIndex = 0, _cityKey, _cityLevel;
        
        List<int> _cityKeys = GameManagerCS.instance.cities.Keys.ToList();

        while (_feedCompleted && _cityIndex < _cityKeys.Count)
        {
            _cityKey = _cityKeys[_cityIndex];
            if(GameManagerCS.instance.cities[_cityKey].ownerId == ClientCS.instance.myId
               && !GameManagerCS.instance.cities[_cityKey].isFeed)
            {
                _cityLevel = GameManagerCS.instance.cities[_cityKey].level;

                if (PlayerCS.instance.food >= Constants.cityFeedValues[_cityLevel]["Food"]
                    && PlayerCS.instance.metal >= Constants.cityFeedValues[_cityLevel]["Metal"]
                    && PlayerCS.instance.wood >= Constants.cityFeedValues[_cityLevel]["Wood"]
                    && PlayerCS.instance.money >= Constants.cityFeedValues[_cityLevel]["Money"])
                {
                    PlayerCS.instance.food -= Constants.cityFeedValues[_cityLevel]["Food"];
                    PlayerCS.instance.metal -= Constants.cityFeedValues[_cityLevel]["Metal"];
                    PlayerCS.instance.wood -= Constants.cityFeedValues[_cityLevel]["Wood"];
                    PlayerCS.instance.money -= Constants.cityFeedValues[_cityLevel]["Money"];
                    GameManagerCS.instance.cities[_cityKey].isFeed = true;
                }
                else
                    _feedCompleted = false;

            }

            _cityIndex++;
        }
        if (_feedCompleted)
            feedButton.SetActive(false);
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    #region Settings

    public void DisplaySettingsMenu()
    {
        settingsMenuContainer.SetActive(true);
        CloseMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
        Time.timeScale = 0;
    }

    public void CloseSettingsMenu()
    {
        settingsMenuContainer.SetActive(false);
        OpenMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Sets option sliders to player prefs values
    /// </summary>
    public virtual void SetSliderUI()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .75f);
        soundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        dragSpeedSlider.value = PlayerPrefs.GetFloat("DragSpeed", 2f);
        rotationSpeedSlider.value = PlayerPrefs.GetFloat("RotationSpeed", 2f);
    }

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetMusicVolumePreference(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetMusicVolume();
    }

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetSoundEffectsPreference(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetSoundEffectVolume();
    }

    /// <summary>
    /// Sets new drag speed
    /// </summary>
    /// <param name="_dragSpeed"> new drag speed </param>
    public void UpdateDragSpeed(float _dragSpeed)
    {
        PlayerPrefs.SetFloat("DragSpeed", _dragSpeed);
        if (PlayerCS.instance != null)
            PlayerCS.instance.dragSpeed = _dragSpeed;
    }

    /// <summary>
    /// Sets new rotate speed
    /// </summary>
    /// <param name="_rotateSpeed"> new rotation speed</param>
    public void UpdateRotateSpeed(float _rotateSpeed)
    {
        PlayerPrefs.SetFloat("RotationSpeed", _rotateSpeed);
        if(PlayerCS.instance != null)
            PlayerCS.instance.rotationSpeed = _rotateSpeed;
    }

    #endregion

    #region Stats

    public void OpenStatsMenu()
    {
        statsMenuContainer.SetActive(true);
        CloseMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
        UpdateStatsMenu();
        Time.timeScale = 0;
    }

    public void CloseStatsMenu()
    {
        statsMenuContainer.SetActive(false);
        OpenMenu();
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    public void InitStatsMenu()
    {
        int _currentId;
        playerStatsObjects = new PlayerStatObject[ClientCS.allClients.Count];

        for (int index = 0; index < playerStatsObjects.Length; index++)
        {
            _currentId = ClientCS.allClients[index + 1].id;
            if(_currentId != ClientCS.instance.myId)
            {
                playerStatsObjects[index] = new PlayerStatObject();
                playerStatsObjects[index].container = Instantiate(playerStatsPrefab, statsMenuContainer.transform);
                playerStatsObjects[index].playerName = 
                    playerStatsObjects[index].container.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].troopsKilled = 
                    playerStatsObjects[index].container.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].troopsDied = 
                    playerStatsObjects[index].container.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].citiesOwned = 
                    playerStatsObjects[index].container.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

                playerStatsObjects[index].clientId = _currentId;
                playerStatsObjects[index].playerName.text = ClientCS.allClients[_currentId].username;
                playerStatsObjects[index].troopsKilled.text = ClientCS.allClients[_currentId].troopsKilled.ToString();
                playerStatsObjects[index].troopsDied.text = ClientCS.allClients[_currentId].ownedTroopsKilled.ToString();
                playerStatsObjects[index].citiesOwned.text = ClientCS.allClients[_currentId].citiesOwned.ToString();
            
                playerStatsObjects[index].troopsKilledInt = ClientCS.allClients[_currentId].troopsKilled;
                playerStatsObjects[index].troopsDiedInt = ClientCS.allClients[_currentId].ownedTroopsKilled;
                playerStatsObjects[index].citiesOwnedInt = ClientCS.allClients[_currentId].citiesOwned;
            }
            else
            {
                playerStatsObjects[index] = new PlayerStatObject();
                playerStatsObjects[index].container = Instantiate(playerStatsPrefab, statsMenuContainer.transform);
                playerStatsObjects[index].playerName =
                    playerStatsObjects[index].container.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].troopsKilled =
                    playerStatsObjects[index].container.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].troopsDied =
                    playerStatsObjects[index].container.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                playerStatsObjects[index].citiesOwned =
                    playerStatsObjects[index].container.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

                playerStatsObjects[index].clientId = ClientCS.instance.myId;
                playerStatsObjects[index].playerName.text = PlayerCS.instance.username;
                playerStatsObjects[index].troopsKilled.text = PlayerCS.instance.troopsKilled.ToString();
                playerStatsObjects[index].troopsDied.text = PlayerCS.instance.ownedTroopsKilled.ToString();
                playerStatsObjects[index].citiesOwned.text = PlayerCS.instance.citiesOwned.ToString();

                playerStatsObjects[index].troopsKilledInt = PlayerCS.instance.troopsKilled;
                playerStatsObjects[index].troopsDiedInt = PlayerCS.instance.ownedTroopsKilled;
                playerStatsObjects[index].citiesOwnedInt = PlayerCS.instance.citiesOwned;
            }
        }
    }

    public void UpdateStatsMenu()
    {
        int _currentId;

        for (int index = 0; index < playerStatsObjects.Length; index++)
        {
            _currentId = playerStatsObjects[index].clientId;
            if(_currentId != ClientCS.instance.myId)
            {
                playerStatsObjects[index].troopsKilled.text = ClientCS.allClients[_currentId].troopsKilled.ToString();
                playerStatsObjects[index].troopsDied.text = ClientCS.allClients[_currentId].ownedTroopsKilled.ToString();
                playerStatsObjects[index].citiesOwned.text = ClientCS.allClients[_currentId].citiesOwned.ToString();

                playerStatsObjects[index].troopsKilledInt = ClientCS.allClients[_currentId].troopsKilled;
                playerStatsObjects[index].troopsDiedInt = ClientCS.allClients[_currentId].ownedTroopsKilled;
                playerStatsObjects[index].citiesOwnedInt = ClientCS.allClients[_currentId].citiesOwned;
            }
            else
            {
                playerStatsObjects[index].troopsKilled.text = PlayerCS.instance.troopsKilled.ToString();
                playerStatsObjects[index].troopsDied.text = PlayerCS.instance.ownedTroopsKilled.ToString();
                playerStatsObjects[index].citiesOwned.text = PlayerCS.instance.citiesOwned.ToString();

                playerStatsObjects[index].troopsKilledInt = PlayerCS.instance.troopsKilled;
                playerStatsObjects[index].troopsDiedInt = PlayerCS.instance.ownedTroopsKilled;
                playerStatsObjects[index].citiesOwnedInt = PlayerCS.instance.citiesOwned;
            }
        }

        SortStatsMenu();
    }

    public void SortStatsMenu()
    {
        bool _swapMade = true;

        // Bubble sort
        while (_swapMade)
        {
            _swapMade = false;
            for (int _index = 0; _index < playerStatsObjects.Length - 1; _index++)
            {
                if (playerStatsObjects[_index].citiesOwnedInt > playerStatsObjects[_index + 1].citiesOwnedInt)
                {
                    _swapMade = true;
                    SwapValues(playerStatsObjects, _index, _index + 1);
                }
            }
        }

        RepositionStatObjects();
    }

    public void RepositionStatObjects()
    {
        int _yPos = 125, _yIncrement = -50, _index;

        // Reset Top Player Object
        for (_index = playerStatsObjects.Length - 1; _index >= 0; _index--)
        {
            playerStatsObjects[_index].container.transform.localPosition = new Vector3(0, _yPos, 0);
            _yPos += _yIncrement;
        }
    }

    public void SwapValues(PlayerStatObject[] _array, int _indexOne, int _indexTwo)
    {
        PlayerStatObject _tempObject = _array[_indexOne];
        _array[_indexOne] = _array[_indexTwo];
        _array[_indexTwo] = _tempObject;
    }

    #endregion
}
