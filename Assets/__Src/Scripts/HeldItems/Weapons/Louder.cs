using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponCooldown))]
[RequireComponent(typeof(WeaponLaunchProjectile))]
[RequireComponent(typeof(WeaponUses))]

public class Louder : Weapon
{
    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;


    private void Awake()
    {
        cooldown = GetComponent<WeaponCooldown>();
        launchProj = GetComponent<WeaponLaunchProjectile>();
    }
    public override void FireDown()
    {
        if (!Owner.entity.isOwner) return;
        if (cooldown.Ready)
        {
            launchProj.Launch();
            uses.Use();
            cooldown.ResetCooldown();
            launchProj.LaunchForce += 15; 
        }
    }

    public override void FireHold()
    {

    }

    public override void FireRelease()
    {

    }

    public override void OnEquip()
    {

    }

    public override void OnDequip() { }
}
