using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : HeldPassive {
    public override void OnEquip() {
        Weapon wep = Owner.GetComponent<PlayerInventoryController>().wizardWeapon;
        if (wep) {
            WeaponUses wepUses = wep.GetComponent<WeaponUses>();
            if (wepUses && wepUses.AmountUsed > 0) {
                wepUses.AmountUsed = 0;
                Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon uses restored!", Owner.transform.position);
            }
        }

        ActiveItem active = Owner.GetComponent<PlayerInventoryController>().activeItem;
        if (active) {
            ActiveUses activeUses = active.GetComponent<ActiveUses>();
            if (activeUses && activeUses.AmountUsed > 0) {
                activeUses.AmountUsed = 0;
                Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Active uses restored!", Owner.transform.position);
            }
        }

        base.OnEquip();
    }
}
