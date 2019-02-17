using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitscreenManager : BoltSingletonPrefab<SplitscreenManager>
{
    [Flags]
    public enum SplitScreenMode { WithPreview, WithoutPreview, VerticalSplitscreen, HorizontalSplitscreen }

    public List<PlayerCamera> playerCameras { get; private set; } = new List<PlayerCamera>();
    public PlayerCamera previewCamera { get; private set; }
    private GameObject playerCamPrefab;

    private void Awake() {
        playerCamPrefab = Resources.Load("UI/PlayerCamera") as GameObject;
    }

    public void CreatePlayerCamera(Transform player) {
        PlayerCamera newCam = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
        newCam.AddPlayerToCamera(player);
        playerCameras.Add(newCam);
        // Change this call to take the splitscreenmode preference from options.
        SetPlayerLayout(playerCameras.Count, SplitScreenMode.WithPreview | SplitScreenMode.VerticalSplitscreen);
        SetCullingMasks();
        UpdatePreviewCamera();
    }

    public void CreatePreviewCamera() {
        previewCamera = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
        previewCamera.camera.rect = new Rect(.5f, .5f, .5f, .5f);
        previewCamera.camera.ResetProjectionMatrix();
        UpdatePreviewCamera();
    }

    private void UpdatePreviewCamera() {
        if (!previewCamera) return;
        previewCamera.ClearTargets();
        foreach (CinemachineTargetGroup.Target t in previewCamera.targetGroup.m_Targets) {
            previewCamera.targetGroup.AddMember(t.target, 1, 1);
        }
    }

    private void SetCullingMasks() {
        for (int i = 0; i < playerCameras.Count; i++) {
            playerCameras[i].GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = 8 + i;
            for (int j = 0; j < playerCameras.Count; j++) {
                if (i != j)
                    playerCameras[i].camera.cullingMask = playerCameras[i].camera.cullingMask & ~(1 << 8);
            }
        }
    }

    public void SetPlayerLayout(int playerCount, SplitScreenMode mode) {
        if (playerCount == 1) {
            playerCameras[0].camera.rect = new Rect(0, 0, 1, 1);
        } else if (playerCount == 2) {
            if (mode.HasFlag(SplitScreenMode.VerticalSplitscreen)) {
                playerCameras[0].camera.rect = new Rect(0, 0, 1f, 1f);
                playerCameras[1].camera.rect = new Rect(.5f, 0f, 1f, 1f);
            } else {
                playerCameras[0].camera.rect = new Rect(0, 0, 1f, 1f);
                playerCameras[1].camera.rect = new Rect(0, -.5f, 1f, 1f);
            }
        } else if (playerCount == 3) {
            if (mode.HasFlag(SplitScreenMode.WithPreview)) {
                playerCameras[0].camera.rect = new Rect(0, 0, 1f, 1f);
                playerCameras[1].camera.rect = new Rect(.5f, 0f, 1f, 1f);
                playerCameras[2].camera.rect = new Rect(0, -.5f, 1f, 1f);
                CreatePreviewCamera();
            } else {
                playerCameras[0].camera.rect = new Rect(0, 0, 1f, 1f);
                playerCameras[1].camera.rect = new Rect(0, -.5f, 1f, 1f);
                playerCameras[2].camera.rect = new Rect(.5f, -.5f, 1f, 1f);
            }
        } else if (playerCount == 4) {
            playerCameras[0].camera.rect = new Rect(0, 0, 1f, 1f);
            playerCameras[1].camera.rect = new Rect(.5f, 0f, 1f, 1f);
            playerCameras[2].camera.rect = new Rect(0, -.5f, 1f, 1f);
            playerCameras[3].camera.rect = new Rect(.5f, -.5f, 1f, 1f);
        } else {
            Debug.LogFormat("{0} is not a valid number of players.", playerCount);
        }

        foreach (PlayerCamera c in playerCameras) {
            c.camera.ResetProjectionMatrix();
        }
    }
}
