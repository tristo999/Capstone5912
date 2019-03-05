using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public BoltEntity entity;
    [Range(0.0f,1.0f)]
    public float ChanceSpawn;

    private void Start() {
        if (BoltNetwork.IsServer) {
            if (Random.Range(0.0f,1.0f) < ChanceSpawn) {
                BoltNetwork.Instantiate(entity, transform.position, Quaternion.identity);
            }
        }
    }
}
