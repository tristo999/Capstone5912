using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemPickup : DroppedItem
{ 
    public override void DoInteract(BoltEntity bEntity) {
        bEntity.GetComponent<PlayerMovementController>().GetPickup(this);
        base.DoInteract(entity);
    }
}
