using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class DestroyOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float Delay;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Boom");
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            StartCoroutine(DelayedDestroy(Delay));
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other)) {
            StartCoroutine(DelayedDestroy(Delay));
        }
    }

    IEnumerator DelayedDestroy(float time) {
        yield return new WaitForSeconds(time);
        if (entity.isAttached)
            BoltNetwork.Destroy(entity);
    }
}
