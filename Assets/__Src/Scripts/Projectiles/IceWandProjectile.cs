using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWandProjectile : Bolt.EntityBehaviour<IProjectileState>
{
    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
    }
}
