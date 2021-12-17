using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string buildingName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        int _food = Constants.buildingResourceGain[buildingName]["Food"];
        int _wood = Constants.buildingResourceGain[buildingName]["Wood"];
        int _metal = Constants.buildingResourceGain[buildingName]["Metal"];
        int _population = Constants.buildingResourceGain[buildingName]["Population"];
        int _morale = Constants.buildingResourceGain[buildingName]["Morale"];
        int _education = Constants.buildingResourceGain[buildingName]["Education"];
        int _experience = Constants.buildingResourceGain[buildingName]["Experience"];


        string _text = string.Format("Resource Gain Per Turn\nFood: {0}\nWood: {1}\nMetal: {2}\n" +
            "Resource Gain\nPopulation: {3}\nMorale: {4}\nEducation: {5}\nExperience: {6}\n", 
            _food, _wood, _metal, _population, _morale, _education, _experience);

        PlayerCS.instance.playerUI.ShowStatsToolTip();
        PlayerCS.instance.playerUI.ChangeStatsToolTipText(_text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerCS.instance.playerUI.HideStatsToolTip();
    }
}
