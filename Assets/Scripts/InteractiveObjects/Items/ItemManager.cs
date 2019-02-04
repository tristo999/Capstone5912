using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private List<GameObject> itemPrefabs;

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;

        LoadAllItemPrefabsFromDirectory();
    }

    public GameObject SpawnItem(Vector3 location)
    {
        return SpawnItem(location, new Vector3(0, 0, 0));
    }

    public GameObject SpawnItem(Vector3 location, Vector3 force)
    {
        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        return SpawnItem(location, force, itemPrefab);
    }

    public GameObject SpawnItem(Vector3 location, Vector3 force, GameObject itemPrefab)
    {
        GameObject newItem = Instantiate(itemPrefab, location, Quaternion.identity);
        newItem.GetComponent<Rigidbody>().AddForce(force);
        newItem.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);

        return newItem;
    }

    private void LoadAllItemPrefabsFromDirectory()
    {
        itemPrefabs = PrefabLoader.LoadAllPrefabsOfType<Item>("Assets/Prefabs/InteractiveObjects/Items");
    }
}
