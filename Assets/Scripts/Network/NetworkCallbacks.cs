using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "WizardFightGame")]
public class NetworkCallbacks : Bolt.GlobalEventListener
{

    private void Awake() {
        WizardFightPlayerRegistry.CreateServerPlayer();
    }

    public override void Connected(BoltConnection connection) {
        WizardFightPlayerRegistry.CreateClientPlayer(connection);
    }

    /*public override void SceneLoadLocalDone(string map) {
        // randomize a position
        var spawnPosition = new Vector3(Random.Range(-4, 4), 4, Random.Range(-4, 4));

        // instantiate cube
        var spawned = BoltNetwork.Instantiate(BoltPrefabs.Player, spawnPosition, Quaternion.identity);
        PlayerJoined pj = PlayerJoined.Create();
        pj.Player = spawned;
        pj.Send();
        GameObject.FindGameObjectWithTag("CameraTarget").GetComponent<NetworkTargetting>().AddAllPlayers();
    }*/
}
