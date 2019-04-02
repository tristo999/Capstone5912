using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCooldown : MonoBehaviour
{
    public float Cooldown;
    public bool Ready
    {
        get
        {
            return cooldownTimer <= 0.0f;
        }
    }
    private float cooldownTimer = 0f;

    public void ResetCooldown() {
        cooldownTimer = Cooldown;
    }

    private void Update() {
        if (cooldownTimer > 0.0f) {
            cooldownTimer -= Time.deltaTime;
            GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.SetActiveItemPercentRechargeRemaining(cooldownTimer / Cooldown);
        }
    }
}
