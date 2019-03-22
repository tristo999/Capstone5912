using Bolt;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        GenerationManager.instance.DoGeneration(WizardFightPlayerRegistry.Players.Count());
        GameMaster.instance.SetRoomLayers(GenerationManager.instance.dungeonGraph.Vertices);
        Debug.Log("After generation there exist " + GameMaster.instance.roomsAndClutter.Count + " entities");
        WaitForMap waitEvnt = WaitForMap.Create();
        waitEvnt.NumberEntities = BoltNetwork.Entities.Count();
        waitEvnt.Send();
    }

    public override void OnEvent(ReadySpawn evnt) {
        readyConnections++;
        if (readyConnections >= BoltNetwork.Connections.Count() + 1) {
            StartMatch();
        }
    }

    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token) {
        BoltNetwork.Refuse(endpoint);
    }

    private void StartMatch() {
        Physics.autoSimulation = true;
        foreach (WizardFightPlayerObject player in WizardFightPlayerRegistry.Players) {
            SpawnPlayer spawnPlayer;
            if (player.connection) {
                spawnPlayer = SpawnPlayer.Create(player.connection);
            } else {
                spawnPlayer = SpawnPlayer.Create(GlobalTargets.OnlySelf);
            }
            spawnPlayer.PlayerId = player.PlayerId;
            spawnPlayer.Name = player.PlayerName;
            spawnPlayer.Color = player.PlayerColor;
            Vector3 pos = GenerationManager.instance.GetSpawnPos(player.PlayerId);
            //Vector3 pos = GenerationManager.instance.rooms[Random.Range(0, GenerationManager.instance.rooms.Count)].transform.position + new Vector3(GenerationManager.instance.roomSize / 2, 2, GenerationManager.instance.roomSize / 2);
            spawnPlayer.Position = pos;
            spawnPlayer.Send();
        }
        GameStart evnt = GameStart.Create();
        evnt.Send();
    }

    public override void EntityAttached(BoltEntity entity) {
        if (entity.gameObject.layer == 14 || entity.gameObject.layer == 15 || entity.gameObject.layer == 16) {
            GameMaster.instance.roomsAndClutter.Add(entity);
        }
        if (entity.tag == "Player") {
            GameMaster.instance.SpawnedPlayers++;
        }
    }

    public override void OnEvent(FreezeDistant evnt) {
        GameMaster.instance.FreezeDistantEntities();
    }

    public override void OnEvent(DisconnectPlayer evnt) {
        evnt.RaisedBy.Disconnect();
    }

    private void Update() {

    }
}
