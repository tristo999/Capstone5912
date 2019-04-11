using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DroppedItem : InteractiveObject {
    public int Id { get; set; }
    public int UsesUsed { get; set; } = 0;

    private GameObject halo;

    private void Start() {
        UpdateHalo();
    }

    public override void AddHighlight() {
        base.AddHighlight();
        if (halo) {
            //halo.SetActive(false);
            ((Behaviour)halo.GetComponent("Halo")).enabled = false;
        }
    }

    public override void RemoveHighlight() {
        base.RemoveHighlight();
        if (halo) {
            //halo.SetActive(true);
            ((Behaviour)halo.GetComponent("Halo")).enabled = true;
        }
    }

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        state.AddCallback("ItemId", IdChanged);
    }

    public override void DoInteract(BoltEntity bEntity) {
        DestroyPickup evnt = DestroyPickup.Create(entity);
        evnt.Send();
    }

    public override void OnEvent(DestroyPickup evnt) {
        BoltNetwork.Destroy(gameObject);
    }

    private void IdChanged() {
        Id = state.ItemId;
        UpdateHalo();
    }

    private void UpdateHalo() {
        if (halo) Destroy(halo);
        
        GameObject glowPrefab = ItemManager.Instance.rarityGlowPrefabs[(int)ItemManager.Instance.items[Id].Rarity];
        halo = Instantiate(glowPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        halo.transform.parent = transform;
        halo.transform.localPosition = Vector3.zero;
    }
}