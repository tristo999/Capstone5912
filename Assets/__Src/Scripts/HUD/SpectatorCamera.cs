using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    enum SpectatorCamState { Orbital, Follow }

    public Camera camera;
    public CinemachineVirtualCamera orbitalCam;
    public CinemachineOrbitalTransposer orbitalTransposer;

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
        camera = GetComponentInChildren<Camera>();
        orbitalTransposer = orbitalCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        ChangeToOrbital();
    }

    void FixedUpdate()
    {
        if (state == SpectatorCamState.Orbital) {
            orbitalTransposer.m_XAxis.Value += .5f;
            if (Input.GetMouseButtonDown(0)) {
                ChangeToFollow();
            }
        } else if (state == SpectatorCamState.Follow) {
            if (Input.GetMouseButtonDown(0)) {
                ChangeToOrbital();
            }
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
}
