using Mirror;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : NetworkBehaviour
{
    public static EndScreen Instance;
    public GameObject endScreen;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI leaderboardText;
    public Button lobbyButton;

    // Start is called before the first frame update
    void Start() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    /*
    public void OpenEndScreen(bool victor) {
        foreach (BoltEntity player in LocalPlayerRegistry.PlayerEntities) {
            player.GetComponent<PlayerMovementController>().InputDisabled = true;
        }
        lobbyButton.gameObject.SetActive(isServer);
        if (victor) {
            victoryText.text = "Wizard Triumph!\n";
        } else {
            victoryText.text = "Wizard Defeat..\n";
        }
        int i = 1;
        foreach (string pl in GameMaster.instance.WinningOrder) {
            leaderboardText.text = leaderboardText.text + "#" + i + ": " + pl + "\n";
            i++;
        }

        endScreen.SetActive(true);
    }

    public void ReturnToLobby() {
        if (isServer) {
            BoltNetwork.LoadScene("Lobby");
        }
    }

    public void ReturnToMenu() {
        if (isServer) {
            foreach (BoltConnection client in BoltNetwork.Clients) {
                client.Disconnect();
            }
            BoltLauncher.Shutdown();
        } else {
            DisconnectPlayer.Create().Send();
        }
    }*/
}
