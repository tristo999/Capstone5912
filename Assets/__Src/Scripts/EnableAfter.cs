using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAfter : NetworkBehaviour
{
    public float UnTriggerTime;
    private double startTime;
    private double activateTime;
    private Collider col;

    private void Awake() {
        startTime = NetworkTime.time;
        activateTime = startTime + UnTriggerTime;
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (col.isTrigger && NetworkTime.time > activateTime)
            col.isTrigger = false;
    }
}
