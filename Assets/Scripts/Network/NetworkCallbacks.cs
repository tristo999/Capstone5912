using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map) {
        // randomize a position
        var spawnPosition = new Vector3(Random.Range(-4, 4), 4, Random.Range(-4, 4));

        // instantiate cube
        var spawned = BoltNetwork.Instantiate(BoltPrefabs.Player, spawnPosition, Quaternion.identity);
        PlayerJoined pj = PlayerJoined.Create();
        pj.Player = spawned;
        pj.Send();
        GameObject.FindGameObjectWithTag("CameraTarget").GetComponent<NetworkTargetting>().AddAllPlayers();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        
    }
}
