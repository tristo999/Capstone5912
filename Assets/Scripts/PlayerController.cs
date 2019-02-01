using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed;
    private Plane aimPlane;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10f;
        aimPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);

        //mouse loook
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (aimPlane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            target.y = gameObject.transform.position.y;
            gameObject.transform.LookAt(target);
        }

        //friction
        rb.velocity = rb.velocity * 0.98f;
    }
}
