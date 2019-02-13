[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private void Awake() {
        
    }

    public override void SceneLoadLocalDone(string scene) {
        
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        //PlayerCamera.Instantiate();
        IPlayerState playerState = entity.GetComponent<IPlayerState>();
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
        // Initial camera target assigning goes here.
    }
}

