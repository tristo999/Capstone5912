using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject floatingTextPrefab;

    private static readonly string EMPTY_SLOT_TEXT = "Empty";

    private Canvas canvas;
    private int screenNumber;

    private GameObject compassArrowElement;
    private Image weaponSlotRechargeImage;
    private Image activeItemSlotRechargeImage;
    private TextMeshProUGUI healthTextElement;
    private TextMeshProUGUI weaponSlotNameTextElement;
    private TextMeshProUGUI weaponSlotUsesTextElement;
    private TextMeshProUGUI activeItemSlotNameTextElement;
    private TextMeshProUGUI activeItemSlotUsesTextElement;
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
        weaponSlotRechargeImage = GetCanvasChildByName("Weapon Slot").GetComponentsInChildren<Image>()[1];
        weaponSlotNameTextElement = GetCanvasChildByName("Weapon Slot").GetComponentInChildren<TextMeshProUGUI>();
        weaponSlotUsesTextElement = GetCanvasChildByName("Weapon Slot").GetComponentsInChildren<TextMeshProUGUI>()[1];
        activeItemSlotRechargeImage = GetCanvasChildByName("Active Item Slot").GetComponentsInChildren<Image>()[1];
        activeItemSlotNameTextElement = GetCanvasChildByName("Active Item Slot").GetComponentInChildren<TextMeshProUGUI>();
        activeItemSlotUsesTextElement = GetCanvasChildByName("Active Item Slot").GetComponentsInChildren<TextMeshProUGUI>()[1];
        itemNameTextElement = GetCanvasChildByName("Item Name").GetComponentInChildren<TextMeshProUGUI>();
        itemDescriptionTextElement = GetCanvasChildByName("Item Description").GetComponentInChildren<TextMeshProUGUI>();
        messageElement = GetCanvasChildByName("Message").GetComponent<TextMeshProUGUI>();
    }

    public void SetWeaponPercentRechargeRemaining(float percentChargeRemaining) {
        UpdateRechargeImage(weaponSlotRechargeImage, percentChargeRemaining);
    }

    public void SetActiveItemPercentRechargeRemaining(float percentChargeRemaining) {
        UpdateRechargeImage(activeItemSlotRechargeImage, percentChargeRemaining);
    }

    public void SetWeaponUsesRemaining(int usesRemaining) {
        UpdateTextUses(weaponSlotUsesTextElement, usesRemaining);
    }

    public void SetActiveItemUsesRemaining(int usesRemaining) {
        UpdateTextUses(activeItemSlotUsesTextElement, usesRemaining);
    }

    public void SetHealth(float health) {
        healthTextElement.text = health.ToString();
    }

    public void SetWeapon(int weaponId) {
        UpdateItemNameText(weaponSlotNameTextElement, weaponId);
        SetWeaponPercentRechargeRemaining(0);
        SetWeaponUsesRemaining(-1);
    }

    public void SetActiveItem(int activeItemId) {
        UpdateItemNameText(activeItemSlotNameTextElement, activeItemId);
        SetActiveItemPercentRechargeRemaining(0);
        SetActiveItemUsesRemaining(-1);
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

    public void AddStatText(string message, Vector3 hitPosition) {
        FloatingTextController statText = Instantiate(floatingTextPrefab).GetComponent<FloatingTextController>();
        statText.AddToCanvas(canvas);
        statText.SetPosition3d(hitPosition, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        statText.SetColor(Color.white);
        statText.SetText(message);
    }

    public void AddHealText(float healAmount, Vector3 hitPosition) {
        FloatingTextController healText = Instantiate(floatingTextPrefab).GetComponent<FloatingTextController>();
        healText.AddToCanvas(canvas);
        healText.SetPosition3d(hitPosition, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        healText.SetColor(Color.green);
        healText.SetText("+" + healAmount);
    }

    public void AddDamageText(float damage, Vector3 hitPosition) {
        FloatingTextController damageText = Instantiate(floatingTextPrefab).GetComponent<FloatingTextController>();
        damageText.AddToCanvas(canvas);
        damageText.SetPosition3d(hitPosition, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        damageText.SetColor(Color.red);
        damageText.SetText("-" + damage);
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

    private void UpdateRechargeImage(Image image, float percentChargeRemaining) {
        image.fillAmount = percentChargeRemaining;
    }

    private void UpdateTextUses(TextMeshProUGUI textElement, int usesRemaining) {
        if (usesRemaining >= 0) {
            textElement.text = "" + usesRemaining;
        } else {
            textElement.text = "";
        }
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
