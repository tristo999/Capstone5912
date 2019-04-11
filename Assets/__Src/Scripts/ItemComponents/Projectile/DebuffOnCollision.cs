using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]

public class DebuffOnCollision : Bolt.EntityBehaviour<IProjectileState>
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
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = collision.gameObject.GetComponent<BoltEntity>();
        if (GetComponent<CollisionCheck>().ValidCollision(collision) && otherEntity && (damageOwner || otherEntity != state.Owner))
        {
            GiveDebuff(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!entity.isAttached || !entity.isOwner) return;
        BoltEntity otherEntity = other.GetComponent<BoltEntity>();
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other) && otherEntity && (damageOwner || otherEntity != state.Owner))
        {
            GiveDebuff(other.gameObject);
        }
    }

    private void GiveDebuff(GameObject target)
    {
        if (target.tag != "Player") return;
        PlayerDebuffHandler debuffHandler = target.GetComponent<PlayerDebuffHandler>();
        ProjectileDebuff debuff = new ProjectileDebuff();
        if (entity.isOwner)
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
