using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DroppedItem : InteractiveObject {
    public int Id { get; set; }
    public int UsesUsed { get; set; } = 0;

    private GameObject halo;

    public override void Attached() {
        // Start with Id uninitialized. 
        state.ItemId = -1;
        Id = -1;

        // Don't allow highlighting until this object has obtained its proper Id.
        CanHighlight = false;

        state.SetTransforms(state.transform, transform);
        state.AddCallback("ItemId", IdChanged);
    }

    public override void AddHighlight() {
        base.AddHighlight();
        if (halo) {
            ((Behaviour)halo.GetComponent("Halo")).enabled = false;
        }
    }

    public override void RemoveHighlight() {
        base.RemoveHighlight();
        if (halo) {
            ((Behaviour)halo.GetComponent("Halo")).enabled = true;
        }
    }

    public override void DoInteract(BoltEntity bEntity) {
        DestroyPickup evnt = DestroyPickup.Create(entity);
        evnt.Send();
    }

    public override void OnEvent(DestroyPickup evnt) {
        BoltNetwork.Destroy(gameObject);
    }

    private void IdChanged() {
        if (state.ItemId >= 0) {
            Id = state.ItemId;
            CanHighlight = true;
            UpdateHalo();
        }
    }

    private void UpdateHalo() {
        if (halo) Destroy(halo);
        
        GameObject glowPrefab = ItemManager.Instance.rarityGlowPrefabs[(int)ItemManager.Instance.items[Id].Rarity];
        halo = Instantiate(glowPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        halo.transform.parent = transform;
        halo.transform.localPosition = Vector3.zero;
    }
}