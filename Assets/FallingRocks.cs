using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRocks : MonoBehaviour
{
    public ParticleSystem system;
    public List<GameObject> rocks = new List<GameObject>();
    public float minScale;
    public float maxScale;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            
        }
        if (Input.GetKeyUp(KeyCode.K)) {
            system.Stop();
        }

    }

    public void DoTheRocks() {
        // Do them.

    }
}
