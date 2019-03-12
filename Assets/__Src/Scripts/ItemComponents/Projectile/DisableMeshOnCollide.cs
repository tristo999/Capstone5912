using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class DisableMeshOnCollide : Bolt.EntityBehaviour<IProjectileState>
{
    public List<MeshRenderer> meshes = new List<MeshRenderer>();

    private void OnCollisionEnter(Collision collision) {
        if (!entity.isAttached || !entity.isOwner) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (MeshRenderer mesh in meshes) {
                mesh.enabled = false;
            }
        } 
    }
}
