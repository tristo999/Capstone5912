using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUses : MonoBehaviour 
{
    public int Uses;
    private int amountUsed = 0;

    void Begin() {
        UpdatePlayerUI();
    }

    public void Use() {
        amountUsed++;
        if (amountUsed > Uses) {
            GetComponentInParent<PlayerInventoryController>().state.OnDestroyActive();
        } else {
            UpdatePlayerUI();
        }
    }

    private void UpdatePlayerUI() {
        GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.SetActiveItemUsesRemaining(Uses - amountUsed);
    }

}
