using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Lobby")]
public class LobbyCallbacks : Bolt.GlobalEventListener
{
    BoltEntity lobbyManager;

    private void Awake() {

    }
}
