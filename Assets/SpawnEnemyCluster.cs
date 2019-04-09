using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyCluster : Bolt.EntityEventListener<IEnemyClusterState>
{
    public List<GameObject> enemyList;
    private void Awake()
    {
        foreach (GameObject x in enemyList)
        {
            BoltNetwork.Instantiate(x, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.rotation);
            BoltNetwork.Instantiate(x, new Vector3(transform.position.x -.8666f, transform.position.y, transform.position.z - .5f), transform.rotation);
            BoltNetwork.Instantiate(x, new Vector3(transform.position.x -.8666f, transform.position.y, transform.position.z + .5f), transform.rotation);
        }
    }
}
