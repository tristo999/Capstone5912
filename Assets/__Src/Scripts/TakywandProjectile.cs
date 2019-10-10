using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakywandProjectile : NetworkBehaviour
{
    public float maxSizeScale;

    private float aliveFor = 0f;
    private float lifetime = .42f;

    public void FixedUpdate() {
        float scale = .6f + (aliveFor / lifetime) * maxSizeScale;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
