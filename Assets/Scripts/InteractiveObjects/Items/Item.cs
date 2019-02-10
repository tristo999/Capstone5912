using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : InteractiveObject
{

    public int Id { get; set; }

    public GameObject ItemNameText;
    public GameObject ItemDescriptionText;

    private TextMeshPro nameText;
    private TextMeshPro descriptionText;

    private void Start() {
        
    }

    private void Update() {
        if (nameText == null) return;
        nameText.transform.position = transform.position + Vector3.up * .5f;
        descriptionText.transform.position = transform.position + Vector3.up * .2f;
        nameText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        descriptionText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public override void FocusGained() {
        nameText = Instantiate(ItemNameText).GetComponent<TextMeshPro>();
        descriptionText = Instantiate(ItemDescriptionText).GetComponent<TextMeshPro>();
        nameText.text = ItemManager.Instance.items[Id].ItemName;
        descriptionText.text = ItemManager.Instance.items[Id].ItemDescription;
        base.FocusGained();
    }

    public override void FocusLost() {
        if (descriptionText != null)
            Destroy(descriptionText.gameObject);
        if (nameText != null)
            Destroy(nameText.gameObject);
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
        if (descriptionText != null)
            Destroy(descriptionText.gameObject);
        if (nameText != null)
            Destroy(nameText.gameObject);
        BoltNetwork.Destroy(gameObject);
    }

    private void IdChanged() {
        Id = state.ItemId;
        if (descriptionText != null)
            descriptionText.text = ItemManager.Instance.items[Id].ItemDescription;
        if (nameText != null)
            nameText.text = ItemManager.Instance.items[Id].ItemName;
    }
}
