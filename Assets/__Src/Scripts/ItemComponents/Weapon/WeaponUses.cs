using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUses : MonoBehaviour 
{
    public int Uses;
    private Weapon weapon;
    public int AmountUsed {
        get {
            return amountUsed;
        }
        set {
            amountUsed = value;

            if (AmountUsed >= Uses) {
                if (Uses > 1) {
                    weapon.Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon exhausted!", GetComponent<Weapon>().Owner.transform.position);
                }

                GetComponentInParent<PlayerInventoryController>().state.OnDestroyWeapon();
            } else {
                UpdatePlayerUI();
            }
        }
    }
    private int amountUsed = 0;


    void Start() {
        weapon = GetComponent<Weapon>();
        if (weapon.Owner.entity.isOwner) {
            UpdatePlayerUI();
        }
    }

    public void Use() {
        if (weapon.Owner.entity.isOwner)
            AmountUsed++;
    }

    private void UpdatePlayerUI() {
        if (weapon && weapon.Owner.entity.isOwner)
            GetComponent<Weapon>().Owner.GetComponent<PlayerStatsController>().ui.SetWeaponUsesRemaining(Uses - AmountUsed);
    }

}
