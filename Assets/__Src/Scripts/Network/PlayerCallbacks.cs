using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private int waitFor = 0;
    private bool waiting = false;

    public override void SceneLoadLocalDone(string scene) {
        SplitscreenManager.Instantiate();
    }

    public override void EntityAttached(BoltEntity entity) {
        if (waiting && BoltNetwork.Entities.Count() >= waitFor) {
            ReadySpawn evnt = ReadySpawn.Create(Bolt.GlobalTargets.OnlyServer);
            evnt.Send();
            waiting = false;
        }
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
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
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, evnt.Position, Quaternion.identity);
        IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state;
        playerEntity.GetComponent<PlayerUI>().ScreenNumber = SplitscreenManager.instance.CreatePlayerCamera(playerEntity.transform);
        playerState.Color = evnt.Color;
        playerState.Name = evnt.Name;
        playerState.PlayerId = evnt.PlayerId;
        playerEntity.TakeControl();
    }

    public override void OnEvent(GameStart evnt) {
        SceneLoader.Instance.CancelLoadScreen();
    }
}

