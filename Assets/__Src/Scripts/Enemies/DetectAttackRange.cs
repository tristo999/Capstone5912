using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAttackRange : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) return;
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hasAuthority) return;
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inAttackRange = false;
        }
    }
}
