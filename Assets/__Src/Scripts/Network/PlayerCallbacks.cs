using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private int receivedRooms = 0;
    private int waitFor;
    private List<WizardFightPlayerObject> heldSpawnEvents = new List<WizardFightPlayerObject>();
    private List<Vector3> positions = new List<Vector3>();

    public override void SceneLoadLocalDone(string scene) {
        SplitscreenManager.Instantiate();
    }

    public override void EntityAttached(BoltEntity entity) {
        if (entity.StateIs(typeof(IRoomObject))) {
            receivedRooms++;
            if (heldSpawnEvents.Count > 0 && receivedRooms >= waitFor) {
                FinallySpawnPlayer();
            }
        }
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
    }

    public override void OnEvent(SpawnPlayer evnt) {
        WizardFightPlayerObject obj = new WizardFightPlayerObject();
        obj.PlayerName = evnt.Name;
        obj.PlayerColor = evnt.Color;
        obj.PlayerId = evnt.PlayerId;
        heldSpawnEvents.Add(obj);
        positions.Add(evnt.Position);
        Debug.Log("Must wait for " + evnt.WaitForRooms + " room spawns.");
        if (BoltNetwork.IsServer || receivedRooms >= evnt.WaitForRooms) {
            FinallySpawnPlayer();
        } else {
            waitFor = evnt.WaitForRooms;
        }
    }

    private void FinallySpawnPlayer() {
        for (int i = 0; i < heldSpawnEvents.Count; i++) {
            BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, positions[i], Quaternion.identity);
            IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state;
            playerEntity.GetComponent<PlayerUI>().ScreenNumber = SplitscreenManager.instance.CreatePlayerCamera(playerEntity.transform);
            playerState.Color = heldSpawnEvents[i].PlayerColor;
            playerState.Name = heldSpawnEvents[i].PlayerName;
            playerState.PlayerId = heldSpawnEvents[i].PlayerId;
            playerEntity.TakeControl();
        }
        heldSpawnEvents.Clear();
        positions.Clear();
    }

    public override void OnEvent(GameStart evnt) {
        SceneLoader.Instance.CancelLoadScreen();
    }
}

