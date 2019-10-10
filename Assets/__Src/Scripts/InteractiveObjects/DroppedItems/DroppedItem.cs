using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DroppedItem : InteractiveObject {
    [SyncVar(hook = nameof(IdChanged))]
    public int Id;
    public HeldItem Item { get; set; }
    public int Used;
    public static readonly float NO_PARENT_COLLIDE_TIME = 0.5f;

    private GameObject halo;
    private float noCollideTimer = 0;
    private string noCollideTag;

    public void Awake() {
        // Start with Id uninitialized. 
        Id = -1;

        // Don't allow highlighting until this object has obtained its proper Id.
        CanHighlight = false;
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

    [Command]
    public override void CmdDoInteract(GameObject gObject) {
        NetworkServer.Destroy(gameObject);
    }

    void Update() {
        noCollideTimer -= Time.deltaTime;
    }

    private void IdChanged() {
        if (Id >= 0) {
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
            Collider otherCollider = GetColliderFromObj(collision.gameObject);
            Collider myCollider = GetColliderFromObj(gameObject);

            Physics.IgnoreCollision(otherCollider, myCollider);
            StartCoroutine(EnableCollide(otherCollider, myCollider, noCollideTimer));
        }
    }

    private IEnumerator EnableCollide(Collider otherCollider, Collider myCollider, float delay) {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(otherCollider, myCollider, false);
    }

    private Collider GetColliderFromObj(GameObject obj) {
        Collider collider = GetComponent<Collider>();
        if (collider == null) {
            collider = obj.GetComponentInChildren<Collider>();
        }
        return collider;
    }
}