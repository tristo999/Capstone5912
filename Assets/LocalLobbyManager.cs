using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalLobbyManager : MonoBehaviour
{
    public static LocalLobbyManager Instance;

    public List<GameObject> wizardObjects;
    public List<TMP_InputField> wizardNames;
    public List<GameObject> wizardReadies;

    public void Awake() {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    public void SetWizards(List<PlayerInfo> infos) {
        for (int i = 0; i < infos.Count; i++) {
            wizardObjects[i].SetActive(true);
            wizardObjects[i].GetComponentsInChildren<Renderer>()[1].material.color = infos[i].color;
            wizardNames[i].text = infos[i].Name;
            wizardReadies[i].SetActive(infos[i].Ready);
        }
        
    }

    public void RemoveWizard(int id) {
        wizardObjects[id].SetActive(false);
        wizardNames[id].gameObject.SetActive(false);
        wizardReadies[id].SetActive(false);
    }

    public void SetLobbyName(string name) {

    }

    public void UpdateLobbyName() {
        if (!NetworkServer.active) return;
        LobbyInfoMessage message = new LobbyInfoMessage();
        message.lobbyName = "";
        NetworkServer.SendToAll(message);
    }

    public void ToggleReady(int controllerId) {
        ToggleReadyMessage mes = new ToggleReadyMessage() { ControllerId = controllerId };
        NetworkClient.Send(mes);
    }
}
