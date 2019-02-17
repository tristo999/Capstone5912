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

    public override void Attached() {
        movementController = GetComponent<PlayerMovementController>();

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

    public override void ControlGained() {
        healthText = Instantiate(HealthCanvas).GetComponentInChildren<TextMeshProUGUI>();
    }

    private void HealthChanged() {
        if (entity.hasControl) {
            healthText.text = state.Health.ToString();
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
