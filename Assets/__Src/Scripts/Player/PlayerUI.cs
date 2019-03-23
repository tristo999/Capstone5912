using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerUI : Bolt.EntityBehaviour<IPlayerState>
{
    public int ScreenNumber
    {
        get
        {
            return screenNumber;
        }
        set
        {
            screenNumber = value;
            if (canvas) {
                SetLayerRecursive(canvas.gameObject, 7 + screenNumber);
            }
        }
    }

    public GameObject damageTextPrefab;

    private static readonly string EMPTY_SLOT_TEXT = "Empty";

    private Canvas canvas;
    private int screenNumber;

    private GameObject compassArrowElement;
    private TextMeshProUGUI healthTextElement;
    private TextMeshProUGUI weaponSlotNameTextElement;
    private TextMeshProUGUI activeItemSlotNameTextElement;
    private TextMeshProUGUI itemNameTextElement;
    private TextMeshProUGUI itemDescriptionTextElement;
    private TextMeshProUGUI messageElement;

    public override void ControlGained() {
        GameObject pref = Resources.Load<GameObject>("UI/PlayerUI");
        canvas = Instantiate(pref).GetComponent<Canvas>();
        SetLayerRecursive(canvas.gameObject, 7 + screenNumber);
        canvas.worldCamera = SplitscreenManager.instance.playerCameras[ScreenNumber - 1].camera;
        canvas.planeDistance = .5f;

        compassArrowElement = GetCanvasChildByName("Compass").transform.GetChild(0).gameObject;
        healthTextElement = GetCanvasChildByName("Health").GetComponentInChildren<TextMeshProUGUI>();
        weaponSlotNameTextElement = GetCanvasChildByName("Weapon Slot").GetComponentInChildren<TextMeshProUGUI>();
        activeItemSlotNameTextElement = GetCanvasChildByName("Active Item Slot").GetComponentInChildren<TextMeshProUGUI>();
        itemNameTextElement = GetCanvasChildByName("Item Name").GetComponentInChildren<TextMeshProUGUI>();
        itemDescriptionTextElement = GetCanvasChildByName("Item Description").GetComponentInChildren<TextMeshProUGUI>();
        messageElement = GetCanvasChildByName("Message").GetComponent<TextMeshProUGUI>();
    }

    public void SetHealth(float health) {
        healthTextElement.text = health.ToString();
    }

    public void SetWeapon(int weaponId) {
        UpdateItemNameText(weaponSlotNameTextElement, weaponId);
    }

    public void SetActiveItem(int activeItemId) {
        UpdateItemNameText(activeItemSlotNameTextElement, activeItemId);
    }

    public void SetItemFullDescription(int itemId) { 
        if (itemId >= 0) { 
            UpdateItemNameText(itemNameTextElement, itemId);

            ItemDefinition item = ItemManager.Instance.items[itemId];
            itemDescriptionTextElement.text = item.ItemDescription;
        } else { 
            itemNameTextElement.text = "";
            itemDescriptionTextElement.text = "";
        }
    }

    public void AddDamageText(float damage, Vector3 hitPosition) {
        DamageTextController damageText = Instantiate(damageTextPrefab).GetComponent<DamageTextController>();
        damageText.AddToCanvas(canvas);
        damageText.SetPosition(hitPosition, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        damageText.SetDamage(damage);
    }

    public void DisplayMessage(string message, float displayInterval, float displayIntroDelay = 0f, TweenCallback callback = null) {
        messageElement.text = message;
        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.AppendInterval(displayIntroDelay);
        fadeSeq.Append(messageElement.DOFade(1f, 2f));
        fadeSeq.AppendInterval(displayInterval);
        fadeSeq.Append(messageElement.DOFade(0f, 2f));
        fadeSeq.AppendCallback(() => messageElement.text = "");
        if (callback != null) {
            fadeSeq.AppendCallback(callback);
        }

        fadeSeq.Play();
    }

    private void Update() { 
        UpdateCompassDirection();
    }

    private void UpdateItemNameText(TextMeshProUGUI textElement, int itemId) {
        if (itemId >= 0) {
            ItemDefinition item = ItemManager.Instance.items[itemId];
            textElement.text = item.ItemName;
            textElement.color = ItemDefinition.RarityColors[(int)item.Rarity];
        } else {
            textElement.text = EMPTY_SLOT_TEXT;
            textElement.color = Color.white;
        }
    }

    private void UpdateCompassDirection() { 
        if (!entity.isOwner) return;
        Vector2 direction = new Vector2(-transform.position.x, -transform.position.z);
        float angle = Vector2.Angle(direction, new Vector2(1, 0)) - 90;
        if (transform.position.z > 0) { 
            angle = 180 - angle;
        }

        compassArrowElement.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void SetLayerRecursive(GameObject root, int layer) {
        root.layer = layer;
        foreach (Transform child in root.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private GameObject GetCanvasChildByName(string name) { 
        foreach (Transform child in canvas.gameObject.transform) { 
            if (String.Equals(child.gameObject.name, name)) { 
                return child.gameObject;
            }
        }
        return null;
    }
}
