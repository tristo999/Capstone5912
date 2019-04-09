using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUses : MonoBehaviour 
{
    public int Uses;
    private int amountUsed = 0;

    void Start() {
        UpdatePlayerUI();
    }

    public void Use() {
        amountUsed++;
        if (amountUsed >= Uses) {
            GetComponentInParent<PlayerInventoryController>().state.OnDestroyWeapon();
        } else {
            UpdatePlayerUI();
        }
    }

    private void UpdatePlayerUI() {
        GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.SetWeaponUsesRemaining(Uses - amountUsed);
    }

}
