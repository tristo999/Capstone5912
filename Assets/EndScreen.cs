using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public static EndScreen Instance;
    public GameObject endScreen;
    public TextMeshProUGUI victoryText;
    public Button lobbyButton;

    // Start is called before the first frame update
    void Start() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void OpenEndScreen(bool victor) {
        foreach (BoltEntity player in LocalPlayerRegistry.PlayerEntities) {
            player.GetComponent<PlayerMovementController>().InputDisabled = true;
        }
        lobbyButton.gameObject.SetActive(BoltNetwork.IsServer);
        if (victor) {
            victoryText.text = "Wizard Triumph!";
        } else {
            victoryText.text = "Wizard Defeat..";
        }

        endScreen.SetActive(true);
    }

    public void ReturnToLobby() {
        if (BoltNetwork.IsServer) {
            BoltNetwork.LoadScene("Lobby");
        }
    }

    public void ReturnToMenu() {
        if (BoltNetwork.IsServer) {
            foreach (BoltConnection client in BoltNetwork.Clients) {
                client.Disconnect();
            }
            BoltLauncher.Shutdown();
        } else {
            DisconnectPlayer.Create().Send();
        }
    }
}
