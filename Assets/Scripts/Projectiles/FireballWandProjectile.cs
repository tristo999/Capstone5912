using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballWandProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public GameObject owner;
    public GameObject trail;
    public GameObject explosion;

    public float damage;
    public float explosionRadius;
    public float explosionForce;

    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        if (transform.position.y <= 0) {
            Explode();
            StartCoroutine(DelayDestroy());
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Explode();
        StartCoroutine(DelayDestroy());
    }

    private void Explode() {
        trail.SetActive(false);
        explosion.SetActive(true);
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;

        Collider[] hit = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider c in hit) {
            Rigidbody rigid = c.GetComponent<Rigidbody>();
            if (rigid != null) {
                rigid.AddExplosionForce(explosionForce, transform.position - new Vector3(0, -1f), explosionRadius);
            }

            if (c.gameObject.GetComponent<BoltEntity>() && entity.isControllerOrOwner) {
                DamageEntity pHit = DamageEntity.Create(c.GetComponent<BoltEntity>());
                pHit.Damage = damage;
                pHit.Send();
            }
        }
    }

    IEnumerator DelayDestroy() {
        yield return new WaitForSeconds(1f);
        if (entity.isAttached)
            BoltNetwork.Destroy(gameObject);
    }
}
