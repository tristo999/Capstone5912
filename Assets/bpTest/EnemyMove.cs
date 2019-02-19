using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator anim;
    int speedHash, turnHash, comboHash, numClicks;
    Vector3 position, forward = Vector3.forward;
    public float speed;
    bool canAttack = true;


    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
        turnHash = Animator.StringToHash("Turn");
        comboHash = Animator.StringToHash("ComboNum");

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }


        float rotateY = Input.GetAxis("Horizontal") * speed;

        float moveVertical = Input.GetAxis("Vertical");

        if (moveVertical < 0)
            moveVertical *= -1;


        anim.SetFloat(speedHash, moveVertical);

        transform.eulerAngles += new Vector3(0.0f, rotateY, 0.0f);
        
        gameObject.transform.Translate(forward *Time.deltaTime * moveVertical * speed);

    }
    private void OnTriggerEnter(Collider other)
    {
       
    }

    public void Attack()
    {
       

        if (canAttack)
        {
            numClicks++;
            anim.SetInteger(comboHash, numClicks);

        }

        if (numClicks >= 3)
            numClicks = 0;
    }

   

  

    private IEnumerator playAnimation(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
