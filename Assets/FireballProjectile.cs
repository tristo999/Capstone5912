using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public GameObject owner;
    public GameObject trail;
    public GameObject explosion;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    private void OnCollisionEnter(Collision collision) {
        if (entity.isOwner && collision.gameObject.tag == "Player") {
            PlayerHit playerHit = PlayerHit.Create();
            playerHit.HitEntity = collision.gameObject.GetComponent<PlayerMovementController>().entity;
            playerHit.Send();
        }
        trail.SetActive(false);
        explosion.SetActive(true);
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy() {
        yield return new WaitForSeconds(1f);
        if (entity.isAttached)
            BoltNetwork.Destroy(gameObject);
    }
}
