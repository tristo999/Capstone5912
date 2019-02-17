using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHitRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inHitRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<BasicEnemyAI>().inHitRange = false;
        }
    }
}
