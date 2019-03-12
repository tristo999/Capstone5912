using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class EnableOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public List<GameObject> objectsToEnable = new List<GameObject>();

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (GameObject gameObject in objectsToEnable) {
                gameObject.SetActive(true);
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (GameObject gameObject in objectsToEnable) {
                gameObject.SetActive(true);
            }
        }
    }
}
