using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeInfo
{
    public static Dictionary<string, Dictionary<string, float>> biomeInfo = new Dictionary<string, Dictionary<string, float>>
    {
        { "Desert", new Dictionary<string, float>
            {
                { "Temperature", .75f },
                { "Metal", 10f },
                { "Wood", 2f },
                { "Food", 2f },
                { "ManPower", 5f },
                { "Money", 2f },
                { "Morale", 5f },
                { "Education", 3f },
            }
        },

        { "Forest", new Dictionary<string, float>
            {
                { "Temperature", -.25f },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 10f },
                { "ManPower", 5f },
                { "Money", 4f },
                { "Morale", 3f },
                { "Education", 3f },
            }
        },

        { "Grassland", new Dictionary<string, float>
            {
                { "Temperature", .1f },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 15f },
                { "ManPower", 5f },
                { "Money", 3f },
                { "Morale", 5f },
                { "Education", 2f },
            }
        },

        { "RainForest", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Metal", 2f },
                { "Wood", 10f },
                { "Food", 10f },
                { "ManPower", 5f },
                { "Money", 2f },
                { "Morale", 5f },
                { "Education", 3f },
            }
        },

        { "Swamp", new Dictionary<string, float>
            {
                { "Temperature", .25f },
                { "Metal", 5f },
                { "Wood", 5f },
                { "Food", 5f },
                { "ManPower", 5f },
                { "Money", 3f },
                { "Morale", 5f },
                { "Education", 2f },
            }
        },

        { "Tundra", new Dictionary<string, float>
            {
                { "Temperature", -.75f },
                { "Metal", 5f },
                { "Wood", 2f },
                { "Food", 2f },
                { "ManPower", 5f },
                { "Money", 4f },
                { "Morale", 5f },
                { "Education", 1f },
            }
        },
    };
}
