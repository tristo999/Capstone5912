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
        if (entity.isOwner) {
            healthText = Instantiate(HealthCanvas).GetComponentInChildren<TextMeshProUGUI>();
            movementController = GetComponent<PlayerMovementController>();

            state.Speed = 1f;
            state.FireRate = 1f;
            state.ProjectileSpeed = 1f;
            state.ProjectileDamage = 1f;
            state.Health = StartingHealth;
        }
        
        state.AddCallback("Health", HealthChanged);
        state.AddCallback("Speed", SpeedChanged);
        state.AddCallback("FireRate", FireRateChanged);
        state.AddCallback("ProjectileSpeed", ProjectileSpeedChanged);
        state.AddCallback("ProjectileDamage", ProjectileDamageChanged);
    }

    private void HealthChanged() {
        if (entity.isOwner) {
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
