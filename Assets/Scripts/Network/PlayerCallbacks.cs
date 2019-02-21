using UnityEngine;

[BoltGlobalBehaviour("WizardFightGame")]
class PlayerCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene) {
        SplitscreenManager.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        IPlayerState playerState = entity.GetComponent<PlayerMovementController>().state;
        int localPlayerId = LocalPlayerRegistry.PlayerFromId(playerState.PlayerId).id;
        entity.GetComponent<PlayerMovementController>().AssignPlayer(localPlayerId);
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

