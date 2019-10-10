using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance;

    public List<ItemDefinition> items = new List<ItemDefinition>();
    public List<GameObject> rarityGlowPrefabs = new List<GameObject>();

    public void Awake() {
        Instance = this;
        AssignIds();
    }

    private void AssignIds() {
        for (int i = 0; i < items.Count; i++) {
            Debug.LogFormat("Assigning id {0} to item {1}", i, items[i].ItemName);
            items[i].HeldModel.GetComponent<HeldItem>().Id = i;
            items[i].DroppedModel.GetComponent<DroppedItem>().Id = i;
        }
    }

    [Command]
    public GameObject CmdSpawnRandom(Vector3 location, Vector3 force, string spawnerTag = "", int usesUsed = 0) {
        float randomValWeighted = Mathf.Pow(Random.value, 0.91f); // Slight root function (best room spawn rate is 0.85f).
        int id = ItemFromDangerRating(GenerationManager.instance.rarityCurve.Evaluate(randomValWeighted)).ItemId;
        return CmdSpawn(location, force, id, spawnerTag, usesUsed);
    }

    [Command]
    public GameObject CmdSpawn(Vector3 location, Vector3 force, int id, string spawnerTag = "", int usesUsed = 0) {
        GameObject itemPrefab = items[id].DroppedModel;
        GameObject newItem = Instantiate(itemPrefab, location, Quaternion.identity);
        newItem.GetComponent<DroppedItem>().Used = usesUsed;
        newItem.GetComponent<Rigidbody>().AddForce(force);
        newItem.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);
        if (!Equals(spawnerTag, "")) {
            newItem.GetComponent<DroppedItem>().StartNoCollideTimer(spawnerTag);
        }
        NetworkServer.Spawn(newItem);
        return newItem;
    }

    [Command]
    public GameObject CmdSpawn(Vector3 location, Vector3 force, GameObject gObject) {
        GameObject spawned = Instantiate(gObject, location, Quaternion.identity);
        spawned.GetComponent<Rigidbody>().AddForce(force);
        spawned.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);
        NetworkServer.Spawn(spawned);
        return spawned;

    }

    [Command]
    public void CmdSpawnItemFromRarity(ItemDefinition.ItemRarity rarity, Vector3 location, string spawnerTag = "") {
        ItemDefinition[] itemsOfRarity = items.Where(i => i.Rarity == rarity).ToArray();
        CmdSpawn(location, Vector3.zero, items.IndexOf(itemsOfRarity[Random.Range(0, itemsOfRarity.Length)]), spawnerTag);
    }

    public ItemDefinition ItemFromDangerRating(float dangerRating) {
        ItemDefinition.ItemRarity rarity = ItemDefinition.ItemRarity.Common;
        if (dangerRating > .985) {
            rarity = ItemDefinition.ItemRarity.Mythic;
        } else if (dangerRating > .95) {
            rarity = ItemDefinition.ItemRarity.Legendary;
        } else if (dangerRating > .8) {
            rarity = ItemDefinition.ItemRarity.Rare;
        } else if (dangerRating > .45) {
            rarity = ItemDefinition.ItemRarity.Uncommon;
        }
        if (Random.Range(0f,1f) > .98f && rarity != ItemDefinition.ItemRarity.Mythic) {
            rarity++;
        }
        return GetItemOfRarity(rarity);
    }

    public ItemDefinition GetItemOfRarity(ItemDefinition.ItemRarity rarity) {
        ItemDefinition[] itemsOfRarity = items.Where(i => i.Rarity == rarity).ToArray();
        if (itemsOfRarity.Length > 0) {
            return itemsOfRarity[Random.Range(0, itemsOfRarity.Length)];
        } else {
            // Backup for while there is not an item of every rarity.
            return items[Random.Range(0, items.Count)];
        }
        
    }
}
