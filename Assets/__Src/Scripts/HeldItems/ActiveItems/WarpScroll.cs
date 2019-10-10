using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpScroll : ActiveItem
{
    private ActiveUses uses;

    public override void ActivateHold() { }
    public override void ActivateRelease() { }

    public override void ActiveDown() {
        Vector3 pos = new Vector3(Random.Range(-15f, 15f), 2, Random.Range(-15f, 15f));
        Owner.GetComponent<PlayerMovementController>().CmdTeleportPlayer(pos);
    }

    public override void OnEquip() {
        uses = GetComponent<ActiveUses>();
    }

    public override void OnDequip() {
    }
}
