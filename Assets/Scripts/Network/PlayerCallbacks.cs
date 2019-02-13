using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private void Awake() {
    }

    public override void SceneLoadLocalDone(string scene) {
        GameObject cube = BoltNetwork.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane));
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        //PlayerCamera.Instantiate();
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
        // Initial camera target assigning goes here.
    }
}

