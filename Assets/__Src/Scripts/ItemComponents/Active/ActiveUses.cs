using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUses : MonoBehaviour 
{
    public int Uses;
    public float DestroyDelay;

    private ActiveItem active;
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
        active = GetComponent<ActiveItem>();
        if (active.Owner.entity.isOwner) {
            UpdatePlayerUI();
        }
    }

    public void Use() {
        if (active.Owner.entity.isOwner) AmountUsed++;
    }

    private void UpdatePlayerUI() {
        if (active && active.Owner.entity.isOwner) GetComponent<ActiveItem>().Owner.GetComponent<PlayerStatsController>().ui.SetActiveItemUsesRemaining(Uses - AmountUsed);
    }

    IEnumerator DelayedDestroy(float time) {
        yield return new WaitForSeconds(time);
        if (Uses > 1) {
            active.Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Active exhausted!", GetComponent<ActiveItem>().Owner.transform.position);
        }
        GetComponentInParent<PlayerInventoryController>().state.OnDestroyActive();
    }

}
