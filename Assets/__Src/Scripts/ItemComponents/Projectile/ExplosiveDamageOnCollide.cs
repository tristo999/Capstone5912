using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class ExplosiveDamageOnCollide : NetworkBehaviour
{
    public float damage;
    public float radius;
    public float explosiveKnockback;
    public bool damageOwner;
    [HideInInspector]
    public float damageModifier = 1f; // Assigned by WeaponLaunchProjectile (or later components).

    private void OnCollisionEnter(Collision collision) {
        if (!hasAuthority) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (Collider areaCol in Physics.OverlapSphere(transform.position, radius)) {
                float distance = Vector3.Distance(transform.position, areaCol.transform.position);
                if (areaCol.gameObject.tag == "Player" && (areaCol.gameObject != GetComponent<Projectile>().OwnerGameObject || damageOwner) || areaCol.gameObject.tag == "Enemy") {
                    // If this is the object you hit directly
                    if (collision.gameObject == areaCol.gameObject) {
                        // Deal full, direct damage.
                        DealDamage(areaCol.gameObject, damage);
                    } else {
                        // Deal damage on radius dropoff.
                        float dropoffDamage = CalculateDamageWithDropoff(distance);
                        DealDamage(areaCol.gameObject, dropoffDamage);
                    }
                }
                if (areaCol.gameObject.tag == "Player" || areaCol.gameObject.tag == "Enemy" || areaCol.gameObject.layer == 15)
                    areaCol.GetComponent<Rigidbody>().AddForce(GetKnockback((areaCol.transform.position - transform.position).normalized, distance));
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!hasAuthority) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (Collider areaCol in Physics.OverlapSphere(transform.position, radius)) {
                float distance = Vector3.Distance(transform.position, areaCol.transform.position);
                if (areaCol.gameObject.tag == "Player" && (areaCol.gameObject != GetComponent<Projectile>().OwnerGameObject || damageOwner) || areaCol.gameObject.tag == "Enemy") {
                    // If this is the object you hit directly
                    if (other.gameObject == areaCol.gameObject) {
                        // Deal full, direct damage.
                        DealDamage(areaCol.gameObject, damage);
                    } else {
                        // Deal damage on radius dropoff.
                        float dropoffDamage = CalculateDamageWithDropoff(distance);
                        DealDamage(areaCol.gameObject, dropoffDamage);
                    }
                }
                if (areaCol.gameObject.tag == "Player" || areaCol.gameObject.tag == "Enemy" || areaCol.gameObject.layer == 15)
                    areaCol.GetComponent<Rigidbody>().AddForce(GetKnockback((areaCol.transform.position - transform.position).normalized, distance));
            }
        }
    }

    private float CalculateExplosiveDropoff(float distance) {
        // Using a bit of a fudge factor here because the center of the hit object is likely to be further than the closest hit point.
        float fudgeMinDistance = 1; // Units of forgiveness for max splash damage;
        float fudgeMultiplier = 1.25f; // % to spread out distance for min damage;
        float minPercentRemaining = 0.25f;

        float percentRemaining = (1 - (distance - fudgeMinDistance) / (radius * fudgeMultiplier));
        if (percentRemaining < 0) percentRemaining = 0;
        if (percentRemaining > 1) percentRemaining = 1;
        return minPercentRemaining + (1 - minPercentRemaining) * percentRemaining;
    }

    private float CalculateDamageWithDropoff(float distance) {
        return damage * CalculateExplosiveDropoff(distance);
    }

    private Vector3 GetKnockback(Vector3 direction, float distance) {
        direction = (direction.normalized + new Vector3(0, 1.2f, 0)).normalized;
        return direction * explosiveKnockback * CalculateExplosiveDropoff(distance);
    }

    private void DealDamage(GameObject target, float damageCalculated) {
        PlayerStatsController playerStatsController = target.GetComponent<PlayerStatsController>();
        BasicEnemyAI enemyAiController = target.GetComponent<BasicEnemyAI>();
        if (playerStatsController) {
            playerStatsController.CmdDamagePlayer(damageCalculated * damageModifier, transform.position, GetComponent<Projectile>().OwnerGameObject);
        } else if (enemyAiController) {
            enemyAiController.CmdDamageEnemy(damageCalculated * damageModifier);
        }
    }
}
