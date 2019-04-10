using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUses : MonoBehaviour 
{
    public int Uses;
    public int AmountUsed {
        get {
            return amountUsed;
        }
        set {
            amountUsed = value;

            if (AmountUsed >= Uses) {
                if (Uses > 1) {
                    GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon exhausted!", GetComponent<Weapon>().Owner.transform.position);
                }

                GetComponentInParent<PlayerInventoryController>().state.OnDestroyWeapon();
            } else {
                UpdatePlayerUI();
            }
        }
    }
    private int amountUsed = 0;

    void Start() {
        UpdatePlayerUI();
    }

    public void Use() {
        AmountUsed++;
    }

    private void UpdatePlayerUI() {
        GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.SetWeaponUsesRemaining(Uses - AmountUsed);
    }

}
