using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene) {
        GameObject cube = BoltNetwork.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane));
        SplitscreenManager.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
        entity.GetComponent<PlayerUI>().ScreenNumber = SplitscreenManager.instance.CreatePlayerCamera(entity.transform);
    }

    public override void OnEvent(SpawnPlayer evnt) {
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player);
        playerEntity.transform.position = evnt.Position;
        IPlayerState playerState = playerEntity.GetComponent<PlayerMovementController>().state; ;
        playerState.Color = evnt.Color;
        playerState.Name = evnt.Name;
        playerState.PlayerId = evnt.PlayerId;
        playerEntity.TakeControl();
    }
}

