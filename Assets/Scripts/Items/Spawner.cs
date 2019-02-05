using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public string folder;

    Object[] LoadFromFolder(string folder){
        Object[] objs = Resources.LoadAll(folder);
        /*
        foreach(Object o in objs){
            Debug.Log(o.name);
        }
        */
        return objs;
    }
    
    Object SpawnObject(Object[] prefabs){
        Transform pos = GetComponent<Transform>();
        return Instantiate(prefabs[Random.Range(0, prefabs.Length)], GetComponent<Transform>());
    }

    void Awake(){
        Object obj = SpawnObject(LoadFromFolder(folder));
        
    }

}
