﻿
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class WizardFightNetworkManager : NetworkManager
{
    // Both
    private List<PlayerInfo> players = new List<PlayerInfo>();

    // Lobby
    string lobbyName;

    // In Game
    private List<GameObject> playerObjects = new List<GameObject>();
    

    public override void OnStartServer() {
        base.OnStartServer();
        NetworkServer.RegisterHandler<AddPlayerMessage>(OnAddPlayer);
        NetworkServer.RegisterHandler<RemovePlayerMessage>(OnRemovePlayer);
        NetworkServer.RegisterHandler<UpdatePlayerMessage>(OnUpdatePlayer);
        NetworkServer.RegisterHandler<ToggleReadyMessage>(OnToggleReady);
    }

    public override void OnStartClient() {
        base.OnStartClient();
        NetworkClient.RegisterHandler<LobbyInfoMessage>(OnReceiveLobby);
    }

    void OnAddPlayer(NetworkConnection con, AddPlayerMessage message) {
        PlayerInfo info = message.info;
        info.ConnectionId = con.connectionId;
        players.Add(info);
        BroadcastLobby();
    }

    void OnRemovePlayer(NetworkConnection con, RemovePlayerMessage message) {
        PlayerInfo toRemove = players.First(p => p.ConnectionId == con.connectionId && p.LocalController == message.LocalController);
        players.Remove(toRemove);
    }

    void OnUpdatePlayer(NetworkConnection con, UpdatePlayerMessage message) {
        PlayerInfo pl = players.First(p => p.ConnectionId == con.connectionId && p.LocalController == message.info.LocalController);
        int index = players.IndexOf(pl);
        players[index] = pl;
        BroadcastLobby();
    }

    void OnToggleReady(NetworkConnection con, ToggleReadyMessage mes) {
        PlayerInfo player = players.First(p => p.ConnectionId == con.connectionId && p.LocalController == mes.ControllerId);
        int idx = players.IndexOf(player);
        player.Ready = !player.Ready;
        players[idx] = player;
        BroadcastLobby();
    }

    void BroadcastLobby() {
        LobbyInfoMessage mes = new LobbyInfoMessage();
        mes.lobbyName = lobbyName;
        mes.players = players;
        NetworkServer.SendToAll(mes);
    }

    void OnReceiveLobby(NetworkConnection con, LobbyInfoMessage mes) {
        if (mes.players != null) {
            players = mes.players;
        }
        lobbyName = mes.lobbyName;
        LocalLobbyManager.Instance.SetLobbyName(lobbyName);
    }

    /*
    public GameObject ServerListPanel;
    public GameObject ServerList;
    public GameObject ServerInfoPanel;
    public Button JoinButton;

    private Dictionary<Guid, ServerInfoPanel> panels = new Dictionary<Guid, ServerInfoPanel>();
    private Guid selectedPanel;
    private WizardFightPooling pooling;

    public void LocalGame() {
        BoltLauncher.StartSinglePlayer();
    }

    public void HostGame() {
        BoltLauncher.StartServer();
    }

    public void JoinGame() {
        BoltLauncher.StartClient();
    }

    public override void BoltStartBegin() {
        BoltNetwork.RegisterTokenClass<LobbyProtocol>();
    }

    public override void BoltStartDone() {
        if (isServer) {
            Debug.Log("Bolt done starting");
            //string matchName = Guid.NewGuid().ToString();
            LobbyProtocol initProtocol = new LobbyProtocol();
            initProtocol.currentPlayers = 0;
            initProtocol.maxPlayers = 4;
            initProtocol.inLobby = true;
            initProtocol.lobbyName = "Wizard Fight Game";

            BoltNetwork.SetServerInfo(initProtocol.lobbyName, initProtocol);
            BoltNetwork.LoadScene("Lobby");
            SceneLoader.Instance.StartLoadScreen();
        } else {
            ServerListPanel.SetActive(true);
        }
    }

    public override void SceneLoadLocalDone(string scene) {
        pooling.LoadSceneObjects(scene);
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList) {
            UdpSession photonSession = session.Value as UdpSession;
            LobbyProtocol lobbyProtocol = (LobbyProtocol)session.Value.GetProtocolToken();

            if (!panels.ContainsKey(session.Key) && lobbyProtocol.inLobby) {
                GameObject newPanel = Instantiate(ServerInfoPanel, ServerList.transform);
                ServerInfoPanel panelInfo = newPanel.GetComponent<ServerInfoPanel>();
                panelInfo.guid = session.Key;
                panels.Add(session.Key, panelInfo);
            }

            if (lobbyProtocol.inLobby) {
                panels[session.Key].ServerName = lobbyProtocol.lobbyName;
                panels[session.Key].MaxPlayers = lobbyProtocol.maxPlayers;
                panels[session.Key].CurrentPlayers = lobbyProtocol.currentPlayers;
            } else if (panels.ContainsKey(session.Key)) {
                Destroy(panels[session.Key].gameObject);
                panels.Remove(session.Key);
            }
        }

        List<Guid> noLongerAvailable = new List<Guid>();
        foreach (var panel in panels.Where(x => !sessionList.Any(y => y.Key == x.Key))) {
            Destroy(panel.Value.gameObject);
            noLongerAvailable.Add(panel.Key);
        }
        foreach (Guid guid in noLongerAvailable) {
            panels.Remove(guid);
        }
    }

    public void PanelSelect(Guid guid) {
        selectedPanel = guid;
    }

    public void BackFromList() {
        BoltLauncher.Shutdown();
        ServerListPanel.SetActive(false);
    }

    public void Connect() {
        UdpSession photonSession = BoltNetwork.SessionList[selectedPanel];
        if (selectedPanel != null && photonSession.Source == UdpSessionSource.Photon) {
            BoltNetwork.Connect(photonSession);
            SceneLoader.Instance.StartLoadScreen();
        }
    }*/
}