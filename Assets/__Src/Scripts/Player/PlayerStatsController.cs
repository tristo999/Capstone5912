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
    private float oldHealth;

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
                // Retriggers HealthChanged()
                state.Health = 0; 
            } else if (state.Health > 100) { // Max Health here
                // Retriggers HealthChanged()
                state.Health = 100;
            } else {
                ui.SetHealth(state.Health);
                if (state.Health == 0) {
                    state.Dead = true;
                }

                float change = state.Health - oldHealth;
                if (Math.Abs(change) > 0.0001f) ui.FlashDamageTaken(-change);
                oldHealth = state.Health;
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
        if (entity.isOwner) {
            float change = state.Speed - oldSpeed;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddSpeedText(change, transform.position);
                ui.SetPlayerSpeedStat(state.Speed);
            }

            oldSpeed = state.Speed;
        }
    }

    private void FireRateChanged() {
        if (entity.isOwner) {
            float change = state.FireRate - oldFireRate;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddFireRateText(change, transform.position);
                ui.SetFireRateStat(state.FireRate);
            }
            oldFireRate = state.FireRate;
        }
    }

    private void ProjectileSpeedChanged() {
        if (entity.isOwner) {
            float change = state.ProjectileSpeed - oldProjectileSpeed;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddProjectileSpeedText(change, transform.position);
                ui.SetProjectileSpeedStat(state.ProjectileSpeed);
            }
            oldProjectileSpeed = state.ProjectileSpeed;
        }
    }

    private void ProjectileDamageChanged() {
        if (entity.isOwner) {
            float change = state.ProjectileDamage - oldProjectileDamage;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddProjectileDamageText(change, transform.position);
                ui.SetProjectileDamageStat(state.ProjectileDamage);
            }
            oldProjectileDamage = state.ProjectileDamage;
        }
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

    private void StoreOldStats() {
        oldSpeed = state.Speed;
        oldFireRate = state.FireRate;
        oldProjectileSpeed = state.ProjectileSpeed;
        oldProjectileDamage = state.ProjectileDamage;
        oldHealth = state.Health; 
        // Health is only used for compounded damage taken, not damage dealt or healing received.
    }
}
