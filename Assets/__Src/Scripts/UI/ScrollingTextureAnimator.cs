using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingTextureAnimator : MonoBehaviour { 
    public float verticalSpeed;

    RawImage healthOrbAnimation;

    public void Start() {
        healthOrbAnimation = GetComponent<RawImage>();
    }

    public void Update() {
        Rect currentUV = healthOrbAnimation.uvRect;
        currentUV.y -= Time.deltaTime * verticalSpeed;

        if (currentUV.y <= -1f || currentUV.y >= 1f) {
            currentUV.y = 0f;
        }

        healthOrbAnimation.uvRect = currentUV;
    }
}
