using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DroppedItem : InteractiveObject {
    public int Id { get; set; }
    public int UsesUsed { get; set; } = 0;
    public static readonly float NO_PARENT_COLLIDE_TIME = 0.25f;

    private GameObject halo;
    private float noCollideTimer = 0;
    private string noCollideTag;

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

    public void StartNoCollideTimer(string tag) {
        noCollideTimer = NO_PARENT_COLLIDE_TIME;
        noCollideTag = tag;
    }

    public override void DoInteract(BoltEntity bEntity) {
        DestroyPickup evnt = DestroyPickup.Create(entity);
        evnt.Send();
    }

    public override void OnEvent(DestroyPickup evnt) {
        BoltNetwork.Destroy(gameObject);
    }

    void Update() {
        noCollideTimer -= Time.deltaTime;
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

    void OnCollisionEnter(Collision collision) {
        if (noCollideTimer > 0 && Equals(collision.gameObject.tag, noCollideTag)) {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            StartCoroutine(EnableCollide(collision.gameObject, noCollideTimer));
        }
    }

    private IEnumerator EnableCollide(GameObject other, float delay) {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(GetComponent<Collider>(), other.GetComponent<Collider>(), false);
    }
}