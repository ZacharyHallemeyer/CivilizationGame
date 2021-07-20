﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorSS : MonoBehaviour
{
    public static WorldGeneratorSS instance;

    public GameObject tile;
    public GameObject[] allCubes;
    public static TileInfo[,] tiles;

    private int index = 0;

    public float groundXSize, groundZSize;
    public float scale = 30, xPerlinOffset, yPerlinOffset;
    [Range(0, 1)]
    public float waterLevel = .25f, landLevel = 1f;
    public int worleyPointCount = 250;
    private Vector3[] worleyPoints;
    private string[] biomes;
    public string[] biomeOptions;

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
        biomeOptions = new string[BiomeInfo.biomeInfo.Count];
        foreach(string _biomeName in BiomeInfo.biomeInfo.Keys)
        {
            biomeOptions[_index] = _biomeName;
            _index++;
        }
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (GameObject _cube in allCubes)
            {
                Destroy(_cube);
            }
            index = 0;
            GenerateEnvironemnt();
        }
        */
    }

    /// <summary>
    /// Generates game environment using perlin noise for land and water tiles. Then uses worley noise to change land biomes
    /// </summary>
    public void GenerateWorld()
    {
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
                    allCubes[index] = _tile;
                    index++;
                    _tile.AddComponent<TileInfo>().FillWaterInfo(_tile, x * z + z);
                    tiles[x, z] = _tile.GetComponent<TileInfo>();
                }
                else if (value <= landLevel)
                {
                    string _biomeType = biomes[GenerateWorleyNoise(x, z)];
                    GameObject _tile = InstaniateCube(tile, x, z);
                    allCubes[index] = _tile;
                    index++;
                    switch (_biomeType)
                    {
                        case "Desert":
                            _tile.AddComponent<TileInfo>().FillDesertInfo(_tile, x * z + z);
                            break;
                        case "Forest":
                            _tile.AddComponent<TileInfo>().FillForestInfo(_tile, x * z + z);
                            break;
                        case "Grassland":
                            _tile.AddComponent<TileInfo>().FillGrasslandInfo(_tile, x * z + z);
                            break;
                        case "RainForest":
                            _tile.AddComponent<TileInfo>().FillRainForestInfo(_tile, x * z + z);
                            break;
                        case "Swamp":
                            _tile.AddComponent<TileInfo>().FillSwampInfo(_tile, x * z + z);
                            break;
                        case "Tundra":
                            _tile.AddComponent<TileInfo>().FillTundraInfo(_tile, x * z + z);
                            break;
                        default:
                            Debug.Log("Biome not found");
                            break;
                    }
                    tiles[x, z] = _tile.GetComponent<TileInfo>();
                }
            }
        }
        Debug.Log("World Created");
        ServerSend.WorldCreated();
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
