using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrolloftheMiddle : ActiveItem
{
    public AudioClip telesound;
    private ActiveUses uses;

    public override void ActivateHold() { }
    public override void ActivateRelease() { }

    public override void ActiveDown() {
        GameMaster.instance.sfxSource.PlayOneShot(telesound);
        Owner.GetComponent<PlayerMovementController>().CmdTeleportPlayer(new Vector3(Random.Range(-15f, 15f), 2, Random.Range(-15f, 15f)));
    }

    public override void OnEquip() {
        uses = GetComponent<ActiveUses>();
    }

    public override void OnDequip() {
    }
}
