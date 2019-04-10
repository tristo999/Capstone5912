using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class PlayerStatsController : Bolt.EntityEventListener<IPlayerState>
{
    public Canvas HealthCanvas;
    public float StartingHealth;
    public Renderer robeAndHat;
    [HideInInspector]
    public PlayerUI ui;

    private PlayerMovementController movementController;

    private float oldSpeed;
    private float oldFireRate;
    private float oldProjectileSpeed;
    private float oldProjectileDamage;

    public override void Attached() {
        movementController = GetComponent<PlayerMovementController>();
        ui = GetComponent<PlayerUI>();

        if (entity.isOwner) {
            state.Speed = 1f;
            state.FireRate = 1f;
            state.ProjectileSpeed = 1f;
            state.ProjectileDamage = 1f;
            state.Health = StartingHealth;
            state.PlayerId = -1;

            StoreOldStats();
        }

        state.AddCallback("Health", HealthChanged);
        state.AddCallback("Speed", SpeedChanged);
        state.AddCallback("FireRate", FireRateChanged);
        state.AddCallback("ProjectileSpeed", ProjectileSpeedChanged);
        state.AddCallback("ProjectileDamage", ProjectileDamageChanged);
        state.AddCallback("Color", ColorChanged);
        state.AddCallback("PlayerId", IdChanged);
        state.AddCallback("Dead", PlayerDied);
        if (entity.isOwner) {
            state.AddCallback("WeaponId", WeaponIdChanged);
            state.AddCallback("ActiveId", ActiveIdChanged);
        }
    }

    private void WeaponIdChanged()
    {
        if (state.WeaponId >= 0)
        {
            ItemDefinition item = ItemManager.Instance.items[state.WeaponId];
            state.Health += item.HealthModifier;
            state.Speed += item.SpeedModifier;
            state.FireRate += item.FireRateModifier;
            state.ProjectileSpeed += item.ProjectileSpeedModifier;
            state.ProjectileDamage += item.DamageModifier;
        }
    }

    private void ActiveIdChanged() {
        if (state.ActiveId >= 0)
        {
            ItemDefinition item = ItemManager.Instance.items[state.ActiveId];
            state.Health += item.HealthModifier;
            state.Speed += item.SpeedModifier;
            state.FireRate += item.FireRateModifier;
            state.ProjectileSpeed += item.ProjectileSpeedModifier;
            state.ProjectileDamage += item.DamageModifier;
        }
    }

    private void IdChanged() {
        if (state.PlayerId < 0) return;
        GameMaster.instance.PlayerIdChange(entity, state.PlayerId);
    }

    private void ColorChanged() {
        robeAndHat.material.color = state.Color;
    }

    private void HealthChanged() {
        if (entity.isOwner) {
            if (state.Health < 0) {
                state.Health = 0;
            }
            ui.SetHealth(state.Health);
            if (state.Health == 0) {
                state.Dead = true;
            }
        }
    }

    private void PlayerDied() {
        if (entity.isOwner) {
            if (state.Dead) {
                ui.DisplayMessage("You Died.", 4f, 2f, () => SplitscreenManager.instance.SetCameraToSpectator(ui.ScreenNumber-1, movementController.localPlayer));
                movementController.localPlayer.StopVibration();
            }
        }
    }

    private void SpeedChanged() {
        float change = state.Speed - oldSpeed;
        if (entity.isOwner && Math.Abs(change) > 0.0001f) ui.AddSpeedText(change, transform.position);
    }

    private void FireRateChanged() {
        float change = state.FireRate - oldFireRate;
        if (entity.isOwner && Math.Abs(change) > 0.0001f) ui.AddFireRateText(change, transform.position);
    }

    private void ProjectileSpeedChanged() {
        float change = state.ProjectileSpeed - oldProjectileSpeed;
        if (entity.isOwner && Math.Abs(change) > 0.0001f) ui.AddProjectileSpeedText(change, transform.position);
    }

    private void ProjectileDamageChanged() {
        float change = state.ProjectileDamage - oldProjectileDamage;
        if (entity.isOwner && Math.Abs(change) > 0.0001f) ui.AddProjectileDamageText(change, transform.position);
    }

    public override void OnEvent(DamageEntity evnt) {
        if (state.Health > 0) {
            if (entity.isOwner) {
                if (evnt.Damage > 0) {
                    movementController.localPlayer.SetVibration(0, 1f, .1f);
                    movementController.localPlayer.SetVibration(1, 1f, .1f);
                    movementController.localPlayer.SetVibration(2, 1f, .1f);
                    movementController.localPlayer.SetVibration(3, 1f, .1f);
                }
                state.Health -= evnt.Damage;
            }
            if (evnt.Owner && evnt.Owner.isOwner) {
                PlayerUI ui = evnt.Owner.GetComponent<PlayerUI>();
                ui.AddDamageText(evnt.Damage, evnt.HitPosition);
            }
        }
    }

    // Store stat changes relative to previous update to handle proper 
    // item swap comparisons (excludes health value special case).
    private void StoreOldStats() {
        oldSpeed = state.Speed;
        oldFireRate = state.FireRate;
        oldProjectileSpeed = state.ProjectileSpeed;
        oldProjectileDamage = state.ProjectileDamage;
    }

    void LateUpdate() {
        if (entity.isOwner) {
            StoreOldStats();
        }
    }
}
