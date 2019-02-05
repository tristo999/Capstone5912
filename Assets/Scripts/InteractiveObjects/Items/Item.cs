using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : InteractiveObject
{
    public abstract string ItemName { get; }
    public abstract string ItemDescription { get; }

    public int Id {
        get
        {
            return id;
        }
    }

    public GameObject ItemNameText;
    public GameObject ItemDescriptionText;

    private TextMeshPro nameText;
    private TextMeshPro descriptionText;
    private int id;

    private void Awake() {
        id = ItemManager.Instance.GetId(this);
    }

    private void Start() {
        nameText = Instantiate(ItemNameText).GetComponent<TextMeshPro>();
        descriptionText = Instantiate(ItemDescriptionText).GetComponent<TextMeshPro>();
        nameText.text = ItemName;
        descriptionText.text = ItemDescription;
    }

    private void Update() {
        if (nameText == null) return;
        nameText.transform.position = transform.position + Vector3.up * .5f;
        descriptionText.transform.position = transform.position + Vector3.up * .2f;
        nameText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        descriptionText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
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
}
