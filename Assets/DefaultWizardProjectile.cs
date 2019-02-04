using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWizardProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public GameObject owner;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isOwner) return;
        if (collision.gameObject.tag == "Player") {
            PlayerHit playerHit = PlayerHit.Create();
            playerHit.HitEntity = collision.gameObject.GetComponent<PlayerController>().entity;
            playerHit.Send();
        }
        BoltNetwork.Destroy(gameObject);
    }
}
