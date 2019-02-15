using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class NetworkManager : Bolt.GlobalEventListener
{
    public void HostGame() {
        BoltLauncher.StartServer();
    }

    public void JoinGame() {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone() {
        if (BoltNetwork.IsServer) {
            Debug.Log("Bolt done starting");
            string matchName = Guid.NewGuid().ToString();

            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene("Lobby");
            SceneLoader.Instance.LoadScreenAsync(BoltNetwork.CurrentAsyncOperation);
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList) {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon) {
                BoltNetwork.Connect(photonSession);
                SceneLoader.Instance.LoadScreenAsync(BoltNetwork.CurrentAsyncOperation);
            }
        }
    }
}
