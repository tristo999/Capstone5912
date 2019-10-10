using System;
using UnityEngine;
using Mirror;

public class PlayerStatsController : NetworkBehaviour
{
    public Canvas HealthCanvas;
    public float StartingHealth;
    public Renderer robeAndHat;
    [HideInInspector]
    public PlayerUI ui;

    private PlayerMovementController movementController;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public float Health;
    [SyncVar(hook = nameof(OnSpeedChanged))]
    public float Speed;
    [SyncVar(hook = nameof(OnFireRateChanged))]
    public float FireRate;
    [SyncVar(hook = nameof(OnProjectileSpeedChanged))]
    public float ProjectileSpeed;
    [SyncVar(hook = nameof(OnProjectileDamageChanged))]
    public float ProjectileDamage;
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color PlayerColor;
    [SyncVar(hook = nameof(OnAliveChanged))]
    public bool Alive;

    private float oldSpeed;
    private float oldFireRate;
    private float oldProjectileSpeed;
    private float oldProjectileDamage;
    private float oldHealth;

    public void Awake() {
        if (isLocalPlayer) {
            movementController = GetComponent<PlayerMovementController>();
            ui = GetComponent<PlayerUI>();
            StoreOldStats();
        }
    }

    [Command]
    public void CmdApplyItemStats(ItemDefinition item) {
        Health += item.HealthModifier;
        Speed += item.SpeedModifier;
        FireRate += item.FireRateModifier;
        ProjectileSpeed += item.ProjectileSpeedModifier;
        ProjectileDamage += item.DamageModifier;
    }

    private void OnColorChanged() {
        robeAndHat.material.color = PlayerColor;
    }

    private void OnHealthChanged() {
        if (hasAuthority) {
            if (Health < 0) {
                Health = 0;
                Alive = false;
            } else if (Health > 100) Health = 100;
        }

        if (isLocalPlayer) {
            ui.SetHealth(Health);
            float change = Health - oldHealth;
            if (Math.Abs(change) > 0.0001f) ui.FlashDamageTaken(-change);
            oldHealth = Health;
        }
    }

    private void OnAliveChanged() {
        if (isLocalPlayer) {
            if (!Alive) {
                ui.DisplayMessage("You Died.", 4f, 2f, () => SplitscreenManager.instance.SetCameraToSpectator(ui.ScreenNumber-1, movementController.localPlayer));
                movementController.localPlayer.StopVibration();
            }
        }
    }

    private void OnSpeedChanged() {
        if (isLocalPlayer) {
            float change = Speed - oldSpeed;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddSpeedText(change, transform.position);
                ui.SetPlayerSpeedStat(Speed);
            }
            oldSpeed = Speed;
        }
    }

    private void OnFireRateChanged() {
        if (isLocalPlayer) {
            float change = FireRate - oldFireRate;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddFireRateText(change, transform.position);
                ui.SetFireRateStat(FireRate);
            }
            oldFireRate = FireRate;
        }
    }

    private void OnProjectileSpeedChanged() {
        if (isLocalPlayer) {
            float change = ProjectileSpeed - oldProjectileSpeed;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddProjectileSpeedText(change, transform.position);
                ui.SetProjectileSpeedStat(ProjectileSpeed);
            }
            oldProjectileSpeed = ProjectileSpeed;
        }
    }

    private void OnProjectileDamageChanged() {
        if (isLocalPlayer) {
            float change = ProjectileDamage - oldProjectileDamage;
            if (Math.Abs(change) > 0.0001f) {
                ui.AddProjectileDamageText(change, transform.position);
                ui.SetProjectileDamageStat(ProjectileDamage);
            }
            oldProjectileDamage = ProjectileDamage;
        }
    }

    [Command]
    public void CmdDamagePlayer(float amt, Vector3 damagePos, GameObject owner) {
        if (Health <= 0) return;
        if (hasAuthority) {
            Health -= amt;
        }
        if (isLocalPlayer) {
            movementController.localPlayer.SetVibration(0, 1f, .1f);
            movementController.localPlayer.SetVibration(1, 1f, .1f);
            movementController.localPlayer.SetVibration(2, 1f, .1f);
            movementController.localPlayer.SetVibration(3, 1f, .1f);
            ui.AddDamageText(amt, damagePos);
        }
    }

    private void StoreOldStats() {
        oldSpeed = Speed;
        oldFireRate = FireRate;
        oldProjectileSpeed = ProjectileSpeed;
        oldProjectileDamage = ProjectileDamage;
        oldHealth = Health; 
        // Health is only used for compounded damage taken, not damage dealt or healing received.
    }
}
