using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetworkedManager : Bolt.EntityEventListener<ILobbyState>
{
    public static LobbyNetworkedManager Instance;

    public List<GameObject> PlayerModels = new List<GameObject>();
    public List<TextMeshProUGUI> PlayerNames = new List<TextMeshProUGUI>();
    public List<GameObject> ReadyObjects = new List<GameObject>();
    public Image ReadyWheel;

    private int waitFrame;

    public override void Attached() {
        Instance = this;

        // Todo: remove this once a way to set max players is established.
        if (entity.isOwner) {
            Debug.Log("Setting Max Players");
            state.MaxPlayers = 4;
        }

        state.AddCallback("Players[]", PlayersChanged);
        state.AddCallback("GameStarting", OnGameStarting);
    }

    public override void SimulateOwner() {
        if (state.NumPlayers > 1 && state.Players.All(p => p.Ready ||  !p.Present)) {
            state.GameStarting = true;
        } else if (state.GameStarting) {
            state.GameStarting = false;
        }
    }

    private void Update() {
        if (!entity.isAttached) return;

        if (state.GameStarting) {
            if (BoltNetwork.IsServer && BoltNetwork.ServerFrame >= waitFrame + 120) {
                BoltNetwork.LoadScene("WizardFightGame");
            } else {
                ReadyWheel.fillAmount = (BoltNetwork.ServerFrame - waitFrame) / 120f;
            }
        }
        

        foreach (Player player in ReInput.players.Players) {
            if (!player.isPlaying && player.GetAnyButton()) {
                AttemptAddPlayer(player);
            } else if (player.isPlaying && player.GetButtonDown("Interact")) {
                PlayerReadyToggle ready = PlayerReadyToggle.Create(entity);
                ready.PlayerId = LocalPlayerRegistry.IdFromPlayer(player);
                ready.Send();
            }
        }
    }

    private void OnGameStarting() {
        if (state.GameStarting) {
            waitFrame = BoltNetwork.ServerFrame;
        } else {
            ReadyWheel.fillAmount = 0f;
        }
    }

    private void PlayersChanged(Bolt.IState boltState, string path, Bolt.ArrayIndices indices) {
        for (int i = 0; i < 8; i++) {
            if (i < state.NumPlayers) {
                PlayerModels[i].SetActive(true);
                PlayerModels[i].GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", state.Players[i].Color);
                PlayerNames[i].text = state.Players[i].Name;
                ReadyObjects[i].SetActive(state.Players[i].Ready);
            }
        }
    }

    public override void OnEvent(LobbyPlayerJoined evnt) {
        state.Players[state.NumPlayers].Present = true;
        state.Players[state.NumPlayers].Name = "Player " + (state.NumPlayers + 1);
        state.Players[state.NumPlayers].Color = Random.ColorHSV();
        if (evnt.RaisedBy == null) {
            WizardFightPlayerRegistry.AddServerPlayer(state.Players[state.NumPlayers]);
        } else {
            WizardFightPlayerRegistry.AddLobbyPlayer(evnt.RaisedBy, state.Players[state.NumPlayers]);
        }
        state.NumPlayers++;
    }

    public override void OnEvent(PlayerReadyToggle evnt) {
        state.Players[evnt.PlayerId].Ready = !state.Players[evnt.PlayerId].Ready;
    }

    private void AttemptAddPlayer(Player player) {
        Debug.Log("Trying to add player " + player.id);
        if (state.NumPlayers < state.MaxPlayers) {
            player.isPlaying = true;
            LocalPlayerRegistry.Players.Add(player);
            LocalPlayerRegistry.PlayerNumbers.Add(state.NumPlayers);
            LocalPlayerRegistry.LobbyPlayers.Add(state.Players[state.NumPlayers]);
            LobbyPlayerJoined.Create(entity).Send();
        }
    }

    public void RemovePlayersFromConnection(BoltConnection connection) {
        List<WizardFightPlayerObject> disconnected = WizardFightPlayerRegistry.Players.Where(p => p.connection == connection).ToList();
        for (int i = 0; i < state.Players.Length; i++) {
            if (disconnected.Any(p => p.PlayerId == state.Players[i].PlayerId)) {
                state.Players[i].Color = Color.white;
                state.Players[i].Name = null;
                state.Players[i].Present = false;
                state.Players[i].Ready = false;
            }
        }
    }
}
