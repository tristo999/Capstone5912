using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class DamageOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float damage;
    public bool damageOwner;
    [HideInInspector]
    public float damageModifier = 1f; // Assigned by WeaponLaunchProjectile (or later components).

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = collision.gameObject.GetComponent<BoltEntity>();
        if (GetComponent<CollisionCheck>().ValidCollision(collision) && otherEntity && (damageOwner || otherEntity != state.Owner)) {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = other.GetComponent<BoltEntity>();
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other) && otherEntity && (damageOwner || otherEntity != state.Owner)) {
            DealDamage(other.gameObject);
        }
    }

    private void DealDamage(GameObject target) {
        DamageEntity DamageEntity = DamageEntity.Create(target.GetComponent<BoltEntity>());
        DamageEntity.Damage = damage * damageModifier;
        if (entity.isOwner) {
            // Only include for the player that dealt this damage.
            DamageEntity.HitPosition = transform.position;
            DamageEntity.Owner = state.Owner;
        }
        DamageEntity.Send();
    }
}
