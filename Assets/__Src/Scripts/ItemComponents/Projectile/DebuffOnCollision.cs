using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]

public class DebuffOnCollision : NetworkBehaviour
{
    public bool damageOwner;
    public string debuffName;
    public float debuffDuration;
    public float healthModifier;
    public float speedModifier;
    public float fireRateModifier;
    public float projectileSpeedModifier;
    public float damageModifier;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasAuthority) return;
        GameObject otherEntity = collision.gameObject;
        if (GetComponent<CollisionCheck>().ValidCollision(collision) && otherEntity && (damageOwner || otherEntity != GetComponent<Projectile>().OwnerGameObject))
        {
            GiveDebuff(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) return;
        GameObject otherEntity = other.gameObject;
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other) && otherEntity && (damageOwner || otherEntity != GetComponent<Projectile>().OwnerGameObject))
        {
            GiveDebuff(other.gameObject);
        }
    }

    private void GiveDebuff(GameObject target)
    {
        if (target.tag != "Player") return;
        PlayerDebuffHandler debuffHandler = target.GetComponent<PlayerDebuffHandler>();
        ProjectileDebuff debuff = new ProjectileDebuff();
        if (hasAuthority)
        {
            Debug.Log("Giving a Debuff! " + debuffName);
            debuff.Name = debuffName;
            debuff.EffectLength = debuffDuration;
            debuff.healthMod = healthModifier;
            debuff.projectileMod = projectileSpeedModifier;
            debuff.damageMod = damageModifier;
            debuff.speedMod = speedModifier;
            debuff.firerateMod = fireRateModifier;
            debuffHandler.GrantDebuff(debuff);
        }
    }
}
