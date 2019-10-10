using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemPickup : DroppedItem
{ 
    [Command]
    public override void CmdDoInteract(GameObject gObject) {
        gObject.GetComponent<PlayerMovementController>().GetPickup(this);
        base.CmdDoInteract(gameObject);
    }
}
