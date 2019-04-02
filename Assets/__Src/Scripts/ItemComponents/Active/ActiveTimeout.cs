using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTimeout : MonoBehaviour
{
    public float Timeout;
    public bool InTimeout
    {
        get
        {
            return timeoutTimer > 0f;
        }
    }
    public delegate void TimeoutDel();
    public TimeoutDel OnTimeout;
    [HideInInspector]
    public float timeoutTimer = 0f;

    public void StartTimeout() {
        timeoutTimer = Timeout;
        GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.SetActiveItemPercentRechargeRemaining(1);
    }

    private void Update() {
        if (timeoutTimer > 0.0f) {
            timeoutTimer -= Time.deltaTime;
            if (timeoutTimer <= 0.0f) {
                OnTimeout();
            }
        }
    }
}
