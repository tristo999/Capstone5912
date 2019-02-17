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
        SplitscreenManager.instance.CreatePlayerCamera(entity.transform);
    }
}

