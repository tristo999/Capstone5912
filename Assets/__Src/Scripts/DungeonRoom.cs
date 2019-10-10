using MyBox;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : NetworkBehaviour
{
    public int DistanceFromCenter = -1;
    public float DangerRating;
    [HideInInspector] public bool isSpawnRoom;
    public bool centerRoom;
    public GameObject northWall;
    [ConditionalField("centerRoom")] public GameObject northWall2;
    public GameObject eastWall;
    [ConditionalField("centerRoom")] public GameObject eastWall2;
    public GameObject southWall;
    [ConditionalField("centerRoom")] public GameObject southWall2;
    public GameObject westWall;
    [ConditionalField("centerRoom")] public GameObject westWall2;
    public GameObject northWallFlat;
    [ConditionalField("centerRoom")] public GameObject northWallFlat2;
    public GameObject eastWallFlat;
    [ConditionalField("centerRoom")] public GameObject eastWallFlat2;
    public GameObject southWallFlat;
    [ConditionalField("centerRoom")] public GameObject southWallFlat2;
    public GameObject westWallFlat;
    [ConditionalField("centerRoom")] public GameObject westWallFlat2;
    public GameObject northWallDestroyed;
    [ConditionalField("centerRoom")] public GameObject northWallDestroyed2;
    public GameObject eastWallDestroyed;
    [ConditionalField("centerRoom")] public GameObject eastWallDestroyed2;
    public GameObject southWallDestroyed;
    [ConditionalField("centerRoom")] public GameObject southWallDestroyed2;
    public GameObject westWallDestroyed;
    [ConditionalField("centerRoom")] public GameObject westWallDestroyed2;
    [Range(0.0f,1.0f)]
    public float chanceDestroyCarpet;
    [SyncVar(hook = nameof(OnCarpetChange))]
    public bool hasCarpet;
    [SyncVar(hook = nameof(OnCarpetChange))]
    public Color carpetColor;
    public ParticleSystem ceilingDust;
    private List<PlayerCamera> camerasInRoom = new List<PlayerCamera>();
    private float shakeTime = 1f;
    [SyncVar(hook = nameof(OnDestructionStateChange))]
    public DestructionState currentDestructionState;
    [SyncVar(hook = nameof(OnNorthWallChange))]
    public WallState northWallState;
    [SyncVar(hook = nameof(OnEastWallChange))]
    public WallState eastWallState;
    [SyncVar(hook = nameof(OnSouthWallChange))]
    public WallState southWallState;
    [SyncVar(hook = nameof(OnWestWallChange))]
    public WallState westWallState;

    public enum WallState { Door=1, Open=2, Closed=0, Destroyed=3 }
    public enum DestructionState { Normal, Warning, Danger, Critical, Destroyed }

    public void Awake() {
        if (!hasAuthority) return;
        if (Random.Range(0.0f, 1.0f) < chanceDestroyCarpet) {
            hasCarpet = false;
        } else {
            hasCarpet = true;
            carpetColor = Random.ColorHSV(0f, 1f, .6f, 1f, .6f, 1f);
        }
    }

    [Command]
    public void CmdChangeNorthWallState(WallState state) {
        northWallState = state;
    }
    [Command]
    public void CmdChangeEastWallState(WallState state) {
        eastWallState = state;
    }
    [Command]
    public void CmdChangeSouthWallState(WallState state) {
        southWallState = state;
    }
    [Command]
    public void CmdChangeWestWallState(WallState state) {
        westWallState = state;
    }

    private void OnCarpetChange() {
        /*
        carpet.SetActive(hasCarpet);
        carpet.GetComponent<Renderer>().material.color = carpetColor;
        */
    }

    private void OnNorthWallChange() {
        northWall.SetActive(false);
        northWallFlat.SetActive(false);
        northWallDestroyed.SetActive(false);
        if (northWallState == WallState.Door) {
            northWall.SetActive(true);
        } else if (northWallState == WallState.Closed) {
            northWallFlat.SetActive(true);
        } else if (northWallState == WallState.Destroyed) {
            northWallDestroyed.SetActive(true);
        }
    }

    private void OnEastWallChange(WallState state) {
        eastWall.SetActive(false);
        eastWallFlat.SetActive(false);
        eastWallDestroyed.SetActive(false);
        if (eastWallState == WallState.Door) {
            eastWall.SetActive(true);
        } else if (eastWallState == WallState.Closed) {
            eastWallFlat.SetActive(true);
        } else if (eastWallState == WallState.Destroyed) {
            eastWallDestroyed.SetActive(true);
        }
    }

    private void OnSouthWallChange(WallState state) {
        southWall.SetActive(false);
        southWallFlat.SetActive(false);
        southWallDestroyed.SetActive(false);
        if (southWallState == WallState.Door) {
            southWall.SetActive(true);
        } else if (southWallState == WallState.Closed) {
            southWallFlat.SetActive(true);
        } else if (southWallState == WallState.Destroyed) {
            southWallDestroyed.SetActive(true);
        }
    }

    private void OnWestWallChange(WallState state) {
        westWall.SetActive(false);
        westWallFlat.SetActive(false);
        westWallDestroyed.SetActive(false);
        if (westWallState == WallState.Door) {
            westWall.SetActive(true);
        } else if (westWallState == WallState.Closed) {
            westWallFlat.SetActive(true);
        } else if (westWallState == WallState.Destroyed) {
            westWallDestroyed.SetActive(true);
        }
    }

    private void OnDestructionStateChange() {
        if (currentDestructionState == DestructionState.Warning) {
            StartCoroutine(warnShake());
        }

        if (currentDestructionState == DestructionState.Critical) {
            ceilingDust.Play();
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.SetShake(1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(0, 1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 1f);
            }
        }

        if (currentDestructionState == DestructionState.Destroyed) {
            if (northWallState != WallState.Closed)
                northWallState = WallState.Destroyed;
            if (eastWallState != WallState.Closed)
                eastWallState = WallState.Destroyed;
            if (westWallState != WallState.Closed)
                westWallState = WallState.Destroyed;
            if (southWallState != WallState.Closed)
                southWallState = WallState.Destroyed;
            ceilingDust.Stop();
            GenerationManager.instance.CmdDestroyNeighborWalls(this);
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.CameraPlayer.GetComponent<PlayerStatsController>().Alive = false;
                cam.SetShake(0f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(0, 0f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 0f);
            }
        }
    }

    float intensity = .25f;
    IEnumerator warnShake() {
        float shakeTime = .5f;
        float timeBetweenShake = 1.5f;
        

        while (currentDestructionState == DestructionState.Warning || currentDestructionState == DestructionState.Danger) {
            if (currentDestructionState == DestructionState.Danger) {
                shakeTime = .75f;
                timeBetweenShake = .75f;
            }

            foreach (PlayerCamera cam in camerasInRoom) {
                cam.SetShake(intensity);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(0, intensity);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, intensity);
            }
            ceilingDust.Play();
            yield return new WaitForSeconds(shakeTime);
            if (currentDestructionState != DestructionState.Critical) {
                foreach (PlayerCamera cam in camerasInRoom) {
                    cam.SetShake(0f);
                    cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(0, 0f);
                    cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 0f);
                }
                ceilingDust.Stop();
                ceilingDust.time = 0;
            }
            intensity += .05f;
            yield return new WaitForSeconds(timeBetweenShake);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.GetComponent<PlayerStatsController>().hasAuthority) {
            PlayerCamera cam = SplitscreenManager.instance.GetEntityCamera(other.gameObject);
            camerasInRoom.Add(cam);
            if (currentDestructionState == DestructionState.Critical) {
                cam.SetShake(1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 1f);
            } else {
                cam.SetShake(0f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.GetComponent<PlayerStatsController>().hasAuthority) {
            PlayerCamera cam = SplitscreenManager.instance.GetEntityCamera(other.gameObject);
            camerasInRoom.Remove(cam);
        }
    }

    private void Update() {
        if (currentDestructionState == DestructionState.Danger) {
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.SetShake(shakeTime, 1f);
            }
            shakeTime += 0.05f;
        }
    }
}
