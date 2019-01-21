using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PongPlayerController))]
public class PlayerController : MonoBehaviour
{
    PongPlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PongPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.MovePaddle(Input.GetAxis("Vertical"));
    }
}
