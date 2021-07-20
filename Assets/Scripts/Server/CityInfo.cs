using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInfo
{
    // resource variables
    public float morale;
    public float education;
    public int manPower;
    public int money;
    public int metal;
    public int wood;
    public int food;
    public int ownerShipRange;

    // resource increase variables
    public int woodResourcesPerTurn;
    public int metalResourcesPerTurn;
    public int foodResourcesPerTurn;

    // Status variables
    public bool isBeingConquered;           // tile is being occupied by troop not owned by city owner
    public bool isOccupied;                 // tile is being occupied by troop owned by city owner
    public bool isConstructingBuilding;
    public bool isTrainingTroops;

    public void InitCity(string _biomeName)
    {
        morale = BiomeInfo.biomeInfo[_biomeName]["Moral"];
        education = BiomeInfo.biomeInfo[_biomeName]["Education"];
        manPower = (int)BiomeInfo.biomeInfo[_biomeName]["ManPower"];
        money = (int)BiomeInfo.biomeInfo[_biomeName]["Money"];
    }
}