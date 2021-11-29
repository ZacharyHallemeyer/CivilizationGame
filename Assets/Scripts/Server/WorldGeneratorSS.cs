using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script generates board using perlin and werley noise
/// </summary>
public class WorldGeneratorSS : MonoBehaviour
{
    public static WorldGeneratorSS instance;

    public GameObject tile;
    public GameObject[] allCubes;
    public TileInfo[,] tiles;
    public List<CityInfo> neutralCities;

    private int index = 0;

    public float groundXSize, groundZSize;
    public float scale = 10, xPerlinOffset, yPerlinOffset;
    [Range(0, 1)]
    public float waterLevel = .25f, landLevel = 1f;
    public int worleyPointCount = 250;
    private Vector3[] worleyPoints;
    private string[] biomes;
    public string[] biomeOptions;

    public int amountOfFoodTiles = 15, amountofWoodTiles = 15, amountOfMetalTiles = 15;
    public int amountOfNeutralCities = 15, amountOfObstacles = 15;

    // Set instance or destroy if instance already exist
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int _index = 0;
        biomeOptions = new string[Constants.biomeInfo.Count];
        foreach(string _biomeName in Constants.biomeInfo.Keys)
        {
            biomeOptions[_index] = _biomeName;
            _index++;
        }

        worleyPointCount = PlayerPrefs.GetInt("WorleyPoints", 250);
        amountOfFoodTiles = PlayerPrefs.GetInt("FoodTiles", 15);
        amountofWoodTiles = PlayerPrefs.GetInt("WoodTiles", 15);
        amountOfMetalTiles = PlayerPrefs.GetInt("MetalTiles", 15);
        amountOfObstacles = PlayerPrefs.GetInt("Obstacles", 15);
        amountOfNeutralCities = PlayerPrefs.GetInt("NeutralCities", 15);
        groundXSize = PlayerPrefs.GetInt("XSize", 25);
        groundZSize = PlayerPrefs.GetInt("XZize", 25);
        waterLevel = PlayerPrefs.GetFloat("WaterLevel", .25f);

    }

    /// <summary>
    /// Generates game environment using perlin noise for land and water tiles. Then uses worley noise to change land biomes
    /// </summary>
    public void GenerateWorld()
    {
        int _groundTiles = 0, _difference;
        tiles = new TileInfo[(int)groundXSize, (int)groundZSize];
        allCubes = new GameObject[(int)groundXSize * (int)groundZSize];
        xPerlinOffset = Random.Range(0, 10000);
        yPerlinOffset = Random.Range(0, 10000);
        GenerateWorleyPoints();

        for (int x = 0; x < groundXSize; x++)
        {
            for (int z = 0; z < groundZSize; z++)
            {
                float value = GeneratePerlinNoise(x, z);

                if (value <= waterLevel)
                {
                    GameObject _tile = InstaniateCube(tile, x, z);
                    _tile.transform.parent = transform;
                    allCubes[index] = _tile;
                    index++;
                    _tile.AddComponent<TileInfo>().InitWaterInfo(_tile, x * (int)groundXSize + z, x, z);
                    tiles[x, z] = _tile.GetComponent<TileInfo>();
                }
                else if (value <= landLevel)
                {
                    _groundTiles++;
                    string _biomeType = biomes[GenerateWorleyNoise(x, z)];
                    GameObject _tile = InstaniateCube(tile, x, z);
                    _tile.transform.parent = transform;
                    allCubes[index] = _tile;
                    index++;
                    _tile.AddComponent<TileInfo>().InitTileInfo(_tile, _biomeType, x * (int)groundXSize + z, 0, x, z);
                    tiles[x, z] = _tile.GetComponent<TileInfo>();
                }
            }
        }

        _difference = (_groundTiles - (amountOfFoodTiles + amountOfMetalTiles + amountofWoodTiles
                                    + amountOfNeutralCities + amountOfObstacles));
        // Check there are enough tiles to spawn all resources, obstacles, and cities wanted by player
        if(_difference < 0)
        {
            // Decrement amount of resources, obstacles, and cities
            while(_difference < 0)
            {
                amountOfFoodTiles--;
                amountofWoodTiles--;
                amountOfMetalTiles--;
                amountOfObstacles--;
                amountOfNeutralCities--;
                _difference += 5;
            }
        }

        CreateResourceTiles();
        CreateObstacles();
        CreateNeutralCities();

        ServerSend.WorldCreated();
    }

    public void CreateResourceTiles()
    {
        int _xIndex, _zIndex;
        TileInfo _tileInfo;

        // Init food and resource tiles
        for (int i = 0; i < amountOfFoodTiles; i++)
        {
            do
            {
                _xIndex = Random.Range(0, (int)groundXSize);
                _zIndex = Random.Range(0, (int)groundZSize);
                _tileInfo = tiles[_xIndex, _zIndex];
            }
            while (_tileInfo.isWater || _tileInfo.isFood);
            _tileInfo.isFood = true;
        }
        for (int i = 0; i < amountofWoodTiles; i++)
        {
            do
            {
                _xIndex = Random.Range(0, (int)groundXSize);
                _zIndex = Random.Range(0, (int)groundZSize);
                _tileInfo = tiles[_xIndex, _zIndex];
            }
            while (_tileInfo.isWater || _tileInfo.isFood || _tileInfo.isWood);
            _tileInfo.isWood = true;
        }
        for (int i = 0; i < amountOfMetalTiles; i++)
        {
            do
            {
                _xIndex = Random.Range(0, (int)groundXSize);
                _zIndex = Random.Range(0, (int)groundZSize);
                _tileInfo = tiles[_xIndex, _zIndex];
            }
            while (_tileInfo.isWater || _tileInfo.isFood || _tileInfo.isWood || _tileInfo.isMetal);
            _tileInfo.isMetal = true;
        }
    }

    public void CreateObstacles()
    {
        int _xIndex, _zIndex;
        TileInfo _tileInfo;

        // Create Obstacles
        for (int i = 0; i < amountOfObstacles; i++)
        {
            do
            {
                _xIndex = Random.Range(0, (int)groundXSize);
                _zIndex = Random.Range(0, (int)groundZSize);
                _tileInfo = tiles[_xIndex, _zIndex];
            }
            // Check if tile is water, a resource, or at an edge of the playable area
            while (_tileInfo.isWater || _tileInfo.isFood || _tileInfo.isWood || _tileInfo.isMetal ||
                   _xIndex == 0 || _xIndex == tiles.GetLength(0) || _zIndex == 0 || _zIndex == tiles.GetLength(1));
            _tileInfo.isObstacle = true;
            GameManagerSS.instance.currentCityId++;
        }
    }

    public void CreateNeutralCities()
    {
        int _xIndex, _zIndex;
        TileInfo _tileInfo;

        // Create neutral cities
        for (int i = 0; i < amountOfNeutralCities; i++)
        {
            do
            {
                _xIndex = Random.Range(0, (int)groundXSize);
                _zIndex = Random.Range(0, (int)groundZSize);
                _tileInfo = tiles[_xIndex, _zIndex];
            }
            while (_tileInfo.isWater || _tileInfo.isFood || _tileInfo.isWood || _tileInfo.isMetal
                   || _tileInfo.isObstacle || _tileInfo.isCity);
            _tileInfo.isCity = true;
            _tileInfo.cityId = GameManagerSS.instance.currentCityId;
            CityInfo _city = gameObject.AddComponent<CityInfo>();
            _city.InitCityServerSide(_tileInfo.biome, GameManagerSS.instance.currentCityId, -1, _xIndex, _zIndex);
            neutralCities.Add(_city);
            GameManagerSS.instance.currentCityId++;
        }
    }

    public GameObject InstaniateCube(GameObject _cube, float x, float z)
    {
        return Instantiate(_cube, new Vector3(x, 0, z), Quaternion.identity);
    }

    /// <summary>
    /// Generates random points on game environment, and selects a random biome to match to that point
    /// </summary>
    public void GenerateWorleyPoints()
    {
        worleyPoints = new Vector3[worleyPointCount];
        biomes = new string[worleyPointCount];
        for (int i = 0; i < worleyPointCount; i++)
        {
            worleyPoints[i] = new Vector3(Random.Range(0, groundXSize), 0, Random.Range(0, groundZSize));
            biomes[i] = biomeOptions[Random.Range(0, biomeOptions.Length)];
        }
    }

    /// <summary>
    /// returns worely noise for x and z coord provided in parems
    /// </summary>
    /// <param name="x"> x coord </param>
    /// <param name="z"> z coord </param>
    public int GenerateWorleyNoise(float x, float z)
    {
        Vector3 _point = new Vector3(x, 0, z);
        float _closetDistance = Vector3.Distance(_point, worleyPoints[0]);
        int _closestIndex = 0;

        for (int i = 1; i < worleyPointCount; i++)
        {
            if (Vector3.Distance(_point, worleyPoints[i]) < _closetDistance)
            {
                _closetDistance = Vector3.Distance(_point, worleyPoints[i]);
                _closestIndex = i;
            }
        }

        return _closestIndex;
    }

    /// <summary>
    /// returns worely noise for x and z coord provided in parems
    /// </summary>
    /// <param name="x"> x coord </param>
    /// <param name="z"> z coord </param> 
    /// <returns></returns>
    public float GeneratePerlinNoise(float x, float z)
    {
        float xCoord = (float)x / groundXSize * scale + xPerlinOffset;
        float zCoord = (float)z / groundZSize * scale + yPerlinOffset;

        return Mathf.Clamp(Mathf.PerlinNoise(xCoord, zCoord), 0, 1);
    }
}
