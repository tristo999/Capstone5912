using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWandProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public GameObject owner;
    public float damage = 1f;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (collision.gameObject.tag == "Player") {
            PlayerHit playerHit = PlayerHit.Create(collision.gameObject.GetComponent<BoltEntity>());
            playerHit.Damage = damage;
            playerHit.Send();
        }
        BoltNetwork.Destroy(gameObject);
    }
}
