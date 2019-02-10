using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Bolt.EntityEventListener<IItemManagerState>
{
    public static ItemManager Instance;

    public List<WizardFightItem> items = new List<WizardFightItem>();

    private void Start() {
        Instance = this;
        AssignIds();
    }

    private void AssignIds() {
        for (int i = 0; i < items.Count; i++) {
            Debug.LogFormat("Assigning id {0} to item {1}", i, items[i].ItemName);
            items[i].HeldModel.GetComponent<HeldItem>().Id = i;
            items[i].WorldModel.GetComponent<Item>().Id = i;
        }
    }

    public override void OnEvent(SpawnItem evnt) {
        if (!entity.isOwner) return;
        if (evnt.ItemId == -1) {
            evnt.ItemId = Random.Range(0, items.Count);
        }
        SpawnItem(evnt.Position, evnt.Force, items[evnt.ItemId].WorldModel);
    }

    private GameObject SpawnItem(Vector3 location, Vector3 force, GameObject itemPrefab)
    {
        GameObject newItem = BoltNetwork.Instantiate(itemPrefab, location, Quaternion.identity);
        newItem.GetComponent<Item>().state.ItemId = itemPrefab.GetComponent<Item>().Id;
        newItem.GetComponent<Rigidbody>().AddForce(force);
        newItem.GetComponent<Rigidbody>().AddTorque(force.magnitude / 4.0f * new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized);

        return newItem;
    }
}
