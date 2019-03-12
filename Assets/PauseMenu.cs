using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void OpenPauseMenu() {
        foreach (BoltEntity player in LocalPlayerRegistry.PlayerEntities) {
            player.GetComponent<PlayerMovementController>().InputDisabled = true;
        }
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu() {
        foreach (BoltEntity player in LocalPlayerRegistry.PlayerEntities) {
            player.GetComponent<PlayerMovementController>().InputDisabled = false;
        }
        pauseMenu.SetActive(false);
    }

    public void TogglePauseMenu() {
        if (pauseMenu.activeInHierarchy) {
            ClosePauseMenu();
        } else {
            OpenPauseMenu();
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
