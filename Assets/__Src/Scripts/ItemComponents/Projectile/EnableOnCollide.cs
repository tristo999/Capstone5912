using Mirror;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class EnableOnCollide : NetworkBehaviour
{
    public List<GameObject> objectsToEnable = new List<GameObject>();

    private void OnCollisionEnter(Collision collision) {
        if (!hasAuthority) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (GameObject gameObject in objectsToEnable) {
                if (gameObject != null) gameObject.SetActive(true);
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!hasAuthority) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            foreach (GameObject gameObject in objectsToEnable) {
                if (gameObject != null) gameObject.SetActive(true);
            }
        }
    }
}
