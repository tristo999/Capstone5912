using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PongPlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void MovePaddle(float mod)
    {
        rigid.MovePosition(transform.position + Vector3.forward * moveSpeed * mod);
    }
}
