using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakofWhoosh : ActiveItem
{
    private ActiveTimeout timeout;
    private ActiveCooldown cooldown;
    private ActiveUses uses;

    public override void ActivateHold() { }
    public override void ActivateRelease() { }

    public override void ActiveDown() {
        ActivateCloak();
    }

    public override void OnEquip() {
        GetComponent<Cloth>().capsuleColliders = new CapsuleCollider[] { Owner.GetComponent<CapsuleCollider>() };
        timeout = GetComponent<ActiveTimeout>();
        cooldown = GetComponent<ActiveCooldown>();
        uses = GetComponent<ActiveUses>();
        timeout.OnTimeout += DeactivateCloak;
    }

    public override void OnDequip() {
        if (timeout.InTimeout) {
            DeactivateCloak();
        }
    }

    private void ActivateCloak() {
        if (timeout.InTimeout || !cooldown.Ready) return;

        Owner.GetComponent<PlayerStatsController>().state.Speed += 0.7f;

        timeout.StartTimeout();
        uses.Use();
    }

    private void DeactivateCloak() {
        cooldown.ResetCooldown();

        Owner.GetComponent<PlayerStatsController>().state.Speed -= 0.7f;
    }
}
