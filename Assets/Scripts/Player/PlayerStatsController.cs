using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsController : Bolt.EntityEventListener<IPlayerState>
{
    public Canvas HealthCanvas;
    public float StartingHealth;
    private PlayerMovementController movementController;
    private TextMeshProUGUI healthText;
    private PlayerUI ui;

    public override void Attached() {
        movementController = GetComponent<PlayerMovementController>();
        ui = GetComponent<PlayerUI>();

        state.Speed = 1f;
        state.FireRate = 1f;
        state.ProjectileSpeed = 1f;
        state.ProjectileDamage = 1f;
        state.Health = StartingHealth;

        // Move these out of the isControllerOrOwner statement if you want all players to receive these callbacks.
        state.AddCallback("Health", HealthChanged);
        state.AddCallback("Speed", SpeedChanged);
        state.AddCallback("FireRate", FireRateChanged);
        state.AddCallback("ProjectileSpeed", ProjectileSpeedChanged);
        state.AddCallback("ProjectileDamage", ProjectileDamageChanged);
    }

    private void HealthChanged() {
        if (entity.hasControl) {
            ui.SetHealth(state.Health);
        }
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
        state.Health -= evnt.Damage;
    }
}
