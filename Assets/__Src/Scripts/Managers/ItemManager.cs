using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Bolt.EntityEventListener<IItemManagerState>
{
    public static ItemManager Instance;

    public List<ItemDefinition> items = new List<ItemDefinition>();
    public List<GameObject> rarityGlowPrefabs = new List<GameObject>();

    public override void Attached() {
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

    public override void OnEvent(SpawnItem evnt) {
        if (!entity.isOwner) return;
        if (evnt.ItemId == -1) {
            float randomValWeighted = Mathf.Pow(Random.value, 0.83f); // Slight root function (best room spawn rate is 0.75f).
            evnt.ItemId = ItemFromDangerRating(GenerationManager.instance.rarityCurve.Evaluate(randomValWeighted)).ItemId;
        }
        Spawn(evnt.Position, evnt.Force, items[evnt.ItemId].DroppedModel, evnt.SpawnerTag, evnt.UsesUsed);
    }

    public GameObject Spawn(Vector3 location, Vector3 force, GameObject itemPrefab, string spawnerTag = "", int usesUsed = 0) { 
        GameObject newItem = BoltNetwork.Instantiate(itemPrefab, location, Quaternion.identity);
        newItem.GetComponent<DroppedItem>().state.ItemId = itemPrefab.GetComponent<DroppedItem>().Id;
        newItem.GetComponent<DroppedItem>().UsesUsed = usesUsed;
        newItem.GetComponent<Rigidbody>().AddForce(force);
        newItem.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);
        if (!Equals(spawnerTag, "")) {
            newItem.GetComponent<DroppedItem>().StartNoCollideTimer(spawnerTag);
        }
        return newItem;
    }

    public void SpawnItemFromRarity(ItemDefinition.ItemRarity rarity, Vector3 location, string spawnerTag = "") {
        ItemDefinition[] itemsOfRarity = items.Where(i => i.Rarity == rarity).ToArray();
        SpawnItem evnt = SpawnItem.Create(entity);
        evnt.ItemId = items.IndexOf(itemsOfRarity[Random.Range(0, itemsOfRarity.Length)]);
        evnt.Position = location;
        evnt.SpawnerTag = spawnerTag;
        evnt.Send();
    }

    public ItemDefinition ItemFromDangerRating(float dangerRating) {
        ItemDefinition.ItemRarity rarity = ItemDefinition.ItemRarity.Common;
        if (dangerRating > .95) {
            rarity = ItemDefinition.ItemRarity.Mythic;
        } else if (dangerRating > .85) {
            rarity = ItemDefinition.ItemRarity.Legendary;
        } else if (dangerRating > .63) {
            rarity = ItemDefinition.ItemRarity.Rare;
        } else if (dangerRating > .35) {
            rarity = ItemDefinition.ItemRarity.Uncommon;
        }
        if (Random.Range(0f,1f) > .95f && rarity != ItemDefinition.ItemRarity.Mythic) {
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
