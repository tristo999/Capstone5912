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
            foreach (Player p in ReInput.players.Players) {
                p.SetVibration(0, 1, 2);
            }
        }

    }

    public void DoTheRocks() {
        // Do them.

    }
}
