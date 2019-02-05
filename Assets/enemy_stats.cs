using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy_stats : MonoBehaviour { 

    public Transform healthBar;
    public Slider healthFill;

    public float currentHealth;
    public float maxHealth;
    public float healthBarYOffset = 2;



    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        //make health value stay between 0 and max
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //percentage for slider fill
        healthFill.value = currentHealth / maxHealth;
    }

    //set pos of healthbar above parent object
    private void HealthBarPos()
    {
        Vector3 currentPos = transform.position;
        healthBar.position = new Vector3(currentPos.x, currentPos.y + healthBarYOffset, currentPos.z);

        //show health bar to camera
        healthBar.LookAt(Camera.main.transform);
    }

 
    // Update is called once per frame
    void Update()
    {
        HealthBarPos();   
    }
}
