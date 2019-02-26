using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public bool debug = true;

    GameObject SpawnObject(GameObject[] prefabs){
        Transform pos = GetComponent<Transform>();
        GameObject toSpawn = GameObject.Find("WallLightSpawner");
        if (toSpawn.GetComponent<BoltEntity>())
            return BoltNetwork.Instantiate(toSpawn, transform.position, Quaternion.identity);
        else
            return Instantiate(toSpawn, transform.position, Quaternion.identity);
    }

}
