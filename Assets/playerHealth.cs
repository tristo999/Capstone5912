using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public Slider healthFill;

    public float currentHealth;
    public float maxHealth;



    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        //make health value stay between 0 and max
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //percentage for slider fill
        healthFill.value = currentHealth / maxHealth;
    }


}
