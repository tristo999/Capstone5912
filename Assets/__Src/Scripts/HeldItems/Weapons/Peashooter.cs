using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponCooldown))]
[RequireComponent(typeof(WeaponLaunchProjectile))]
[RequireComponent(typeof(WeaponUses))]
public class Peashooter : Weapon {
    private static readonly float speedMod = 0.15f;

    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;

    public override void FireDown() {
        cooldown = GetComponent<WeaponCooldown>();
        launchProj = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireHold() {
        if (!Owner.hasAuthority) return;
        if (cooldown.Ready) {
            launchProj.Launch();
            uses.Use();
            cooldown.ResetCooldown();
        }
    }

    public override void FireRelease() {
        
    }

    public override void OnEquip() {
        Owner.GetComponent<PlayerStatsController>().Speed += speedMod;
    }

    public override void OnDequip() {
        Owner.GetComponent<PlayerStatsController>().Speed -= speedMod;
    }
}
