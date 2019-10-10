using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDestroy : NetworkBehaviour
{
    [Tooltip("Probability of object existing")]
    [Range(0, 100)]
    public int probability;

    public void Awake()
    {
        if (isServer) {
            if (Random.Range(0, 100) > probability) {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
