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

    public GameObject playerUIContainer, mainContainer, skillTreeContainer, quickMenuContainer, menuButton, feedButton;

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
    }

    public void CloseMenu()
    {
        quickMenuContainer.SetActive(false);
        menuButton.SetActive(true);
        PlayerCS.instance.isAbleToCommitActions = true;
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
                 _skill == "Port")
            Constants.avaliableBuildings.Add(_skill);

        InitSkillTree();
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
    }

    public void HideSkillTree()
    {
        skillTreeContainer.SetActive(false);
        OpenMenu();
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
    }
}
