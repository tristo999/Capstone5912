using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PongPlayerController))]
public class AIControl : MonoBehaviour
{
    private PongPlayerController controller;

    public Transform ball;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PongPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float dir = ball.position.z - (transform.position.z - 1);
        if (dir > 1.0f) dir = 1.0f;
        if (dir < -1.0f) dir = -1.0f;

        controller.MovePaddle(dir);
    }
}
