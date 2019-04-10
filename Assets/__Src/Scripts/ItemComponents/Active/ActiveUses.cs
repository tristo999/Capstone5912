using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUses : MonoBehaviour 
{
    public int Uses;
    public int AmountUsed { get; set; } = 0;

    void Start() {
        UpdatePlayerUI();
    }

    public void Use() {
        AmountUsed++;
        if (AmountUsed >= Uses) {
            if (Uses > 1) {
                GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Active exhausted!", GetComponent<ActiveItem>().Owner.transform.position);
            }

            GetComponentInParent<PlayerInventoryController>().state.OnDestroyActive();
        } else {
            UpdatePlayerUI();
        }
    }

    private void UpdatePlayerUI() {
        GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.SetActiveItemUsesRemaining(Uses - AmountUsed);
    }

}
