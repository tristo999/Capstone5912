using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoWoWProj : Projectile
{
    public GameObject Projectile;
    public float force;

    internal void Shoot(Vector3 dir) {
        GameObject proj = Instantiate(Projectile, transform.position, Quaternion.LookRotation(dir));
        NetworkServer.Spawn(proj);
        proj.GetComponent<Rigidbody>().velocity = dir * force * OwnerGameObject.GetComponent<PlayerStatsController>().ProjectileSpeed;

        DamageOnCollide damageOnCollide = proj.GetComponent<DamageOnCollide>();
        if (damageOnCollide) {
            damageOnCollide.damageModifier = OwnerGameObject.GetComponent<PlayerStatsController>().ProjectileDamage;
        }
    }
}
