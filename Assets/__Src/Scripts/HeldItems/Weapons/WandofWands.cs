using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandofWands : Weapon
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

    }

    public override void OnDequip() { }
}
