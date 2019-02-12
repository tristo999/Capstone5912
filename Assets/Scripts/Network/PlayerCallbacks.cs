using Rewired;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private void Awake() {
        
    }

    public override void SceneLoadLocalDone(string scene) {
        GameStateManager.GameState = GameStateManager.State.Online;
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        PlayerCamera.Instantiate();
        // Initial camera target assigning goes here.
    }
}

