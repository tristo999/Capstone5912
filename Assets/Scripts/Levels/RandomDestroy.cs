using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDestroy : Bolt.EntityBehaviour<IRoomObject>
{
    [Tooltip("Probability of object existing")]
    [Range(0, 100)]
    public int probability;

    public override void Attached()
    {
        if (entity.isOwner) {
            if (Random.Range(0, 100) > probability) {
                BoltNetwork.Destroy(entity);
            }
        }
    }
}
