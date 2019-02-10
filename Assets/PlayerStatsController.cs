using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsController : Bolt.EntityEventListener<IPlayerState>
{
    private PlayerMovementController movementController;

    public override void Attached() {
        movementController = GetComponent<PlayerMovementController>();

        state.Speed = 1f;
        state.FireRate = 1f;
        state.ProjectileSpeed = 1f;
        state.ProjectileDamage = 1f;

        state.AddCallback("Health", HealthChanged);
        state.AddCallback("Speed", SpeedChanged);
        state.AddCallback("FireRate", FireRateChanged);
        state.AddCallback("ProjectileSpeed", ProjectileSpeedChanged);
        state.AddCallback("ProjectileDamage", ProjectileDamageChanged);
    }

    private void HealthChanged() {

    }

    private void SpeedChanged() {

    }

    private void FireRateChanged() {

    }

    private void ProjectileSpeedChanged() {

    }

    private void ProjectileDamageChanged() {

    }

    public override void OnEvent(PlayerHit evnt) {
        if (entity.isOwner) {
            state.Health -= evnt.Damage;
        }
    }
}
