using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Item
{
    public override string ItemName => itemName;
    public override string ItemDescription => itemDescription;
    public enum PickupType { Passive, Active, Weapon }

    [SerializeField]
    private string itemName = "???";
    [SerializeField]
    private string itemDescription = "Change Me!";

    public GameObject pickupPrefab;
    public PickupType pickupType;

    public override void DoInteract(BoltEntity bEntity) {
        bEntity.GetComponent<PlayerMovementController>().GetPickup(this);
        base.DoInteract(entity);
    }
}
