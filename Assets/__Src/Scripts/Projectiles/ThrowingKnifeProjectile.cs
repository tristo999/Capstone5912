using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnifeProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public override void Attached() {
        state.SetTransforms(state.transform, transform);
    }
}
