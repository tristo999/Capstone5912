using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance;

    private List<GameObject> chestPrefabs;

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;

        LoadAllChestPrefabsFromDirectory();
        SpawnChestsOnLevelStart();
    }

    public GameObject SpawnChest(Vector3 location, Quaternion directionFacing)
    {
        GameObject chestPrefab = chestPrefabs[Random.Range(0, chestPrefabs.Count)];
        return SpawnChest(location,directionFacing, chestPrefab);
    }

    public GameObject SpawnChest(Vector3 location, Quaternion directionFacing, GameObject chestPrefab)
    {
        return Instantiate(chestPrefab, location, directionFacing);
    }

    private void LoadAllChestPrefabsFromDirectory()
    {
        chestPrefabs = PrefabLoader.LoadAllPrefabsOfType<Chest>("Assets/Prefabs/InteractiveObjects/Chests");
    }


    // TEMP - Remove once level management is in place
    private void SpawnChestsOnLevelStart()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
            Quaternion direction = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            SpawnChest(position, direction);
        }
    }
}
