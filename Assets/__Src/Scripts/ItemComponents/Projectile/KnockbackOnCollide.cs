using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class KnockbackOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float knockback;
    private Rigidbody rigid;

    public override void Attached() {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        CollisionCheck checker = GetComponent<CollisionCheck>();
        if (checker && !checker.ValidCollision(collision)) return;

        if (collision.gameObject.GetComponent<BoltEntity>() && collision.gameObject.GetComponent<BoltEntity>().isAttached) {
            KnockbackEntity KnockbackEntity = KnockbackEntity.Create(collision.gameObject.GetComponent<BoltEntity>());
            KnockbackEntity.Force = GetKnockback();
            KnockbackEntity.Send();
        } else {
            Rigidbody otherRigid = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigid) {
                otherRigid.AddForce(GetKnockback());
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        CollisionCheck checker = GetComponent<CollisionCheck>();
        if (checker && !checker.ValidCollision(other)) return;
        if (other.gameObject.GetComponent<BoltEntity>() && other.gameObject.GetComponent<BoltEntity>().isAttached) {
            KnockbackEntity KnockbackEntity = KnockbackEntity.Create(other.gameObject.GetComponent<BoltEntity>());
            KnockbackEntity.Force = GetKnockback();
            KnockbackEntity.Send();
        } else {
            Rigidbody otherRigid = other.GetComponent<Rigidbody>();
            if (otherRigid) {
                otherRigid.AddForce(GetKnockback());
            }
        }
    }

    private Vector3 GetKnockback() {
        return (rigid.velocity.normalized + new Vector3(0, 0.75f, 0)).normalized * knockback;
    }
}
