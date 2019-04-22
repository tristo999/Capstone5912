using Cinemachine;
using DG.Tweening;
using QuickGraph;
using Rewired;
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
    public List<SpectatorCamera> spectatorCameras { get; private set; } = new List<SpectatorCamera>();
    private List<Renderer> renderers = new List<Renderer>();
    private List<Light> lights = new List<Light>();
    private GameObject playerCamPrefab;
    private GameObject spectatorCamPrefab;

    private void Awake() {
        playerCamPrefab = Resources.Load("UI/PlayerCamera") as GameObject;
        spectatorCamPrefab = Resources.Load("UI/SpectatorCam") as GameObject;
    }

    public PlayerCamera GetEntityCamera(BoltEntity entity) {
        return playerCameras.First(p => p.CameraPlayer == entity);
    }

    public int CreatePlayerCamera(Transform player) {
        PlayerCamera newCam = Instantiate(playerCamPrefab).GetComponent<PlayerCamera>();
        SpectatorCamera spectCam = Instantiate(spectatorCamPrefab).GetComponent<SpectatorCamera>();
        newCam.AddPlayerToCamera(player);
        playerCameras.Add(newCam);
        spectatorCameras.Add(spectCam);
        // Change this call to take the splitscreenmode preference from options.
        SetPlayerLayout(playerCameras.Count, SplitScreenMode.WithoutPreview | SplitScreenMode.VerticalSplitscreen);
        spectCam.gameObject.SetActive(false);
        SetCullingMasks();
        return playerCameras.Count;
    }

    private void SetCullingMasks() {
        for (int i = 0; i < playerCameras.Count; i++) {
            playerCameras[i].GetComponentsInChildren<CinemachineVirtualCamera>().ToList().ForEach(c => c.gameObject.layer = 8 + i);
            spectatorCameras[i].GetComponentsInChildren<CinemachineVirtualCamera>().ToList().ForEach(c => c.gameObject.layer = 8 + i);

            for (int j = 0; j < playerCameras.Count; j++) {
                if (i != j) {
                    playerCameras[i].camera.cullingMask = playerCameras[i].camera.cullingMask & ~(1 << 8 + j);
                    spectatorCameras[i].spectatorCam.cullingMask = spectatorCameras[i].spectatorCam.cullingMask & ~(1 << 8 + j);
                }
            }
        }
    }

    public void Reset() {
        playerCameras.ForEach(o => Destroy(o.gameObject));
        spectatorCameras.ForEach(o => Destroy(o.gameObject));
        playerCameras.Clear();
        spectatorCameras.Clear();
    }

    public void SetPlayerLayout(int playerCount, SplitScreenMode mode) {
        if (playerCount == 1) {
            playerCameras[0].camera.rect = new Rect(0, 0, 1, 1);
            spectatorCameras[0].spectatorCam.rect = new Rect(0, 0, 1, 1);
        } else if (playerCount == 2) {
            if (mode.HasFlag(SplitScreenMode.VerticalSplitscreen)) {
                playerCameras[0].camera.rect = new Rect(0, 0, .5f, 1f);
                spectatorCameras[0].spectatorCam.rect = new Rect(0, 0, .5f, 1f);
                playerCameras[1].camera.rect = new Rect(.5f, 0f, .5f, 1f);
                spectatorCameras[1].spectatorCam.rect = new Rect(.5f, 0f, .5f, 1f);
            } else {
                playerCameras[0].camera.rect = new Rect(0, .5f, 1f, .5f);
                spectatorCameras[0].spectatorCam.rect = new Rect(0, .5f, 1f, .5f);
                playerCameras[1].camera.rect = new Rect(0, 0f, 1f, .5f);
                spectatorCameras[1].spectatorCam.rect = new Rect(0, 0f, 1f, .5f);
            }
        } else if (playerCount == 3) {
            if (mode.HasFlag(SplitScreenMode.WithPreview)) {
                playerCameras[0].camera.rect = new Rect(0f, .5f, .5f, .5f);
                spectatorCameras[0].spectatorCam.rect = new Rect(0f, .5f, .5f, .5f);
                spectatorCameras[0].spectatorCam.rect = new Rect(0f, .5f, .5f, .5f);
                playerCameras[1].camera.rect = new Rect(.5f, .5f, .5f, .5f);
                spectatorCameras[1].spectatorCam.rect = new Rect(.5f, .5f, .5f, .5f);
                playerCameras[2].camera.rect = new Rect(0f, 0f, .5f, .5f);
                spectatorCameras[2].spectatorCam.rect = new Rect(0f, 0f, .5f, .5f);
            } else {
                playerCameras[0].camera.rect = new Rect(0f, .5f, 1f, .5f);
                spectatorCameras[0].spectatorCam.rect = new Rect(0f, .5f, 1f, .5f);
                playerCameras[1].camera.rect = new Rect(0f, 0f, .5f, .5f);
                spectatorCameras[1].spectatorCam.rect = new Rect(0f, 0f, .5f, .5f);
                playerCameras[2].camera.rect = new Rect(.5f, 0f, .5f, .5f);
                spectatorCameras[2].spectatorCam.rect = new Rect(.5f, 0f, .5f, .5f);
            }
        } else if (playerCount == 4) {
            playerCameras[0].camera.rect = new Rect(0, .5f, .5f, .5f);
            spectatorCameras[0].spectatorCam.rect = new Rect(0, .5f, .5f, .5f);
            playerCameras[1].camera.rect = new Rect(.5f, .5f, .5f, .5f);
            spectatorCameras[1].spectatorCam.rect = new Rect(.5f, .5f, .5f, .5f);
            playerCameras[2].camera.rect = new Rect(0, 0f, .5f, .5f);
            spectatorCameras[2].spectatorCam.rect = new Rect(0, 0f, .5f, .5f);
            playerCameras[3].camera.rect = new Rect(.5f, 0f, .5f, .5f);
            spectatorCameras[3].spectatorCam.rect = new Rect(.5f, 0f, .5f, .5f);
        } else {
            Debug.LogFormat("{0} is not a valid number of players.", playerCount);
        }
    }

    public void SetCameraToSpectator(int cam, Player rewiredPlayer) {
        playerCameras[cam].gameObject.SetActive(false);
        spectatorCameras[cam].gameObject.SetActive(true);
        spectatorCameras[cam].rewiredPlayer = rewiredPlayer;
        spectatorCameras[cam].GetRandomTarget();
        if (spectatorCameras.All(c => c.gameObject.activeInHierarchy)) {
            Camera.main.DORect(new Rect(0, 0, 1, 1), 1.5f).OnComplete(() => {
                for (int i = 1; i < spectatorCameras.Count; i++) {
                    spectatorCameras[i].gameObject.SetActive(false);
                }
            });
        }
    }

    public void DoRoomCulling() {
        FreezeDistant.Create().Send();
        
        if (renderers.Count == 0) {
            Renderer[] rend = FindObjectsOfType<Renderer>();
            renderers.AddRange(rend.Where(r => r.gameObject.layer == 14 || r.gameObject.layer == 15 || r.gameObject.layer == 16));
            lights = FindObjectsOfType<Light>().ToList();
        }

        foreach (Renderer ren in renderers) {
            if (ren) {
                ren.enabled = false;
            }
        }
        foreach (Light light in lights) {
            if (light) {
                light.enabled = false;
            }
        }

        foreach (PlayerCamera cam in playerCameras) {
            foreach (Renderer ren in renderers) {
                if (ren && Vector3.Distance(ren.transform.position, cam.CameraPlayer.transform.position) < GenerationManager.instance.roomSize * 3.5f) {
                    ren.enabled = true;
                }
            }

            foreach (Light light in lights) {
                if (light && Vector3.Distance(light.transform.position, cam.CameraPlayer.transform.position) < GenerationManager.instance.roomSize * 3.5f) {
                    light.enabled = true;
                }
            }
        }
    }
}


