﻿using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

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

    private static readonly string EMPTY_SLOT_TEXT = "Empty";

    private Canvas canvas;
    private int screenNumber;

    private GameObject compassArrowElement;
    private TextMeshProUGUI healthTextElement;
    private TextMeshProUGUI weaponSlotNameTextElement;
    private TextMeshProUGUI activeItemSlotNameTextElement;

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
    }

    public void SetHealth(float health) {
        healthTextElement.text = health.ToString();
    }

    public void SetWeapon(int weaponId)
    {
        if (weaponId >= 0)
        {
            weaponSlotNameTextElement.text = ItemManager.Instance.items[weaponId].ItemName;
        }
        else
        {
            weaponSlotNameTextElement.text = EMPTY_SLOT_TEXT;
        }
    }

    public void SetActiveItem(int activeItemId)
    {
        if (activeItemId >= 0)
        {
            activeItemSlotNameTextElement.text = ItemManager.Instance.items[activeItemId].ItemName;
        }
        else
        {
            activeItemSlotNameTextElement.text = EMPTY_SLOT_TEXT;
        }
    }

    private void Update()
    {
        UpdateCompassDirection();
    }

    private void UpdateCompassDirection()
    {
        if (!entity.isOwner) return;
        Vector2 direction = new Vector2(-transform.position.x, -transform.position.z);
        float angle = Vector2.Angle(direction, new Vector2(1, 0)) - 90;
        if (transform.position.z > 0)
        {
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

    private GameObject GetCanvasChildByName(string name)
    {
        foreach (Transform child in canvas.gameObject.transform)
        {
            if (String.Equals(child.gameObject.name, name))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
