using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck))]
public class AttachToWorldOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public List<GameObject> objectsToSurvive = new List<GameObject>();
    public List<bool> retainPosition = new List<bool>();
    public float DestroyAfterDelay;

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            for (int i = 0; i < objectsToSurvive.Count; i++) {
                GameObject obj = objectsToSurvive[i];
                bool retainPos = retainPosition[i];
                AttachToWorld(obj, retainPos);
            }
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(other)) {
            for (int i = 0; i < objectsToSurvive.Count; i++) {
                GameObject obj = objectsToSurvive[i];
                bool retainPos = retainPosition[i];
                AttachToWorld(obj, retainPos);
            }
        }
    }

    private void AttachToWorld(GameObject obj, bool retainPos) {
        if (obj != null) {
            obj.transform.SetParent(null, retainPos);
            if (!retainPos) {
                obj.transform.position = new Vector3(-99999, -99999, -99999);
            }
            obj.GetComponent<DestroyOnDelay>().DestroyAfter(DestroyAfterDelay);
        }
    }
}
