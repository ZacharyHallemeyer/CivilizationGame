using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Dictionary<string, Dictionary<string, float>> biomeInfo = new Dictionary<string, Dictionary<string, float>>
    {
        { "Desert", new Dictionary<string, float>
            {
                { "Temperature", .75f },
                { "Height", 0 },
                { "Metal", 10f },
                { "Wood", 2f },
                { "Food", 2f },
                { "ManPower", 5f },
                { "Money", 2f },
                { "Morale", 5f },
                { "Education", 3f },
                { "MovementCost", 1f },
            }
        },

        { "Forest", new Dictionary<string, float>
            {
                { "Temperature", -.25f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 10f },
                { "ManPower", 5f },
                { "Money", 4f },
                { "Morale", 3f },
                { "Education", 1f },
                { "MovementCost", 1f },
            }
        },

        { "Grassland", new Dictionary<string, float>
            {
                { "Temperature", .1f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 15f },
                { "ManPower", 5f },
                { "Money", 3f },
                { "Morale", 5f },
                { "Education", 2f },
                { "MovementCost", 1f },
            }
        },

        { "RainForest", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Height", 0 },
                { "Metal", 2f },
                { "Wood", 10f },
                { "Food", 10f },
                { "ManPower", 5f },
                { "Money", 2f },
                { "Morale", 5f },
                { "Education", 3f },
                { "MovementCost", 1f },
            }
        },

        { "Swamp", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 5f },
                { "ManPower", 5f },
                { "Money", 3f },
                { "Morale", 5f },
                { "Education", 2f },
                { "MovementCost", 2f },
            }
        },

        { "Tundra", new Dictionary<string, float>
            {
                { "Temperature", -.75f },
                { "Height", 0 },
                { "Metal", 5f },
                { "Wood", 2f },
                { "Food", 2f },
                { "ManPower", 5f },
                { "Money", 4f },
                { "Morale", 5f },
                { "Education", 1f },
                { "MovementCost", 1f },
            }
        },
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
                { "SeeRange", 3},
            }
        },
        { "NormalMilitia", new Dictionary<string, int>
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
                { "SeeRange", 2},
            }
        },
        { "AdvancedMilitia", new Dictionary<string, int>
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
                { "SeeRange", 2},
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
                { "SeeRange", 2},
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
                { "SeeRange", 2},
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
                { "SeeRange", 2},
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
                { "SeeRange", 2},
            }
        },
    };

    public static Dictionary<string, Dictionary<string, bool>> troopInfoBool = new Dictionary<string, Dictionary<string, bool>>
    {
        { "Scout", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
        { "NormalMilitia", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
        { "AdvancedMilitia", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
        { "Missle", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
        { "Defense", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
        { "Stealth", new Dictionary<string, bool>
            {
                { "CanMultyKill", true},
            }
        },
        { "Snipper", new Dictionary<string, bool>
            {
                { "CanMultyKill", false},
            }
        },
    };
}
