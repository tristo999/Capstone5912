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
                    if (bEntity.gameObject.tag == "Player" && (bEntity!= state.Owner || damageOwner) || bEntity.gameObject.gameObject.tag == "Enemy") {
                        Debug.Log(col.gameObject.name);
                        DealDamage(collision.gameObject);

                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy" || col.gameObject.layer == 15)
                    col.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Explosion!");
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (Collider col in Physics.OverlapSphere(transform.position, radius)) {
                BoltEntity bEntity = col.GetComponent<BoltEntity>();
                if (bEntity) {
                    if (bEntity.gameObject.tag == "Player" && (bEntity != state.Owner || damageOwner) || bEntity.gameObject.gameObject.tag == "Enemy") {
                        Debug.Log(col.gameObject.name);
                        DealDamage(col.gameObject);
                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy" || col.gameObject.layer == 15)
                        col.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }

    private void DealDamage(GameObject target) {
        if (target.GetComponent<BoltEntity>() == null) return;
        DamageEntity DamageEntity = DamageEntity.Create(target.GetComponent<BoltEntity>());
        DamageEntity.Damage = damage;
        if (entity.isOwner) {
            // Only include for the player that dealt this damage.
            DamageEntity.HitPosition = transform.position;
            DamageEntity.Owner = state.Owner;
        }
        DamageEntity.Send();
    }
}
