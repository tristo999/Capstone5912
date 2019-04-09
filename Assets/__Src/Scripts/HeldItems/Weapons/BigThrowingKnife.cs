using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponLaunchProjectile))]
[RequireComponent(typeof(WeaponUses))]
public class BigThrowingKnife : Weapon { 
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;

    private void Awake() {
        launchProj = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireDown() {
        if (!Owner.entity.isOwner) return;
        launchProj.Launch(0, new Vector3(0, 0, -200));
        uses.Use();
    }

    public override void FireHold() { }

    public override void FireRelease() { }

    public override void OnEquip() {
        Owner.GetComponent<PlayerStatsController>().state.Speed -= 0.3f;
    }

    public override void OnDequip() {
        Owner.GetComponent<PlayerStatsController>().state.Speed += 0.3f;
    }
}
