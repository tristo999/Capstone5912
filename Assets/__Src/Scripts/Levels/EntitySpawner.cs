using Mirror;
using UnityEngine;

public class EntitySpawner : NetworkBehaviour
{
    public GameObject entity;
    [Range(0.0f,1.0f)]
    public float ChanceSpawn;

    private void Start() {
        if (isServer) {
            if (Random.Range(0.0f,1.0f) < ChanceSpawn) {
                GameObject spawned = Instantiate(entity, transform.position, Quaternion.identity);
                NetworkServer.Spawn(spawned);
            }
        }
    }
}
