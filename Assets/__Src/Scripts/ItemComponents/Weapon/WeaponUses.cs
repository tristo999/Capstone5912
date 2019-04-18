using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUses : MonoBehaviour 
{
    public int Uses;
    public float DestroyDelay;

    private Weapon weapon;
    public int AmountUsed {
        get {
            return amountUsed;
        }
        set {
            amountUsed = value;

            if (AmountUsed >= Uses || DestroyDelay > 0) {
                UpdatePlayerUI();
            }

            if (AmountUsed >= Uses) {
                StartCoroutine(DelayedDestroy(DestroyDelay));
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
        if (weapon.Owner.entity.isOwner) AmountUsed++;
    }

    private void UpdatePlayerUI() {
        if (weapon && weapon.Owner.entity.isOwner) weapon.Owner.GetComponent<PlayerStatsController>().ui.SetWeaponUsesRemaining(Uses - AmountUsed);
    }

    IEnumerator DelayedDestroy(float time) {
        yield return new WaitForSeconds(time);
        if (Uses > 1) {
            weapon.Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon exhausted!", GetComponent<Weapon>().Owner.transform.position);
        }
        GetComponentInParent<PlayerInventoryController>().state.OnDestroyWeapon();
    }

}
