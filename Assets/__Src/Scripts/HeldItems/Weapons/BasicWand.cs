using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponCooldown))]
[RequireComponent(typeof(WeaponLaunchProjectile))]
public class BasicWand : Weapon
{
    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;

    private void Awake() {
        cooldown = GetComponent<WeaponCooldown>();
        launchProj = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireDown() {
        
    }

    public override void FireHold() {
        if (!Owner.entity.isOwner) return;
        if (cooldown.Ready) {
            launchProj.Launch();
            uses.Use();
            cooldown.ResetCooldown();
        } 
    }

    public override void FireRelease() {

    }

    public override void OnEquip() {

    }
}
