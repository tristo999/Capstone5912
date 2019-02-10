using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Item
{ 
    public override void DoInteract(BoltEntity bEntity) {
        bEntity.GetComponent<PlayerMovementController>().GetPickup(this);
        base.DoInteract(entity);
    }
}
