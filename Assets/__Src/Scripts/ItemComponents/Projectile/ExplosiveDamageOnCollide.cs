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
            foreach (Collider areaCol in Physics.OverlapSphere(transform.position, radius)) {
                BoltEntity bEntity = areaCol.GetComponent<BoltEntity>();
                if (bEntity) {
                    if (bEntity.gameObject.tag == "Player" && (bEntity!= state.Owner || damageOwner) || bEntity.gameObject.gameObject.tag == "Enemy") {
                        // If this is the object you hit directly
                        if (collision.gameObject == areaCol.gameObject) {
                            // Deal full, direct damage.
                            DealDamage(areaCol.gameObject, damage);
                        } else {
                            // Deal damage on radius dropoff.
                            float dropoffDamage = CalculateDamageWithDropoff(Vector3.Distance(transform.position, areaCol.transform.position));
                            DealDamage(areaCol.gameObject, dropoffDamage);
                        }
                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    if (areaCol.gameObject.tag == "Player" || areaCol.gameObject.tag == "Enemy" || areaCol.gameObject.layer == 15)
                        areaCol.GetComponent<Rigidbody>().AddForce((areaCol.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (Collider areaCol in Physics.OverlapSphere(transform.position, radius)) {
                BoltEntity bEntity = areaCol.GetComponent<BoltEntity>();
                if (bEntity) {
                    if (bEntity.gameObject.tag == "Player" && (bEntity != state.Owner || damageOwner) || bEntity.gameObject.gameObject.tag == "Enemy") {
                        // If this is the object you hit directly
                        if (other.gameObject == areaCol.gameObject) {
                            // Deal full, direct damage.
                            DealDamage(areaCol.gameObject, damage);
                        } else {
                            // Deal damage on radius dropoff.
                            float dropoffDamage = CalculateDamageWithDropoff(Vector3.Distance(transform.position, areaCol.transform.position));
                            DealDamage(areaCol.gameObject, dropoffDamage);
                        }
                    }
                    KnockbackEntity knockback = KnockbackEntity.Create(bEntity);
                    knockback.Force = (bEntity.transform.position - transform.position).normalized * explosiveKnockback;
                    knockback.Send();
                } else {
                    if (areaCol.gameObject.tag == "Player" || areaCol.gameObject.tag == "Enemy" || areaCol.gameObject.layer == 15)
                        areaCol.GetComponent<Rigidbody>().AddForce((areaCol.transform.position - transform.position).normalized * explosiveKnockback);
                }
            }
        }
    }

    private float CalculateDamageWithDropoff(float distance) {
        // Using a bit of a fudge factor here because the center of the hit object is likely to be further than the closest hit point.
        float fudgeMinDistance = 1; // Units of forgiveness for max splash damage;
        float fudgeMultiplier = 1.25f; // % to spread out distance for min damage;
        float minDamagePercent = 0.25f;

        float damagePercent = (1 - (distance - fudgeMinDistance) / (radius * fudgeMultiplier)); 
        if (damagePercent < 0) {
            damagePercent = 0;
        }
        return damage * (minDamagePercent + (1 - minDamagePercent) * damagePercent);
    }

    private void DealDamage(GameObject target, float damage) {
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
