using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : Bolt.EntityBehaviour<IDungeonRoom>
{
    public int DistanceFromCenter = -1;
    public GameObject northWall;
    public GameObject eastWall;
    public GameObject southWall;
    public GameObject westWall;
    public GameObject northWallFlat;
    public GameObject eastWallFlat;
    public GameObject southWallFlat;
    public GameObject westWallFlat;
    public GameObject northWallDestroyed;
    public GameObject eastWallDestroyed;
    public GameObject southWallDestroyed;
    public GameObject westWallDestroyed;
    public GameObject carpet;
    [Range(0.0f,1.0f)]
    public float chanceDestroyCarpet;

    public enum WallState { Door=1, Open=2, Closed=0, Destroyed=3 }

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
    }

    private void NorthWallChange() {
        WallState wallState = (WallState)state.NorthWall;
        Debug.LogFormat("Set north wall to: {0}", wallState.ToString());
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
        Debug.LogFormat("Set east wall to: {0}", wallState.ToString());
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
        Debug.LogFormat("Set south wall to: {0}", wallState.ToString());
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
        Debug.LogFormat("Set west wall to: {0}", wallState.ToString());
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

    private void CarpetChange() {
        carpet.SetActive(state.Carpet);
    }

    private void CarpetColorChange() {
        carpet.GetComponent<Renderer>().material.color = state.CarpetColor;
    }
}
