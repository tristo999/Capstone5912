[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    private void Awake() {
        
    }

    public override void SceneLoadLocalDone(string scene) {
        
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        PlayerCamera.Instantiate();
        // Initial camera target assigning goes here.
    }
}

