using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponLaunchProjectile))]
[RequireComponent(typeof(WeaponUses))]
public class ThrowingKnife : Weapon { 
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;

    private void Awake() {
        launchProj = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireDown() {
        if (!Owner.hasAuthority) return;
        launchProj.Launch(0, new Vector3(0, 0, -70));
        uses.Use();
    }

    public override void FireHold() { }

    public override void FireRelease() { }

    public override void OnEquip() { }

    public override void OnDequip() { }
}
