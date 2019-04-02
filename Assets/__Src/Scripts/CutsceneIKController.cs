using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CutsceneIKController : MonoBehaviour
{
    Animator animator;

    public bool ikActive = false;
    public Transform lookTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex) {
        if (ikActive) {
            if (lookTarget) {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(lookTarget.position);
            }
        } else {
            animator.SetLookAtWeight(0);
        }
    }
}
