using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandofWandsofWands : Weapon
{
    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;
    private List<GameObject> projectiles = new List<GameObject>();
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
            projectiles.RemoveAll(e => e == null);
            foreach (GameObject e in projectiles) {
                if (e != null)
                    e.GetComponent<WoWoWProj>().Shoot(Owner.transform.forward);
            }
            projectiles.Add(launchProj.Launch());
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
