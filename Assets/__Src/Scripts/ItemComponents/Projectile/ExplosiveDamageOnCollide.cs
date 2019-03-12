using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class ExplosiveDamageOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float damage;
    public float radius;
    public float explosiveKnockback;
    public bool damageOwner;

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (Collider col in Physics.OverlapSphere(transform.position, radius)) {
                BoltEntity bEntity = col.GetComponent<BoltEntity>();
                if (bEntity) {
                    if (col.tag == "Player" && (bEntity!= state.Owner || damageOwner)) {
                        DamageEntity DamageEntity = DamageEntity.Create(collision.gameObject.GetComponent<BoltEntity>());
                        DamageEntity.Damage = damage;
                        DamageEntity.Send();
                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    col.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (Collider col in Physics.OverlapSphere(transform.position, radius)) {
                BoltEntity bEntity = col.GetComponent<BoltEntity>();
                if (bEntity) {
                    if (col.tag == "Player" && (bEntity != state.Owner || damageOwner)) {
                        DamageEntity DamageEntity = DamageEntity.Create(other.gameObject.GetComponent<BoltEntity>());
                        DamageEntity.Damage = damage;
                        DamageEntity.Send();
                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    col.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }
}
