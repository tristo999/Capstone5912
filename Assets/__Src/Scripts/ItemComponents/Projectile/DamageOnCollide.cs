using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class DamageOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float damage;
    public bool damageOwner;

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = collision.gameObject.GetComponent<BoltEntity>();
        if (GetComponent<CollisionCheck>().ValidCollision(collision) && otherEntity && (damageOwner || otherEntity != state.Owner)) {
            DamageEntity DamageEntity = DamageEntity.Create(collision.gameObject.GetComponent<BoltEntity>());
            DamageEntity.Damage = damage;
            DamageEntity.Send();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = other.GetComponent<BoltEntity>();
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other) && otherEntity && (damageOwner || otherEntity != state.Owner)) {
            DamageEntity DamageEntity = DamageEntity.Create(other.gameObject.GetComponent<BoltEntity>());
            DamageEntity.Damage = damage;
            DamageEntity.Send();
        }
    }
}
