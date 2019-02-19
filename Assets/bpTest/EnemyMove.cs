using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator anim;
    int speedHash;
    Vector3 position;


    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
    }



    // Update is called once per frame
    void FixedUpdate()
    {


        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        anim.SetFloat(speedHash, moveHorizontal);


        gameObject.transform.Translate(Vector3.forward * Time.deltaTime * moveHorizontal * 5f);

    }
    private void OnTriggerEnter(Collider other)
    {
       
    }

    public void Attack()
    {

    }

    public void TurnAround() {
        


    }

}
