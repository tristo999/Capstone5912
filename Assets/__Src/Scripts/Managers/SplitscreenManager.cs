using Cinemachine;
using QuickGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitscreenManager : BoltSingletonPrefab<SplitscreenManager>
{
    [Flags]
    public enum SplitScreenMode { WithPreview = 1, WithoutPreview = 2, VerticalSplitscreen = 4, HorizontalSplitscreen = 8 }

    public List<PlayerCamera> playerCameras { get; private set; } = new List<PlayerCamera>();
    public PlayerCamera previewCamera { get; private set; }
    private List<Renderer> renderers = new List<Renderer>();
    private GameObject playerCamPrefab;

    private void Awake() {
        playerCamPrefab = Resources.Load("UI/PlayerCamera") as GameObject;
    }

    public PlayerCamera GetEntityCamera(BoltEntity entity) {
        return playerCameras.First(p => p.CameraPlayer == entity);
    }

    public int CreatePlayerCamera(Transform player) {
        PlayerCamera newCam = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
        newCam.AddPlayerToCamera(player);
        playerCameras.Add(newCam);
        // Change this call to take the splitscreenmode preference from options.
        SetPlayerLayout(playerCameras.Count, SplitScreenMode.WithoutPreview | SplitScreenMode.VerticalSplitscreen);
        SetCullingMasks();
        UpdatePreviewCamera();
        return playerCameras.Count;
    }

    public void CreatePreviewCamera() {
        previewCamera = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
        previewCamera.camera.rect = new Rect(.5f, 0f, .5f, .5f);
        playerCameras.Add(previewCamera);
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
                    playerCameras[i].camera.cullingMask = playerCameras[i].camera.cullingMask & ~(1 << 8 + j);
            }
        }
    }

    public void SetPlayerLayout(int playerCount, SplitScreenMode mode) {
        if (playerCount == 1) {
            playerCameras[0].camera.rect = new Rect(0, 0, 1, 1);
        } else if (playerCount == 2) {
            if (mode.HasFlag(SplitScreenMode.VerticalSplitscreen)) {
                playerCameras[0].camera.rect = new Rect(0, 0, .5f, 1f);
                playerCameras[1].camera.rect = new Rect(.5f, 0f, .5f, 1f);
            } else {
                playerCameras[0].camera.rect = new Rect(0, .5f, 1f, .5f);
                playerCameras[1].camera.rect = new Rect(0, 0f, 1f, .5f);
            }
        } else if (playerCount == 3) {
            if (mode.HasFlag(SplitScreenMode.WithPreview)) {
                playerCameras[0].camera.rect = new Rect(0f, .5f, .5f, .5f);
                playerCameras[1].camera.rect = new Rect(.5f, .5f, .5f, .5f);
                playerCameras[2].camera.rect = new Rect(0f, 0f, .5f, .5f);
                CreatePreviewCamera();
            } else {
                playerCameras[0].camera.rect = new Rect(0f, .5f, 1f, .5f);
                playerCameras[1].camera.rect = new Rect(0f, 0f, .5f, .5f);
                playerCameras[2].camera.rect = new Rect(.5f, 0f, .5f, .5f);
            }
        } else if (playerCount == 4) {
            playerCameras[0].camera.rect = new Rect(0, .5f, .5f, .5f);
            playerCameras[1].camera.rect = new Rect(.5f, .5f, .5f, .5f);
            playerCameras[2].camera.rect = new Rect(0, 0f, .5f, .5f);
            playerCameras[3].camera.rect = new Rect(.5f, 0f, .5f, .5f);
        } else {
            Debug.LogFormat("{0} is not a valid number of players.", playerCount);
        }
    }

    public void DoRoomCulling() {
        FreezeDistant.Create().Send();
        if (renderers.Count == 0) {
            Renderer[] rend = Resources.FindObjectsOfTypeAll<Renderer>();
            renderers.AddRange(rend.Where(r => r.gameObject.layer == 14 || r.gameObject.layer == 15 || r.gameObject.layer == 16));
        }

        foreach (Renderer ren in renderers) {
            ren.enabled = false;
        }

        foreach (PlayerCamera cam in playerCameras) {
            foreach (Collider col in Physics.OverlapSphere(cam.CameraPlayer.transform.position, GenerationManager.instance.roomSize * 1.25f, (1 << 14) | (1 << 15) | (1 << 16), QueryTriggerInteraction.Collide)) {
                foreach (Renderer ren in col.GetComponentsInChildren<Renderer>()) {
                    ren.enabled = true;
                }
            }
        }
    }
}


