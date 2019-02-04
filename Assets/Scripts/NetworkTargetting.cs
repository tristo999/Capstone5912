using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTargetting : Bolt.GlobalEventListener
{
    private Cinemachine.CinemachineTargetGroup tg;

    public void Start() {
        tg = GetComponent<Cinemachine.CinemachineTargetGroup>();
    }

    public void AddAllPlayers() {
        StartCoroutine(DelayedAdd());
    }

    IEnumerator DelayedAdd() {
        Debug.Log("Gonna look for players");
        yield return new WaitForSeconds(1);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
            Debug.Log("Adding player to camera");
            tg.AddMember(g.transform, 1f, 1f);
        }
    }

    public override void OnEvent(PlayerJoined evnt) {
        tg.AddMember(evnt.Player.transform, 1, 1);
    }
}
