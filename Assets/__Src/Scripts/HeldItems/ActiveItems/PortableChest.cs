using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortableChest : ActiveItem {
    private ActiveCooldown cooldown;
    private ActiveUses uses;

    public override void ActivateHold() {

    }

    public override void ActivateRelease() {

    }

    public override void ActiveDown() {
        if (!Owner.hasAuthority) return;
        if (cooldown.Ready) {
            SpawnItems();
            uses.Use();
            cooldown.ResetCooldown();
        }
    }

    public override void OnEquip() {
        cooldown = GetComponent<ActiveCooldown>();
        uses = GetComponent<ActiveUses>();
    }

    public override void OnDequip() { }

    private void SpawnItems() {
        SpawnSingleItem(1);
        SpawnSingleItem(-1);
    }

    private void SpawnSingleItem(float rightOffset) {
        Transform t = Owner.transform;
        Vector3 pos = t.position + t.forward * 1.3f + t.up * 0.5f + t.right * rightOffset;
        Vector3 force = 500f * t.forward + 1000f * t.up + 500f * rightOffset * t.right;
        string tag = Owner.gameObject.tag;
        ItemManager.Instance.CmdSpawnRandom(pos, force, tag);
    }
}
