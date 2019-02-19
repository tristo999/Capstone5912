using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrandonMove : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody rb;
    public float speed;
    BrandonPlayerHealth playerHealth;



    void Start()
    {

        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<BrandonPlayerHealth>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(movement * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        BrandonEnemyHealth health;

        if (health = other.GetComponent<BrandonEnemyHealth>())
        {
            health.ChangeHealth(-6);
            playerHealth.ChangeHealth(-4);
        }
    }


}
