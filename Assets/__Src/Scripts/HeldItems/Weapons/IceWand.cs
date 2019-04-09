using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWand : Weapon
{
    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProj;

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
            launchProj.Launch(-30);
            launchProj.Launch();
            launchProj.Launch(30);
            cooldown.ResetCooldown();
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
