using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionCheck))]
public class DropOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public float Delay;
    public GameObject DropItem;

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            StartCoroutine(DelayedDrop(Delay));
        } 
    }

    private void OnTriggerEnter(Collider other) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (other.tag != "Room" && GetComponent<CollisionCheck>().ValidCollision(other)) {
            StartCoroutine(DelayedDrop(Delay));
        }
    }

    IEnumerator DelayedDrop(float time) {
        yield return new WaitForSeconds(time);
        if (entity.isAttached) {
            Drop();
        }
    }

    private void Drop() {
        GameObject dropItem = ItemManager.Instance.Spawn(transform.position, Vector3.zero, DropItem);
        dropItem.transform.rotation = transform.rotation;
        dropItem.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
    }
}
