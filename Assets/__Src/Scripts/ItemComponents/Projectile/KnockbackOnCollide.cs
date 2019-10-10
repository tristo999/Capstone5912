using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class KnockbackOnCollide : NetworkBehaviour
{
    public float knockback;
    private Rigidbody rigid;

    public void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (!isServer) return;
        CollisionCheck checker = GetComponent<CollisionCheck>();
        if (checker && !checker.ValidCollision(collision)) return;
        PlayerMovementController pMove = collision.gameObject.GetComponent<PlayerMovementController>();
        if (pMove) {
            pMove.CmdApplyKnockback(GetKnockback());
        } else {
            Rigidbody otherRigid = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigid) {
                otherRigid.AddForce(GetKnockback());
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isServer) return;
        CollisionCheck checker = GetComponent<CollisionCheck>();
        if (checker && !checker.ValidCollision(other)) return;
        PlayerMovementController pMove = other.gameObject.GetComponent<PlayerMovementController>();
        if (pMove) {
            pMove.CmdApplyKnockback(GetKnockback());
        } else {
            Rigidbody otherRigid = other.gameObject.GetComponent<Rigidbody>();
            if (otherRigid) {
                otherRigid.AddForce(GetKnockback());
            }
        }
    }

    private Vector3 GetKnockback() {
        return (rigid.velocity.normalized + new Vector3(0, 1f, 0)).normalized * knockback;
    }
}
