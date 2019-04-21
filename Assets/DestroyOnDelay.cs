using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDelay : MonoBehaviour
{
    public void DestroyAfter(float delay) {
        StartCoroutine(DelayedDestroy(delay));
    }

    IEnumerator DelayedDestroy(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
