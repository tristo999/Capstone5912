using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class DisableColliderOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public List<Collider> colliders = new List<Collider>();

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (Collider collider in colliders) {
                collider.enabled = false;
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (Collider collider in colliders) {
                collider.enabled = false;
            }
        }
    }
}
