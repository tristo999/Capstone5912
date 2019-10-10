using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPhysicsObject : NetworkBehaviour
{
    public void Awake() {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
