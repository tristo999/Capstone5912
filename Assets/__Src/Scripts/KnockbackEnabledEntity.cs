using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackEnabledEntity : NetworkBehaviour
{
    private Rigidbody rb;
    public void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    [Command]
    public void CmdAddKnockback(Vector3 force) {
        rb.AddForce(force);
    }
}
