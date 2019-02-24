using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPhysicsObject : Bolt.EntityBehaviour<IRoomObject>
{
    public override void Attached() {
        state.SetTransforms(state.transform, transform);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
