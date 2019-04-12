using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoWoWProj : Bolt.EntityBehaviour<IProjectileState>
{
    public GameObject Projectile;
    public float force;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    internal void Shoot(Vector3 dir) {
        BoltEntity proj = BoltNetwork.Instantiate(Projectile, transform.position, transform.rotation);
        proj.GetComponent<Rigidbody>().velocity = dir * force * state.Owner.GetState<IPlayerState>().ProjectileSpeed;
        proj.GetState<IProjectileState>().Owner = state.Owner;

        DamageOnCollide damageOnCollide = proj.GetComponent<DamageOnCollide>();
        if (damageOnCollide) {
            damageOnCollide.damageModifier = state.Owner.GetState<IPlayerState>().ProjectileDamage;
        }
    }
}
