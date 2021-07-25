﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityActionsCS : MonoBehaviour
{
    public GameObject quickMenuContainer;
    public CityInfo cityInfo;

    public void InitCityActions(CityInfo _cityInfo)
    {
        cityInfo = _cityInfo;
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
        quickMenuContainer.SetActive(false);
    }

    public void ShowQuickMenu()
    {
        quickMenuContainer.SetActive(true);
    }

    public void SpawnTroop(string _troopName)
    {
        if (GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].isOccupied || cityInfo.isTrainingTroops) return;
        cityInfo.isTrainingTroops = true;
        int _xCoord = (int)GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].position.x;
        int _zCoord = (int)GameManagerCS.instance.tiles[cityInfo.xIndex, cityInfo.zIndex].position.y;
        GameManagerCS.instance.SpawnTroop(ClientCS.instance.myId, _troopName, _xCoord, _zCoord, 0);
        Dictionary<CityInfo, string> _cityData = new Dictionary<CityInfo, string>()
        { { cityInfo, "SpawnTroop"} };
        GameManagerCS.instance.modifiedCityInfo.Add(_cityData);
    }
}
