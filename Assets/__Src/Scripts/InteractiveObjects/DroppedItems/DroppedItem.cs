using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DroppedItem : InteractiveObject
{

    public int Id { get; set; }

    public GameObject ItemNameText;
    private TextMeshPro nameText;

    private void Start() {
        
    }

    private void Update() {
        if (nameText == null) return;
        nameText.transform.position = transform.position + Vector3.up * 1.5f;
        nameText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        nameText.transform.position = transform.position + nameText.transform.up * .4f; 
    }

    public override void FocusGained() {
        // TEMP: Disabling the text attached to dropped items this simple way for Timebox 4.
        // nameText = Instantiate(ItemNameText).GetComponent<TextMeshPro>();
        // nameText.text = ItemManager.Instance.items[Id].ItemName;
        base.FocusGained();
    }

    public override void FocusLost() {
        if (nameText != null) Destroy(nameText.gameObject);
        base.FocusLost();
    }

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        state.AddCallback("ItemId", IdChanged);
    }

    public override void DoInteract(BoltEntity bEntity) {
        DestroyPickup evnt = DestroyPickup.Create(entity);
        evnt.Send();
    }

    public override void OnEvent(DestroyPickup evnt)
    {
        if (nameText != null) Destroy(nameText.gameObject);
        BoltNetwork.Destroy(gameObject);
    }

    private void IdChanged() {
        Id = state.ItemId;
        if (nameText != null) nameText.text = ItemManager.Instance.items[Id].ItemName;
    }
}
