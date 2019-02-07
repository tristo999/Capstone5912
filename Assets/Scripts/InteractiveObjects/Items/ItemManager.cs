using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Bolt.EntityEventListener<IItemManagerState>
{
    public static ItemManager Instance;

    public List<GameObject> itemPrefabs = new List<GameObject>();

    public override void Attached() {
        Instance = this;
        SetItemIds();
    }

    private void SetItemIds() {
        for (int i = 0; i < itemPrefabs.Count; i++) {
            itemPrefabs[i].GetComponent<ItemPickup>().pickupPrefab.GetComponent<HeldItem>().Id = i;
        }
    }

    public override void OnEvent(SpawnItem evnt) {
        if (!entity.isOwner) return;
        if (evnt.ItemId == -1) {
            evnt.ItemId = Random.Range(0, itemPrefabs.Count);
        }
        SpawnItem(evnt.Position, evnt.Force, itemPrefabs[evnt.ItemId]);
    }

    private GameObject SpawnItem(Vector3 location, Vector3 force, GameObject itemPrefab)
    {
        GameObject newItem = BoltNetwork.Instantiate(itemPrefab, location, Quaternion.identity);
        newItem.GetComponent<Rigidbody>().AddForce(force);
        newItem.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);

        return newItem;
    }

    public int GetId(Item item) {
        return itemPrefabs.Select((value, index) => new { value, index = index + 1 }).Where(pair => pair.value.GetComponent<Item>() == item).Select(pair => pair.index).FirstOrDefault() - 1;
    }
}
