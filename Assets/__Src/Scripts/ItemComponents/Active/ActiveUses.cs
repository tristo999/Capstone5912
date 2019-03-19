using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUses : MonoBehaviour 
{
    public int Uses;
    private int amountUsed = 0;

    public void Use() {
        amountUsed++;
        if (amountUsed > Uses) {
            GetComponentInParent<PlayerInventoryController>().state.OnDestroyActive();
        }
    }

}
