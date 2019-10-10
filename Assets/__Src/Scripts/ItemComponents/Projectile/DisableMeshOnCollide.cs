using Mirror;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CollisionCheck))]
public class DisableMeshOnCollide : NetworkBehaviour
{
    public List<MeshRenderer> meshes = new List<MeshRenderer>();

    private void OnCollisionEnter(Collision collision) {
        if (!hasAuthority) return;
        if (GetComponent<CollisionCheck>().ValidCollision(collision)) {
            foreach (MeshRenderer mesh in meshes) {
                mesh.enabled = false;
            }
        } 
    }
}
