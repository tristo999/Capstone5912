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
        if (!Owner.entity.isOwner) return;
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

        SpawnItem evnt = SpawnItem.Create(ItemManager.Instance.entity);
        evnt.ItemId = -1;
        evnt.Position = t.position + t.forward * 1.3f + t.up * 0.5f + t.right * rightOffset;
        evnt.Force = 500f * t.forward + 1000f * t.up + 500f * rightOffset * t.right;
        evnt.SpawnerTag = Owner.gameObject.tag;
        evnt.Send();
    }
}
