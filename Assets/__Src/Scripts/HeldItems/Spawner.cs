using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public string folder;
    private GameObject spawned;
    public bool debug = true;

    GameObject[] LoadFromFolder(string folder){
        GameObject[] objs = Resources.LoadAll<GameObject>(folder);
        return objs;
    }
    
    [Command]
    GameObject CmdSpawnObject(GameObject[] prefabs){
        Transform pos = GetComponent<Transform>();
        GameObject toSpawn = prefabs[Random.Range(0, prefabs.Length)];
        GameObject spawned = Instantiate(toSpawn, transform.position, transform.rotation);
        NetworkServer.Spawn(spawned);
        return spawned;
    }

    void Awake(){
        if (isServer)
            spawned = CmdSpawnObject(LoadFromFolder(folder));
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
        spawned = CmdSpawnObject(LoadFromFolder(folder));
    }

}
