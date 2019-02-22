using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackRange : Bolt.EntityBehaviour<IEnemyState>
{
    private void OnTriggerEnter(Collider other)
    {
        if (entity.isAttached && !entity.isOwner) return;
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (entity.isAttached && !entity.isOwner) return;
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inAttackRange = false;
        }
    }
}
