using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "WizardFightGame")]
public class GameNetworkCallbacks : Bolt.GlobalEventListener
{
    private int connections = WizardFightPlayerRegistry.NumberConnections;
    private int readyConnections = 0;

    public override void SceneLoadLocalDone(string scene) {
        BoltNetwork.Instantiate(BoltPrefabs.ItemManager);
        // Begin dungeon generation. 
        readyConnections++;
        TryStartMatch();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection) {
        // Track established connections, then once all are established or a timeout happens, spawn players and begin game.

        readyConnections++;
        Debug.LogFormat("Remote connection complete, expecting {0} total connections, have {1}", connections, readyConnections);
        TryStartMatch();
    }

    private void TryStartMatch() {
        if (readyConnections >= connections) {
            foreach (WizardFightPlayerObject player in WizardFightPlayerRegistry.Players) {
                BoltEntity playerEntity = player.Spawn();
                playerEntity.transform.position = new Vector3(Random.Range(-2f, 2f), 3, Random.Range(-2f, 2f));
                if (player.connection != null)
                    playerEntity.AssignControl(player.connection);
                else {
                    playerEntity.TakeControl();
                }
            }
        }
    }

    private void Update() {

    }
}
