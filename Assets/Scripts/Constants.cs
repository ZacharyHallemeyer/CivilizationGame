﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static float troopYPosition = .899f;

    public static float troopHealthYPositionDescend = -1.5f;
    public static float troopHealthYPositionAscend = 0f;

    public static Dictionary<string, Dictionary<string, float>> biomeInfo = new Dictionary<string, Dictionary<string, float>>
    {
        { "Desert", new Dictionary<string, float>
            {
                { "Temperature", .75f },
                { "Height", 0 },
                { "Metal", 10f },
                { "Wood", 2f },
                { "Food", 2f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 2f },
                { "MaxStartingMorale", 5f },
                { "MaxStartingEducation", 3f },
                { "MovementCost", 1f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 2f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 100f },
                { "MaxStartingPopulationResourcesPerTurn", 5f },
            }
        },

        { "Forest", new Dictionary<string, float>
            {
                { "Temperature", -.25f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 10f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 4f },
                { "MaxStartingMorale", 3f },
                { "MaxStartingEducation", 1f },
                { "MovementCost", 1f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 1f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 100f },
                { "MaxStartingPopulationResourcesPerTurn", 10f },
            }
        },

        { "Grassland", new Dictionary<string, float>
            {
                { "Temperature", .1f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 15f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 3f },
                { "MaxStartingMorale", 5f },
                { "MaxStartingEducation", 2f },
                { "MovementCost", 1f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 1f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 100f },
                { "MaxStartingPopulationResourcesPerTurn", 10f },
            }
        },

        { "RainForest", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Height", 0 },
                { "Metal", 2f },
                { "Wood", 10f },
                { "Food", 10f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 2f },
                { "MaxStartingMorale", 5f },
                { "MaxStartingEducation", 3f },
                { "MovementCost", 1f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 1f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 50f },
                { "MaxStartingPopulationResourcesPerTurn", 20f },
            }
        },

        { "Swamp", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 5f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 3f },
                { "MaxStartingMorale", 5f },
                { "MaxStartingEducation", 2f },
                { "MovementCost", 2f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 1f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 100f },
                { "MaxStartingPopulationResourcesPerTurn", 5f },
            }
        },

        { "Tundra", new Dictionary<string, float>
            {
                { "Temperature", -.75f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 2f },
                { "Food", 2f },
                { "MaxStartingManPower", 5f },
                { "MaxStartingMoney", 4f },
                { "MaxStartingMorale", 5f },
                { "MaxStartingEducation", 1f },
                { "MovementCost", 1f },
                { "MaxStartingWoodResourcesPerTurn", 1f },
                { "MaxStartingMetalResourcesPerTurn", 1f },
                { "MaxStartingFoodResourcesPerTurn", 1f },
                { "MaxStartingMoneyResourcesPerTurn", 200f },
                { "MaxStartingPopulationResourcesPerTurn", 5f },
            }
        },
    };

    public static Dictionary<string, Color> tileColors = new Dictionary<string, Color>()
    {
        { "Desert", new Color(0.9302326f, 1f, 0f, 1f) },     
        { "Forest", new Color(0.1394851f, 1f, 0f, 1f) },     
        { "Grassland", new Color(0.443431f, 1f, 0.01415092f, 1f) },     
        { "RainForest", new Color (0.2272161f, 0.6981132f, 0.4206657f, 1f) },     
        { "Swamp", new Color (0.5849056f, 0.2498035f, 0f, 1f) },     
        { "Tundra", new Color (0.8254717f, 1f, 0.9677305f, 1f) },     
        { "Water", new Color (0f, 0.7060928f, 1f, .9f) },     
    };

    public static Dictionary<string, Dictionary<string, int>> troopInfoInt = new Dictionary<string, Dictionary<string, int>>
    {
        { "Scout", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 3 },
                { "BaseAttack", 1},
                { "StealthAttack", 2},
                { "CounterAttack", 1},
                { "BaseDefense", 0},
                { "FacingDefense", 0},
                { "MovementCost", 5},
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
        { "Militia", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 5},
                { "BaseAttack", 3},
                { "StealthAttack", 4},
                { "CounterAttack", 2},
                { "BaseDefense", 0},
                { "FacingDefense", 1},
                { "MovementCost", 2},
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
        { "Army", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 10},
                { "BaseAttack", 5},
                { "StealthAttack", 6},
                { "CounterAttack", 4},
                { "BaseDefense", 1},
                { "FacingDefense", 3},
                { "MovementCost", 2 },
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
        { "Missle", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 5 },
                { "BaseAttack", 15},
                { "StealthAttack", 15},
                { "CounterAttack", 10},
                { "BaseDefense", 0},
                { "FacingDefense", 0},
                { "MovementCost", 1},
                { "AttackRange", 5},
                { "SeeRange", 5},
            }
        },
        { "Defense", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 15 },
                { "BaseAttack", 1},
                { "StealthAttack", 2},
                { "CounterAttack", 5},
                { "BaseDefense", 5},
                { "FacingDefense", 10},
                { "MovementCost", 1},
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
        { "Stealth", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 1 },
                { "BaseAttack", 1},
                { "StealthAttack", 15},
                { "CounterAttack", 0},
                { "BaseDefense", 0},
                { "FacingDefense", 0},
                { "MovementCost", 4},
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
        { "Snipper", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 1 },
                { "BaseAttack", 10},
                { "StealthAttack", 10},
                { "CounterAttack", 0},
                { "BaseDefense", 0},
                { "FacingDefense", 0},
                { "MovementCost", 3},
                { "AttackRange", 4},
                { "SeeRange", 5},
            }
        },
        { "King", new Dictionary<string, int>
            {
                { "Rotation", 0 },
                { "Health", 20 },
                { "BaseAttack", 10},
                { "StealthAttack", 10},
                { "CounterAttack", 5},
                { "BaseDefense", 3},
                { "FacingDefense", 5},
                { "MovementCost", 3},
                { "AttackRange", 1},
                { "SeeRange", 5},
            }
        },
    };

    public static Dictionary<string, Dictionary<string, bool>> troopInfoBool = new Dictionary<string, Dictionary<string, bool>>
    {
        { "Scout", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", true},
            }
        },
        { "Militia", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", true},
            }
        },
        { "Army", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", true},
            }
        },
        { "Missle", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", false},
            }
        },
        { "Defense", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", true},
            }
        },
        { "Stealth", new Dictionary<string, bool>
            {
                { "CanMultyKill", true},
                { "CanMoveAfterKill", true},
            }
        },
        { "Snipper", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", false},
            }
        },
        { "King", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
                { "CanMoveAfterKill", true},
            }
        },
    };

    public static Dictionary<string, Dictionary<string, int>> prices = new Dictionary<string, Dictionary<string, int>>()
    {
        { "City" , new Dictionary<string, int> 
            {
                { "Food", 1 },
                { "Metal", 1 },
                { "Wood", 1 },
                { "Money", 50 },
                { "Population", 5 },
            }
        },
        { "LumberYard" , new Dictionary<string, int> 
            {
                { "Food", 5 },
                { "Metal", 5 },
                { "Wood", 5 },
                { "Money", 100 },
                { "Population", 0 },
            }
        },
        { "Farm" , new Dictionary<string, int> 
            {
                { "Food", 5 },
                { "Metal", 5 },
                { "Wood", 5 },
                { "Money", 100 },
                { "Population", 0 },
            }
        },
        { "Mine" , new Dictionary<string, int> 
            {
                { "Food", 5 },
                { "Metal", 5 },
                { "Wood", 5 },
                { "Money", 100 },
                { "Population", 0 },
            }
        },
        { "Housing" , new Dictionary<string, int> 
            {
                { "Food", 10 },
                { "Metal", 10 },
                { "Wood", 10 },
                { "Money", 200 },
                { "Population", 0 },
            }
        },
        { "School" , new Dictionary<string, int> 
            {
                { "Food", 10 },
                { "Metal", 10 },
                { "Wood", 10 },
                { "Money", 200 },
                { "Population", 0 },
            }
        },
        { "Library" , new Dictionary<string, int> 
            {
                { "Food", 10 },
                { "Metal", 10 },
                { "Wood", 10 },
                { "Money", 200 },
                { "Population", 0 },
            }
        },
        { "Dome" , new Dictionary<string, int> 
            {
                { "Food", 10 },
                { "Metal", 10 },
                { "Wood", 10 },
                { "Money", 200 },
                { "Population", 0 },
            }
        },
        { "Market" , new Dictionary<string, int> 
            {
                { "Food", 10 },
                { "Metal", 10 },
                { "Wood", 10 },
                { "Money", 200 },
                { "Population", 0 },
            }
        },
        { "Road" , new Dictionary<string, int> 
            {
                { "Food", 1 },
                { "Metal", 1 },
                { "Wood", 1 },
                { "Money", 10 },
                { "Population", 0 },
            }
        },
        { "Wall" , new Dictionary<string, int> 
            {
                { "Food", 1 },
                { "Metal", 1 },
                { "Wood", 1 },
                { "Money", 10 },
                { "Population", 0 },
            }
        },
        { "Scout" , new Dictionary<string, int> 
            {
                { "Food", 1 },
                { "Metal", 1 },
                { "Wood", 1 },
                { "Money", 50 },
                { "Population", 2 },
            }
        },
        { "Militia" , new Dictionary<string, int> 
            {
                { "Food", 1 },
                { "Metal", 2 },
                { "Wood", 2 },
                { "Money", 50 },
                { "Population", 5 },
            } 
        },
        { "Army" , new Dictionary<string, int> 
            {
                { "Food", 2 },
                { "Metal", 3 },
                { "Wood", 3 },
                { "Money", 100 },
                { "Population", 5 },
            } 
        },
        { "Missle" , new Dictionary<string, int> 
            {
                { "Food", 2 },
                { "Metal", 3 },
                { "Wood", 3 },
                { "Money", 100 },
                { "Population", 5 },
            } 
        },
        { "Defense" , new Dictionary<string, int> 
            {
                { "Food", 3 },
                { "Metal", 3 },
                { "Wood", 3 },
                { "Money", 100 },
                { "Population", 5 },
            } 
        },
        { "Stealth" , new Dictionary<string, int> 
            {
                { "Food", 3 },
                { "Metal", 3 },
                { "Wood", 3 },
                { "Money", 100 },
                { "Population", 3 },
            } 
        },
        { "Snipper" , new Dictionary<string, int> 
            {
                { "Food", 5 },
                { "Metal", 5 },
                { "Wood", 5 },
                { "Money", 200 },
                { "Population", 5 },
            } 
        },
    };

    public static Dictionary<string, Dictionary<string, float>> buildingResourceGain = new Dictionary<string, Dictionary<string, float>>()
    {
        { "Lumberyard" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 10 },
                { "Money", 100 },
                { "Population", 1 },
                { "Morale", 0 },
                { "Education", .05f },
                { "Experience", 10f },
            } 
        },
        { "Farm" , new Dictionary<string, float> 
            {
                { "Food", 10 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 100 },
                { "Population", 1 },
                { "Morale", 0 },
                { "Education", .05f },
                { "Experience", 10f },
            } 
        },
        { "Mine" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 10 },
                { "Wood", 0 },
                { "Money", 100 },
                { "Population", 1 },
                { "Morale", 0 },
                { "Education", .05f },
                { "Experience", 10f },
            } 
        },
        { "Housing" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 50 },
                { "Population", 5 },
                { "Morale", .05f },
                { "Education", 0 },
                { "Experience", 10f },
            } 
        },
        { "School" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 0 },
                { "Population", 0 },
                { "Morale", 0 },
                { "Education", .1f },
                { "Experience", 10f },
            } 
        },
        { "Library" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 00 },
                { "Population", 5 },
                { "Morale", .1f },
                { "Education", .05f },
                { "Experience", 10f },
            } 
        },
        { "Market" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 200 },
                { "Population", 1 },
                { "Morale", .05f },
                { "Education", 0 },
                { "Experience", 10f },
            } 
        },
        { "Dome" , new Dictionary<string, float> 
            {
                { "Food", 0 },
                { "Metal", 0 },
                { "Wood", 0 },
                { "Money", 50 },
                { "Population", 1 },
                { "Morale", .15f },
                { "Education", 0 },
                { "Experience", 10f },
            } 
        },
    };

    public static Dictionary<string, int> allSkills = new Dictionary<string, int>()
    {
        { "Army", 100 },    
        { "Snipper", 200 },    
        { "Missle", 300 },    
        { "Defense", 200 },    
        { "Stealth", 200 },    
        { "Sailing", 300 },    
        { "Warship", 500 },    
        { "Walls", 100 },    
        { "Dome", 200 },    
        { "Library", 200 },    
        { "School", 300 },    
        { "Housing", 200 },    
        { "Roads", 100 },
        { "Market", 300 },
        { "Farm", 100},
        { "LumberYard", 100},
        { "Mine", 100 },
    };

    public static Dictionary<string, string[]> neededSkillsForCertainSkills = new Dictionary<string, string[]>()
    {
        { "Army", new string[0] },
        { "Snipper", new [] { "Army", "Defense" } },
        { "Missle", new [] { "Army", "Snipper" } },
        { "Defense", new [] { "Army" } },
        { "Stealth", new [] { "Army" } },
        { "Sailing", new string[0] },
        { "Warship", new [] { "Sailing" } },
        { "Walls", new [] { "Mine"} },
        { "Dome", new [] { "Mine", "Walls" } },
        { "Library", new string[0] },
        { "School", new [] { "Library" } },
        { "Housing", new string[0] },
        { "Roads", new string[0] },
        { "Market", new [] { "Roads" } },
        { "Farm", new string[0]},
        { "LumberYard", new string[0] },
        { "Mine", new string[0] },
    };

    public static List<string> tribes = new List<string>()
    {
        "Blue",
        "Cyan",
        "Green",
        "Magenta",
        "Purple",
        "Orange",
        "Pink",
        "Red",
        "White",
        "Yellow",
    };

    public static Dictionary<string, Color> tribeColors = new Dictionary<string, Color>()
    {
        { "Blue", new Color(0f, 0.102445f, 0.5471698f) },
        { "Cyan", new Color(0f, 0.5607098f, 0.5660378f) },
        { "Green", new Color(0.09104663f, 0.6226415f, 0.1229981f) },
        { "Magenta", new Color(0.6226415f, 0.2672659f, 0.4626456f) },
        { "Purple", new Color(0.4716981f, 0f, 0.4716878f) },
        { "Orange", new Color(0.8018868f, 0.476318f, 0f) },
        { "Pink", new Color(0.9056604f, 0f, 0.4993411f) },
        { "Red", new Color(0.6415094f, 0.08775366f, 0.08775366f) },
        { "White", new Color(1f, 1f, 1f) },
        { "Yellow", new Color(0.9433962f, 0.8335572f, 0f) },
    };

    public static Dictionary<string, string> tribeSkills = new Dictionary<string, string>()
    {
        { "Blue", "LumberYard"},
        { "Cyan", "Library"},
        { "Green", "Farm"},
        { "Purple", "Walls"},
        { "Magenta", "Defense"},
        { "Orange", "Roads"},
        { "Pink", "Housing"},
        { "Red", "Army"},
        { "White", "Stealth"},
        { "Yellow", "Mine"},
    };


    public static List<string> avaliableTroops = new List<string>()
    { 
        "Scout",
        "Militia",
    };

    public static List<string> avaliableBuildings = new List<string>()
    { 
        "Farm",
        "LumberYard",
        "Mine",
    };
}
