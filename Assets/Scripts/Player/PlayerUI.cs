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

    public override void ControlGained() {
        GameObject pref = Resources.Load<GameObject>("UI/PlayerUI");
        canvas = Instantiate(pref).GetComponent<Canvas>();
        SetLayerRecursive(canvas.gameObject, 7 + screenNumber);
        canvas.worldCamera = SplitscreenManager.instance.playerCameras[ScreenNumber - 1].camera;
        canvas.planeDistance = .5f;
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
}
