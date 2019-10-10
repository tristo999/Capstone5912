using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyCluster : NetworkBehaviour
{
    public List<GameObject> enemyList;
    public void Awake()
    {
        if (!hasAuthority) return;
        foreach (GameObject x in enemyList)
        {
            GameObject spawnOne = Instantiate(x, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.rotation);
            GameObject spawnTwo = Instantiate(x, new Vector3(transform.position.x -.8666f, transform.position.y, transform.position.z - .5f), transform.rotation);
            GameObject spawnThree = Instantiate(x, new Vector3(transform.position.x -.8666f, transform.position.y, transform.position.z + .5f), transform.rotation);
            NetworkServer.Spawn(spawnOne);
            NetworkServer.Spawn(spawnTwo);
            NetworkServer.Spawn(spawnThree);
        }
    }
}
