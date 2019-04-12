using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : Bolt.EntityBehaviour<IDungeonRoom>
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
    public ParticleSystem ceilingDust;
    private List<PlayerCamera> camerasInRoom = new List<PlayerCamera>();
    private float shakeTime = 1f;

    public enum WallState { Door=1, Open=2, Closed=0, Destroyed=3 }
    public enum DestructionState { Normal, Warning, Danger, Critical, Destroyed }

    public override void Attached() {
        if (entity.isOwner) {
            if (Random.Range(0.0f,1.0f) < chanceDestroyCarpet) {
                state.Carpet = false;
            } else {
                state.Carpet = true;
                state.CarpetColor = Random.ColorHSV(0f, 1f, .6f, 1f, .6f, 1f);
            }
        }

        state.AddCallback("NorthWall", NorthWallChange);
        state.AddCallback("SouthWall", SouthWallChange);
        state.AddCallback("EastWall", EastWallChange);
        state.AddCallback("WestWall", WestWallChange);
        state.AddCallback("Carpet", CarpetChange);
        state.AddCallback("CarpetColor", CarpetColorChange);
        state.AddCallback("DestructionState", DestructionStateChange);
    }

    private void NorthWallChange() {
        WallState wallState = (WallState)state.NorthWall;
        northWall.SetActive(false);
        northWallFlat.SetActive(false);
        northWallDestroyed.SetActive(false);
        if (wallState == WallState.Open) {
            northWall.SetActive(true);
        } else if (wallState == WallState.Closed) {
            northWallFlat.SetActive(true);
        } else if (wallState == WallState.Destroyed) {
            northWallDestroyed.SetActive(true);
        }
    }

    private void EastWallChange() {
        WallState wallState = (WallState)state.EastWall;
        eastWall.SetActive(false);
        eastWallFlat.SetActive(false);
        eastWallDestroyed.SetActive(false);
        if (wallState == WallState.Open) {
            eastWall.SetActive(true);
        } else if (wallState == WallState.Closed) {
            eastWallFlat.SetActive(true);
        } else if (wallState == WallState.Destroyed) {
            eastWallDestroyed.SetActive(true);
        }
    }

    private void SouthWallChange() {
        WallState wallState = (WallState)state.SouthWall;
        southWall.SetActive(false);
        southWallFlat.SetActive(false);
        southWallDestroyed.SetActive(false);
        if (wallState == WallState.Open) {
            southWall.SetActive(true);
        } else if (wallState == WallState.Closed) {
            southWallFlat.SetActive(true);
        } else if (wallState == WallState.Destroyed) {
            southWallDestroyed.SetActive(true);
        }
    }

    private void WestWallChange() {
        WallState wallState = (WallState)state.WestWall;
        westWall.SetActive(false);
        westWallFlat.SetActive(false);
        westWallDestroyed.SetActive(false);
        if (wallState == WallState.Open) {
            westWall.SetActive(true);
        } else if (wallState == WallState.Closed) {
            westWallFlat.SetActive(true);
        } else if (wallState == WallState.Destroyed) {
            westWallDestroyed.SetActive(true);
        }
    }

    private void DestructionStateChange() {
        if (state.DestructionState == (int)DestructionState.Warning) {
            StartCoroutine(warnShake());
        }

        if (state.DestructionState == (int)DestructionState.Critical) {
            ceilingDust.Play();
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.SetShake(1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(0, 1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 1f);
            }
        }

        if (state.DestructionState == (int)DestructionState.Destroyed) {
            if (state.NorthWall != (int)WallState.Closed)
                state.NorthWall = (int)WallState.Destroyed;
            if (state.EastWall != (int)WallState.Closed)
                state.EastWall = (int)WallState.Destroyed;
            if (state.WestWall != (int)WallState.Closed)
                state.WestWall = (int)WallState.Destroyed;
            if (state.SouthWall != (int)WallState.Closed)
                state.SouthWall = (int)WallState.Destroyed;
            ceilingDust.Stop();
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.CameraPlayer.GetState<IPlayerState>().Dead = true;
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
        

        while (state.DestructionState == (int)DestructionState.Warning || state.DestructionState == (int) DestructionState.Danger) {
            if (state.DestructionState == (int) DestructionState.Danger) {
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
            if (state.DestructionState != (int)DestructionState.Critical) {
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
        if (other.tag == "Player" && other.GetComponent<BoltEntity>().isOwner) {
            PlayerCamera cam = SplitscreenManager.instance.GetEntityCamera(other.GetComponent<BoltEntity>());
            camerasInRoom.Add(cam);
            if (state.DestructionState == (int)DestructionState.Critical) {
                cam.SetShake(1f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 1f);
            } else {
                cam.SetShake(0f);
                cam.CameraPlayer.GetComponent<PlayerMovementController>().localPlayer.SetVibration(1, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.GetComponent<BoltEntity>().isOwner) {
            PlayerCamera cam = SplitscreenManager.instance.GetEntityCamera(other.GetComponent<BoltEntity>());
            camerasInRoom.Remove(cam);
        }
    }

    private void Update() {
        if (state.DestructionState == (int)DestructionState.Danger) {
            foreach (PlayerCamera cam in camerasInRoom) {
                cam.SetShake(shakeTime, 1f);
            }
            shakeTime += 0.05f;
        }
    }

    private void CarpetChange() {
        //carpet.SetActive(state.Carpet);
    }

    private void CarpetColorChange() {
        //carpet.GetComponent<Renderer>().material.color = state.CarpetColor;
    }
}
