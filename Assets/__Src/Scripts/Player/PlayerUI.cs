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
    private Dictionary<Vector3, int> floatingTextStackDictionary;

    private static readonly string EMPTY_SLOT_TEXT = "Empty";
    private static readonly float MAX_PAIN = 20; // In damage points.
    private static readonly float LOW_HP_THESHOLD = 15f;

    private Canvas canvas;
    private int screenNumber;

    private float painMagnitude = 0;

    private GameObject compassArrowElement;
    private Image compassGlowImage;
    private Image damageTakenImage;
    private Image weaponSlotImage;
    private Image weaponSlotRechargeImage;
    private Image activeItemSlotImage;
    private Image activeItemSlotRechargeImage;
    private Image healthOrbFillImage;
    private Image healthOrbSurfaceFillImage;
    private Image healthOrbGlowStaticImage;
    private TextMeshProUGUI healthTextElement;
    private TextMeshProUGUI weaponSlotNameTextElement;
    private TextMeshProUGUI weaponSlotUsesTextElement;
    private TextMeshProUGUI activeItemSlotNameTextElement;
    private TextMeshProUGUI activeItemSlotUsesTextElement;
    private TextMeshProUGUI itemNameTextElement;
    private TextMeshProUGUI itemTypeTextElement;
    private TextMeshProUGUI itemDescriptionTextElement;
    private TextMeshProUGUI messageElement;
    
    private TextMeshProUGUI playerSpeedStatTextElement;
    private TextMeshProUGUI fireRateStatTextElement;
    private TextMeshProUGUI projectileSpeedStatTextElement;
    private TextMeshProUGUI projectileDamageStatTextElement;

    public override void ControlGained() {
        GameObject pref = Resources.Load<GameObject>("UI/PlayerUI");
        canvas = Instantiate(pref).GetComponent<Canvas>();
        SetLayerRecursive(canvas.gameObject, 7 + screenNumber);
        canvas.worldCamera = SplitscreenManager.instance.playerCameras[ScreenNumber - 1].camera;
        canvas.planeDistance = .5f;
        
        compassArrowElement = GetCanvasChildByName("Compass").transform.GetChild(0).gameObject;
        compassGlowImage = GetCanvasChildByName("Compass Glow").GetComponent<Image>();
        damageTakenImage = GetCanvasChildByName("Damage Taken").GetComponent<Image>();
        healthTextElement = GetCanvasChildByName("Health").GetComponentInChildren<TextMeshProUGUI>();
        healthOrbFillImage = GetCanvasChildByName("Health Orb Fill").GetComponent<Image>();
        healthOrbSurfaceFillImage = GetCanvasChildByName("Health Orb Surface Fill").GetComponent<Image>();
        healthOrbGlowStaticImage = GetCanvasChildByName("Health Orb Glow Static").GetComponent<Image>();
        weaponSlotImage = GetCanvasChildByName("Weapon Slot").GetComponent<Image>();
        weaponSlotRechargeImage = GetCanvasChildByName("Weapon Slot").GetComponentsInChildren<Image>()[1];
        weaponSlotNameTextElement = GetCanvasChildByName("Weapon Slot").GetComponentInChildren<TextMeshProUGUI>();
        weaponSlotUsesTextElement = GetCanvasChildByName("Weapon Slot").GetComponentsInChildren<TextMeshProUGUI>()[1];
        activeItemSlotImage = GetCanvasChildByName("Active Item Slot").GetComponent<Image>();
        activeItemSlotRechargeImage = GetCanvasChildByName("Active Item Slot").GetComponentsInChildren<Image>()[1];
        activeItemSlotNameTextElement = GetCanvasChildByName("Active Item Slot").GetComponentInChildren<TextMeshProUGUI>();
        activeItemSlotUsesTextElement = GetCanvasChildByName("Active Item Slot").GetComponentsInChildren<TextMeshProUGUI>()[1];
        itemNameTextElement = GetCanvasChildByName("Item Name").GetComponentInChildren<TextMeshProUGUI>();
        itemTypeTextElement = GetCanvasChildByName("Item Type").GetComponentInChildren<TextMeshProUGUI>();
        itemDescriptionTextElement = GetCanvasChildByName("Item Description").GetComponentInChildren<TextMeshProUGUI>();
        messageElement = GetCanvasChildByName("Message").GetComponent<TextMeshProUGUI>();

        playerSpeedStatTextElement = GetCanvasChildByName("Player Speed Stat").GetComponentInChildren<TextMeshProUGUI>();
        fireRateStatTextElement = GetCanvasChildByName("Fire Rate Stat").GetComponentInChildren<TextMeshProUGUI>();
        projectileSpeedStatTextElement = GetCanvasChildByName("Projectile Speed Stat").GetComponentInChildren<TextMeshProUGUI>();
        projectileDamageStatTextElement = GetCanvasChildByName("Projectile Damage Stat").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetPlayerSpeedStat(float speed) {
        UpdateStatText(playerSpeedStatTextElement, "Player Speed", speed);
    }

    public void SetFireRateStat(float fireRate) {
        UpdateStatText(fireRateStatTextElement, "Fire Rate", fireRate);
    }

    public void SetProjectileSpeedStat(float projectileSpeed) {
        UpdateStatText(projectileSpeedStatTextElement, "Projectile Speed", projectileSpeed);
    }

    public void SetProjectileDamageStat(float projectileDamage) {
        UpdateStatText(projectileDamageStatTextElement, "Projectile Damage", projectileDamage);
    }

    public void SetWeaponPercentRechargeRemaining(float percentChargeRemaining) {
        UpdateRechargeImage(weaponSlotRechargeImage, percentChargeRemaining);

        // Optionally change tile brightness to indicate charge as well
        // weaponSlotImage.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f), Color.white, (float)Math.Pow(1 - percentChargeRemaining, 14));
    }

    public void SetActiveItemPercentRechargeRemaining(float percentChargeRemaining) {
        UpdateRechargeImage(activeItemSlotRechargeImage, percentChargeRemaining);

        // activeItemSlotImage.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f), Color.white, (float)Math.Pow(1 - percentChargeRemaining, 14));
    }

    public void SetWeaponUsesRemaining(int usesRemaining) {
        UpdateTextUses(weaponSlotUsesTextElement, usesRemaining);
    }

    public void SetActiveItemUsesRemaining(int usesRemaining) {
        UpdateTextUses(activeItemSlotUsesTextElement, usesRemaining);
    }

    public void SetHealth(float health) {
        healthTextElement.text = $"{Math.Ceiling(health)}";

        float fillPercent = 0.1f + (health / 100) * 0.8f;

        healthOrbFillImage.fillAmount = fillPercent;
        healthOrbSurfaceFillImage.fillAmount = fillPercent + 0.012f;
        healthOrbGlowStaticImage.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f, 0.3f), new Color(0.55f, 0.55f, 0.55f, 1f), (1 - fillPercent) * 1.2f);
    }

    public void SetWeapon(int weaponId) {
        UpdateItemNameText(weaponSlotNameTextElement, weaponId);
        if (weaponId == -1) {
            SetWeaponPercentRechargeRemaining(1);
            weaponSlotImage.color = new Color(0.75f, 0.75f, 0.75f);
        } else {
            SetWeaponPercentRechargeRemaining(0);
            weaponSlotImage.color = Color.white;
        }
        SetWeaponUsesRemaining(-1);
    }

    public void SetActiveItem(int activeItemId) {
        UpdateItemNameText(activeItemSlotNameTextElement, activeItemId);
        if (activeItemId == -1) {
            SetActiveItemPercentRechargeRemaining(1);
            activeItemSlotImage.color = new Color(0.75f, 0.75f, 0.75f);
        } else {
            SetActiveItemPercentRechargeRemaining(0);
            activeItemSlotImage.color = Color.white;
        }
        SetActiveItemUsesRemaining(-1);
    }

    public void SetItemFullDescription(int itemId) { 
        if (itemId >= 0) { 
            UpdateItemNameText(itemNameTextElement, itemId);

            ItemDefinition item = ItemManager.Instance.items[itemId];

            itemTypeTextElement.text = $"{item.Rarity.ToString()} {item.Type.ToString()}";
            itemTypeTextElement.color = ItemDefinition.RarityColors[(int)item.Rarity];

            itemDescriptionTextElement.text = item.ItemDescription;
        } else { 
            itemNameTextElement.text = "";
            itemTypeTextElement.text = "";
            itemDescriptionTextElement.text = "";
        }
    }

    public void AddSpeedText(float speedChange, Vector3 position3d) { AddStatModText(speedChange, "speed", position3d); }

    public void AddFireRateText(float fireRateChange, Vector3 position3d) { AddStatModText(fireRateChange, "fire rate", position3d); }

    public void AddProjectileSpeedText(float projectileSpeedChange, Vector3 position3d) { AddStatModText(projectileSpeedChange, "projectile speed", position3d); }

    public void AddProjectileDamageText(float projectileDamageChange, Vector3 position3d) { AddStatModText(projectileDamageChange, "damage", position3d); }

    public void AddDamageText(float damage, Vector3 position3d, bool showStatName = false) {
        if (entity.isOwner) {
            if (Math.Abs(damage) > 0.0001f) {
                if (damage > 0) {
                    AddFloatingText($"-{FloatToOneDecimalPrecision(damage)}{(showStatName ? " health" : "")}", position3d, Color.red);
                } else {
                    AddFloatingText($"+{FloatToOneDecimalPrecision(-damage)}{(showStatName ? " health" : "")}", position3d, Color.green);
                }
            }
        }
    }

    public void AddFloatingMessageText(string message, Vector3 position3d) {
        AddFloatingText(message, position3d, Color.white);
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

    public void FlashDamageTaken(float damage) {
        painMagnitude += damage;
        AddFlashDamageTakenText(damage);
    }

    public override void SimulateOwner() {
        UpdateCompassDirection();
        UpdateDamageOverlay();
        floatingTextStackDictionary = new Dictionary<Vector3, int>();
    }

    public void AddStatModText(float modAmount, string statName, Vector3 position3d) {
        if (modAmount >= 0) {
            AddFloatingText($"+{GetStatPercentFormat(modAmount)}% {statName}", position3d, Color.green);
        } else {
            AddFloatingText($"-{GetStatPercentFormat(-modAmount)}% {statName}", position3d, Color.red);
        }
    }

    private void AddFloatingText(string message, Vector3 position3d, Color color) {
        if (floatingTextStackDictionary.ContainsKey(position3d)) {
            floatingTextStackDictionary[position3d]++;
        } else {
            floatingTextStackDictionary[position3d] = 0;
        }
        float stackPositionOffset = floatingTextStackDictionary[position3d];

        FloatingTextController text = Instantiate(floatingTextPrefab).GetComponent<FloatingTextController>();
        text.AddToCanvas(canvas);
        text.SetPosition3d(position3d, stackPositionOffset, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        text.SetColor(color);
        text.SetText(message);
    }

    private void UpdateStatText(TextMeshProUGUI textElement, string statName, float statValue) {
        textElement.text = $"{statName}: {GetStatPercentFormat(statValue)}%";

        if (statValue >= 1) {
            textElement.color = Color.Lerp(Color.white, Color.green, (statValue - 1) * 2f);
        } else {
            textElement.color = Color.Lerp(Color.white, Color.red, (1 - statValue) * 2f);
        }
    }

    private void UpdateRechargeImage(Image image, float percentChargeRemaining) {
        image.fillAmount = 1 - percentChargeRemaining;
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
        Vector2 positionTopDown = new Vector2(-transform.position.x, -transform.position.z);

        float angle = Vector2.Angle(positionTopDown, new Vector2(1, 0)) - 90;
        if (transform.position.z > 0) { 
            angle = 180 - angle;
        }
        compassArrowElement.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float distance = Vector2.Distance(positionTopDown, Vector2.zero);
        float percentGlow = (float)Math.Pow(1 - (distance / 250f), 0.7f);
        compassGlowImage.color = Color.Lerp(new Color(0.6603774f, 0.3519936f, 0.3519936f, 0f), new Color(0.6603774f, 0.3519936f, 0.3519936f, 1f), percentGlow);
    }

    private void AddFlashDamageTakenText(float damage) {
        RectTransform parentRectTransform = (RectTransform)healthTextElement.rectTransform.parent.transform;
        Vector2 position2d = new Vector2(0.07f, 0.16f); // TEMP can't get this to line up and wasting time.

        FloatingTextController text = Instantiate(floatingTextPrefab).GetComponent<FloatingTextController>();
        text.AddToCanvas(canvas);
        text.SetPosition2d(position2d, SplitscreenManager.instance.GetEntityCamera(entity).camera);
        if (damage > 0) {
            text.SetText($"-{(int)Math.Round(damage)}");
            text.SetColor(Color.red);
        } else {
            text.SetText($"+{(int)Math.Round(-damage)}");
            text.SetColor(Color.green);
        }
    }

    private void UpdateDamageOverlay() {
        if (painMagnitude > MAX_PAIN) painMagnitude = MAX_PAIN;
        if (painMagnitude < 0) painMagnitude = 0;

        float painAlpha = (float)Math.Sqrt(painMagnitude / MAX_PAIN) * 0.25f;
        float lowHpAlpha = (1 - Math.Min(state.Health / LOW_HP_THESHOLD, 1)) * 0.175f;
        float alpha = painAlpha + lowHpAlpha;
        damageTakenImage.color = new Color(144, 0, 0, alpha);

        painMagnitude -= 0.45f * MAX_PAIN * Time.deltaTime;
    }

    private int GetStatPercentFormat(float statValue) {
        return (int)Math.Round(statValue * 100);
    }

    private float FloatToOneDecimalPrecision(float num) {
        return ((int)Math.Round(num * 10)) / 10f;
    }

    private void SetLayerRecursive(GameObject root, int layer) {
        root.layer = layer;
        foreach (Transform child in root.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private GameObject GetCanvasChildByName(string name) {
        return GetObjectChildByName(name, canvas.gameObject);
    }

    private GameObject GetObjectChildByName(string name, GameObject obj) { 
        foreach (Transform child in obj.transform) { 
            if (String.Equals(child.gameObject.name, name)) { 
                return child.gameObject;
            } else {
                GameObject getFromChild = GetObjectChildByName(name, child.gameObject);
                if (getFromChild) {
                    return getFromChild;
                }
            }
        }
        return null;
    }
}
