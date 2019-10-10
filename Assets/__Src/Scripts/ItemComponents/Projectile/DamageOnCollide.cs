using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class DamageOnCollide : NetworkBehaviour
{
    public float damage;
    public bool damageOwner;
    [HideInInspector]
    public float damageModifier = 1f; // Assigned by WeaponLaunchProjectile (or later components).

    private void OnCollisionEnter(Collision collision) {
        if (!hasAuthority) return;
        GameObject otherEntity = collision.gameObject;
        if (GetComponent<CollisionCheck>().ValidCollision(collision) && otherEntity && (damageOwner || otherEntity != GetComponent<Projectile>().OwnerGameObject)) {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!hasAuthority) return;
        GameObject otherEntity = other.gameObject;
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other) && otherEntity && (damageOwner || otherEntity != GetComponent<Projectile>().OwnerGameObject)) {
            DealDamage(other.gameObject);
        }
    }

    private void DealDamage(GameObject target) {
        float dealt = damage * damageModifier;
        target.GetComponent<PlayerStatsController>().CmdDamagePlayer(dealt, transform.position, GetComponent<Projectile>().OwnerGameObject);
    }
}
