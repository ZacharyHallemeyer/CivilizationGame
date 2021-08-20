using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public GameObject playerUIContainer, mainContainer, skillTreeContainer, quickMenuContainer, menuButton;

    // Resource
    public TextMeshProUGUI foodText, woodText, metalText, moneyText, moraleText, educationText, populationText;

    // SKill Tree
    public Button roadSkillButton, wallSkillButton, armySkillButton, snipperSkillButton, missleSkillButton, defenseSkillButton,
                  stealthSkillButton, marketSkillButton, housingSkillButton, librarySkillButton, schoolSkillButton, domeSkillButton,
                  portSkillButton, warshipSkillButton, farmSkillButton, mineSkillButton, lumberYardSkillButton;

    public TextMeshProUGUI roadSkillText, wallSkillText, armySkillText, snipperSkillText, missleSkillText, defenseSkillText,
                  stealthSkillText, marketSkillText, housingSkillText, librarySkillText, schoolSkillText, domeSkillText,
                  portSkillText, warshipSkillText, farmSkillText, mineSkillText, lumberYardSkillText;

    public Dictionary<string, Button> skillButtons = new Dictionary<string, Button>();
    public Dictionary<string, TextMeshProUGUI> skillText = new Dictionary<string, TextMeshProUGUI>();

    public void Start()
    {
        skillButtons = new Dictionary<string, Button>()
        {
            { "Army", armySkillButton },
            { "Snipper", snipperSkillButton },
            { "Missle", missleSkillButton },
            { "Defense", defenseSkillButton },
            { "Stealth", stealthSkillButton },
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
    }

    /// <summary>
    /// Set player resource UI
    /// </summary>
    public void SetAllResourceUI(int _foodAmount, int _woodAmount, int _metalAmount, int _moneyAmount, float _moraleAmount,
                                 float _educationAmount, int _populationAmount)
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
    /// </summary>
    public void SetAllIntResourceUI(int _foodAmount, int _woodAmount, int _metalAmount, int _moneyAmount, int _populationAmount)
    {
        SetFoodAmountUI(_foodAmount);
        SetWoodAmountUI(_woodAmount);
        SetMetalAmountUI(_metalAmount);
        SetMoneyAmount(_moneyAmount);
        SetPopulationText(_populationAmount);
    }

    public void SetFoodAmountUI(int _foodAmount)
    {
        foodText.text = "Food: " + _foodAmount;
    }

    public void SetWoodAmountUI(int _woodAmount)
    {
        woodText.text = "Wood: " + _woodAmount;
    }

    public void SetMetalAmountUI(int _metalAmount)
    {
        metalText.text = "Metal: " + _metalAmount;
    }

    public void SetMoneyAmount(int _moneyAmount)
    {
        moneyText.text = "Money: " + _moneyAmount;
    }

    public void SetMoraleAmount(float _moraleAmount)
    {
        moraleText.text = "Morale: " + string.Format("{0:N2}", _moraleAmount);
    }

    public void SetEducationText(float _educationAmount)
    {
        educationText.text = "Education: " + string.Format("{0:N2}", _educationAmount);
    }

    public void SetPopulationText(float _populationAmount)
    {
        populationText.text = "Population: " + _populationAmount;
    }

    public void OpenMenu()
    {
        quickMenuContainer.SetActive(true);
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

        if (_skill == "Army" || _skill == "Snipper" || _skill == "Missle" || _skill == "Defense" || _skill == "Stealth" || _skill == "Stealh")
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
            if(PlayerCS.instance.skills.Contains(_key))
            {
                skillButtons[_key].enabled = false;
                skillText[_key].color = new Color(1f, 1f, 1f);
            }
            else
            {
                _neededSkills = Constants.neededSkillsForCertainSkills[_key];
                foreach(string _neededSkill in _neededSkills)
                {
                    if (!PlayerCS.instance.skills.Contains(_neededSkill))
                        _hasPreviousSkills = false;
                }
                if (PlayerCS.instance.money >= Constants.allSkills[_key] && _hasPreviousSkills)
                {
                    skillButtons[_key].enabled = true;
                    skillText[_key].color = new Color(0f, 1f, 0f);
                }
                else
                {
                    skillButtons[_key].enabled = false;
                    skillText[_key].color = new Color(1f, 0.4858491f, 0.4858491f);
                }
            }
        }
    }

    public void DisplaySkillTree()
    {
        InitSkillTree();
        skillTreeContainer.SetActive(true);
        CloseMenu();
    }

    public void HideSkillTree()
    {
        skillTreeContainer.SetActive(false);
        OpenMenu();
    }
}
