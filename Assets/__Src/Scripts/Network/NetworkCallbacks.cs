using Bolt;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "WizardFightGame")]
public class GameNetworkCallbacks : Bolt.GlobalEventListener
{
    private int connections = WizardFightPlayerRegistry.NumberConnections;
    private int readyConnections = 0;

    public override void SceneLoadLocalDone(string scene) {
        BoltNetwork.Instantiate(BoltPrefabs.ItemManager);
        GameMaster.Instantiate();
        GenerationManager.Instantiate();
        Physics.autoSimulation = false;
        GenerationManager.instance.GenerateStemmingMazeGraph();
        GameMaster.instance.SetRoomLayers(GenerationManager.instance.rooms);
        readyConnections++;
        TryStartMatch();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection) {
        // Track established connections, then once all are established or a timeout happens, spawn players and begin game.

        readyConnections++;
        Debug.LogFormat("Remote connection complete, expecting {0} total connections, have {1}", connections, readyConnections);
        TryStartMatch();
    }

    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token) {
        BoltNetwork.Refuse(endpoint);
    }

    private void TryStartMatch() {
        if (readyConnections >= connections) {
            Physics.autoSimulation = true;
            foreach (WizardFightPlayerObject player in WizardFightPlayerRegistry.Players) {
                SpawnPlayer spawnPlayer;
                if (player.connection) {
                    spawnPlayer = SpawnPlayer.Create(player.connection);
                } else {
                    spawnPlayer = SpawnPlayer.Create(GlobalTargets.OnlySelf);
                }
                spawnPlayer.WaitForRooms = GenerationManager.instance.rooms.Count;
                spawnPlayer.PlayerId = player.PlayerId;
                spawnPlayer.Name = player.PlayerName;
                spawnPlayer.Color = player.PlayerColor;
                Vector3 pos = GenerationManager.instance.SpawnPositions(1)[0];
                //Vector3 pos = GenerationManager.instance.rooms[Random.Range(0, GenerationManager.instance.rooms.Count)].transform.position + new Vector3(GenerationManager.instance.roomSize / 2, 2, GenerationManager.instance.roomSize / 2);
                spawnPlayer.Position = pos;
                spawnPlayer.Send();
            }
        }
        GameStart evnt = GameStart.Create();
        evnt.Send();
    }

    private void Update() {

    }
}
