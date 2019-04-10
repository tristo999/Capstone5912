using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUses : MonoBehaviour 
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
                GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon exhausted!", GetComponent<Weapon>().Owner.transform.position);
            }

            GetComponentInParent<PlayerInventoryController>().state.OnDestroyWeapon();
        } else {
            UpdatePlayerUI();
        }
    }

    private void UpdatePlayerUI() {
        GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.SetWeaponUsesRemaining(Uses - AmountUsed);
    }

}
