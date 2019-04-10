using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCooldown : MonoBehaviour
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
        if (cooldownTimer > 0.0f && transform.parent != null) { // Check if this item is still attached
            cooldownTimer -= Time.deltaTime;
            GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.SetWeaponPercentRechargeRemaining(cooldownTimer / Cooldown);
        }
    }
}
