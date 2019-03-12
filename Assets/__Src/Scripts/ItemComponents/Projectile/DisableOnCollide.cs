using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class DisableOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public List<GameObject> objectsToDisable = new List<GameObject>();

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (GameObject gameObject in objectsToDisable) {
                gameObject.SetActive(false);
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (GameObject gameObject in objectsToDisable) {
                gameObject.SetActive(false);
            }
        }
    }
}
