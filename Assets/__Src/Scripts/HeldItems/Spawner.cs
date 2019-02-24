using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public string folder;
    private GameObject spawned;
    public bool debug = true;

    GameObject[] LoadFromFolder(string folder){
        GameObject[] objs = Resources.LoadAll<GameObject>(folder);
        return objs;
    }
    
    GameObject SpawnObject(GameObject[] prefabs){
        Debug.LogFormat("Attempting to spawn from {0}", folder);
        Transform pos = GetComponent<Transform>();
        GameObject toSpawn = prefabs[Random.Range(0, prefabs.Length)];
        if (toSpawn.GetComponent<BoltEntity>())
            return BoltNetwork.Instantiate(toSpawn, transform.position, Quaternion.identity);
        else
            return Instantiate(toSpawn, transform.position, Quaternion.identity);
    }

    void Awake(){
        if (BoltNetwork.IsServer)
            spawned = SpawnObject(LoadFromFolder(folder));
    }

    /*
    private void Update() {
        if(debug){
            if (Input.GetMouseButtonDown(0))
            DemoRespawn();
        }
    }*/

    private void DemoRespawn() {
        Destroy(spawned);
        spawned = SpawnObject(LoadFromFolder(folder));
    }

}
