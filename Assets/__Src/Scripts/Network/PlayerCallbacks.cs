using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private int receivedRooms = 0;
    private SpawnPlayer heldSpawnEvent;

    public override void SceneLoadLocalDone(string scene) {
        SplitscreenManager.Instantiate();
    }

    public override void EntityReceived(BoltEntity entity) {
        if (entity.StateIs(typeof(IRoomObject))) {
            receivedRooms++;
            if (heldSpawnEvent != null && receivedRooms >= heldSpawnEvent.WaitForRooms) {
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
        heldSpawnEvent = evnt;
        if (BoltNetwork.IsServer || receivedRooms >= evnt.WaitForRooms) {
            FinallySpawnPlayer();
        } 
    }

    private void FinallySpawnPlayer() {
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, heldSpawnEvent.Position, Quaternion.identity);
        IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state;
        playerEntity.GetComponent<PlayerUI>().ScreenNumber = SplitscreenManager.instance.CreatePlayerCamera(playerEntity.transform);
        playerState.Color = heldSpawnEvent.Color;
        playerState.Name = heldSpawnEvent.Name;
        playerState.PlayerId = heldSpawnEvent.PlayerId;
        playerEntity.TakeControl();
    }

    public override void OnEvent(GameStart evnt) {
        SceneLoader.Instance.CancelLoadScreen();
    }
}

