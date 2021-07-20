using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSS : MonoBehaviour
{
    public int ownedTroopIndex = 0, foreignTroopIndex = 0;
    public static Dictionary<int, TroopInfo> ownedTroops = new Dictionary<int, TroopInfo>();
    public static Dictionary<int, TroopInfo> foreignTroops = new Dictionary<int, TroopInfo>();
    public GameObject troopPrefab;

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            InstantiateTroop();
        }
        */
    }

    public void InstantiateTroop()
    {
        // Spawn at random point
        //int _xCoord = Random.Range(0, TileGenerator.tiles.GetLength(0));
        int _xCoord = Random.Range(0, 0);
        //int _zCoord = Random.Range(0, TileGenerator.tiles.GetLength(1));
        int _zCoord = Random.Range(0, 0);

        GameObject _troop = Instantiate(troopPrefab, new Vector3(_xCoord, 1, _zCoord), Quaternion.identity);

        TroopActionsSS _troopActions = new TroopActionsSS();
        _troop.AddComponent<TroopInfo>().FillScoutInfo(_troop, _troopActions, ownedTroopIndex, 0, _xCoord, _zCoord, 0);
        //_troopInfo.FillScoutInfo(_troop, _troopActions, ownedTroopIndex, 0, _xCoord, _zCoord, 0);
        ownedTroops.Add(ownedTroopIndex, _troop.GetComponent<TroopInfo>());
    }
}
