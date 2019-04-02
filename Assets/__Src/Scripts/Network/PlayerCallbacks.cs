using Bolt;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private int waitFor = 0;
    private bool waiting = false;

    public override void SceneLoadLocalDone(string scene) {
        if (SplitscreenManager.instance) {
            SplitscreenManager.instance.Reset();
        } else {
            SplitscreenManager.Instantiate();
        }
    }

    public override void EntityAttached(BoltEntity entity) {
        if (!BoltNetwork.IsServer && entity.tag == "Room") {
            DestroyWithTag(entity.gameObject, "CarpetSpawner");
            DestroyWithTag(entity.gameObject, "CabinetClutter");
            DestroyWithTag(entity.gameObject, "EnemySpawn");
            DestroyWithTag(entity.gameObject, "TableClutter");
            DestroyWithTag(entity.gameObject, "ChestSpawn");
            DestroyWithTag(entity.gameObject, "GroundClutter");
            DestroyWithTag(entity.gameObject, "ChildClutter");
        }


        if (waiting && BoltNetwork.Entities.Count() >= waitFor) {
            ReadySpawn evnt = ReadySpawn.Create(Bolt.GlobalTargets.OnlyServer);
            evnt.Send();
            waiting = false;
        }
    }

    private void DestroyWithTag(GameObject parent, string tag) {
        foreach (GameObject gobj in GenerationManager.instance.FindChildrenWithTag(parent,tag)) {
            Destroy(gobj);
        }
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason) {
        Debug.Log("Bolt is shutting down");
        foreach (Player controllerPlayer in ReInput.players.AllPlayers)
            controllerPlayer.isPlaying = false;
        SceneManager.LoadScene("Title");
    }

    public override void OnEvent(WaitForMap evnt) {
        if (BoltNetwork.Entities.Count() >= evnt.NumberEntities) {
            ReadySpawn ready = ReadySpawn.Create(Bolt.GlobalTargets.OnlyServer);
            ready.Send();
        } else {
            waiting = true;
            waitFor = evnt.NumberEntities;
        }
    }

    public override void OnEvent(SpawnPlayer evnt) {
        LocalPlayerRegistry.SpawnPlayer(evnt);
    }

    public override void OnEvent(GameStart evnt) {
        //Debug.Log("Batching rooms.");
        //StaticBatchingUtility.Combine(GameObject.Find("Batch Root"));
        SceneLoader.Instance.CancelLoadScreen();
    }

    public override void OnEvent(MatchComplete evnt) {
        bool amWinner = LocalPlayerRegistry.PlayerEntities.Contains(evnt.Winner);
        EndScreen.Instance.OpenEndScreen(amWinner);
    }
}

