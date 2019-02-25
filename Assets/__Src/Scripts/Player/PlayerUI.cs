using System.Collections;
using System.Collections.Generic;
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

    private Canvas canvas;
    private int screenNumber;
    private GameObject compassArrow;

    public override void ControlGained() {
        GameObject pref = Resources.Load<GameObject>("UI/PlayerUI");
        canvas = Instantiate(pref).GetComponent<Canvas>();
        SetLayerRecursive(canvas.gameObject, 7 + screenNumber);
        canvas.worldCamera = SplitscreenManager.instance.playerCameras[ScreenNumber - 1].camera;
        canvas.planeDistance = .5f;

        compassArrow = canvas.gameObject.transform.GetChild(1).GetChild(0).gameObject;
    }

    public void SetHealth(float health) {
        canvas.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
    }

    private void SetLayerRecursive(GameObject root, int layer) {
        root.layer = layer;
        foreach (Transform child in root.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
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
        
        compassArrow.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        UpdateCompassDirection();
    }
}
