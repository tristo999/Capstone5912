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
    public GameObject carpet;
    [Range(0.0f,1.0f)]
    public float chanceDestroyCarpet;

    public override void Attached() {
        if (entity.isOwner) {
            if (Random.Range(0.0f,1.0f) < chanceDestroyCarpet) {
                state.Carpet = false;
            } else {
                state.Carpet = true;
                state.CarpetColor = Random.ColorHSV(0f, 1f, .6f, 1f, .6f, 1f);
            }
        }

        state.NorthWall = true;
        state.EastWall = true;
        state.SouthWall = true;
        state.WestWall = true;

        state.AddCallback("NorthWall", NorthWallChange);
        state.AddCallback("SouthWall", SouthWallChange);
        state.AddCallback("EastWall", EastWallChange);
        state.AddCallback("WestWall", WestWallChange);
        state.AddCallback("Carpet", CarpetChange);
        state.AddCallback("CarpetColor", CarpetColorChange);
    }

    private void NorthWallChange() {
        Debug.Log("Wall Changed: " + state.NorthWall);
        northWall.SetActive(state.NorthWall);
    }

    private void EastWallChange() {
        Debug.Log("Wall Changed: " + state.EastWall);

        eastWall.SetActive(state.NorthWall);
    }

    private void SouthWallChange() {
        Debug.Log("Wall Changed: " + state.SouthWall);

        southWall.SetActive(state.NorthWall);
    }

    private void WestWallChange() {
        Debug.Log("Wall Changed: " + state.WestWall);

        westWall.SetActive(state.NorthWall);
    }

    private void CarpetChange() {
        carpet.SetActive(state.Carpet);
    }

    private void CarpetColorChange() {
        carpet.GetComponent<Renderer>().material.color = state.CarpetColor;
    }
}
