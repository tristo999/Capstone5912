﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : NetworkBehaviour
{
    public bool debug = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (isServer)
            DoRotation();
    }

    void DoRotation(){
         transform.Rotate(new Vector3(transform.rotation.x, Random.Range(0, 359), transform.rotation.z));
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if(debug){
            if (Input.GetMouseButtonDown(0)) DoRotation();
        }
    }
    */
}
