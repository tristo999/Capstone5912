using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public string folder;
    private GameObject spawned;
    public bool debug = true;

    Object[] LoadFromFolder(string folder){
        Object[] objs = Resources.LoadAll(folder);
        /*
        foreach(Object o in objs){
            Debug.Log(o.name);
        }
        */
        return objs;
    }
    
    GameObject SpawnObject(Object[] prefabs){
        Transform pos = GetComponent<Transform>();
        return Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform) as GameObject;
    }

    void Awake(){
        spawned = SpawnObject(LoadFromFolder(folder));
    }

    private void Update() {
        if(debug){
            if (Input.GetMouseButtonDown(0))
            DemoRespawn();
        }
    }

    private void DemoRespawn() {
        Destroy(spawned);
        spawned = SpawnObject(LoadFromFolder(folder));
    }

}
