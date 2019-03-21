using Cinemachine;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    enum SpectatorCamState { Orbital, Follow }

    public Camera spectatorCam;
    public CinemachineVirtualCamera orbitalCam;
    public CinemachineOrbitalTransposer orbitalTransposer;
    public Player rewiredPlayer;
    private int currentTargetIndex;

    public CinemachineVirtualCamera followCam;

    public Transform followTarget {
        get
        {
            return _followTarget;
        }
        set
        {
            _followTarget = value;
            orbitalCam.Follow = _followTarget;
            orbitalCam.LookAt = _followTarget;
            followCam.Follow = _followTarget;
        }
    }

    private SpectatorCamState state;
    private Transform _followTarget;

    private void Awake() {
        spectatorCam = GetComponentInChildren<Camera>();
        orbitalTransposer = orbitalCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        ChangeToOrbital();
    }

    void FixedUpdate()
    {
        if (state == SpectatorCamState.Orbital) {
            orbitalTransposer.m_XAxis.Value += .5f;
            if (rewiredPlayer.GetButtonDown("Interact")) {
                ChangeToFollow();
            }
        } else if (state == SpectatorCamState.Follow) {
            if (rewiredPlayer.GetButtonDown("Interact")) {
                ChangeToOrbital();
            }
        }
        if (rewiredPlayer.GetButtonDown("Fire")) {
            currentTargetIndex++;
            if (currentTargetIndex >= GameMaster.instance.LivePlayers.Count) {
                currentTargetIndex = 0;
            }
            followTarget = GameMaster.instance.LivePlayers[currentTargetIndex].transform;
        }
    }

    public void ChangeToOrbital() {
        state = SpectatorCamState.Orbital;
        orbitalCam.gameObject.SetActive(true);
        followCam.gameObject.SetActive(false);
        orbitalTransposer.m_XAxis.Value = 0f;
    }

    public void ChangeToFollow() {
        state = SpectatorCamState.Follow;
        followCam.gameObject.SetActive(true);
        orbitalCam.gameObject.SetActive(false);
    }

    public void GetRandomTarget() {
        followTarget = GameMaster.instance.LivePlayers[Random.Range(0, GameMaster.instance.LivePlayers.Count)].transform;
    }
}
