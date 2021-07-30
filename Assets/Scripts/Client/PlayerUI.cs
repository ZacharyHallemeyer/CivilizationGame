using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public GameObject playerUIContainer;
    public TextMeshProUGUI foodText, woodText, metalText, moneyText, moraleText, educationText, populationText;

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
        moraleText.text = "Morale: " + _moraleAmount;
    }

    public void SetEducationText(float _educationAmount)
    {
        educationText.text = "Education: " + _educationAmount;
    }

    public void SetPopulationText(float _populationAmount)
    {
        populationText.text = "Population: " + _populationAmount;
    }
}
