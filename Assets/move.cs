using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody rb;
    public float speed;
    playerHealth playerHealth;



    void Start()
    {

        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<playerHealth>();
        
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
        enemy_stats health;

        if (health = other.GetComponent<enemy_stats>())
        {
            health.ChangeHealth(-6);
            playerHealth.ChangeHealth(-4);
        }
    }


}
