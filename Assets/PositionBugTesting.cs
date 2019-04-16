using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionBugTesting : Bolt.GlobalEventListener
{

    public List<BoltEntity> enemies = new List<BoltEntity>();

    public override void SceneLoadLocalDone(string scene) {
        base.SceneLoadLocalDone(scene);
        

        if (BoltNetwork.IsServer) {
            BoltNetwork.Instantiate(enemies[Random.Range(0, enemies.Count)], new Vector3(Random.Range(-10f,10f), .5f, Random.Range(-5f,5f)), Quaternion.identity);
        }
    }
}
