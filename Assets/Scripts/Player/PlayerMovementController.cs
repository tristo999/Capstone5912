using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityBehaviour<IPlayerState>
{
    private Rigidbody rb;
    private float speed = 10f;
    private Plane aimPlane = new Plane(Vector3.up, Vector3.zero);

    public override void Attached()
    {
        rb = GetComponent<Rigidbody>();
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);

        //mouse loook
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (aimPlane.Raycast(ray, out distance)) {
            Vector3 target = ray.GetPoint(distance);
            target.y = gameObject.transform.position.y;
            gameObject.transform.LookAt(target);
        }
    }
}
