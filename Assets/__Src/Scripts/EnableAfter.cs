using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAfter : MonoBehaviour
{
    public float UnTriggerTime;
    private int startFrame;
    private int activateFrame;
    private Collider col;

    private void Awake() {
        startFrame = BoltNetwork.Frame;
        activateFrame = startFrame + (int)(BoltNetwork.FramesPerSecond * UnTriggerTime);
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (col.isTrigger && BoltNetwork.Frame > activateFrame)
            col.isTrigger = false;
    }
}
