using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackEnabledEntity : Bolt.EntityEventListener
{
    private Rigidbody rb;
    public override void Attached() {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEvent(KnockbackEntity evnt) {
        rb.AddForce(evnt.Force);
    }
}
