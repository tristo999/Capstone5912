using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : ActiveItem
{
    public override void ActivateHold() {

    }

    public override void ActivateRelease() {

    }

    public override void ActiveDown() {
        int randomMessageId = Random.Range(0, 4);
        string randomMessage = "";

        if (randomMessageId == 0) {
            randomMessage = "Grossss!";
        } else if (randomMessageId == 1) {
            randomMessage = "Ewww! Don't play with it!";
        } else if (randomMessageId == 2) {
            randomMessage = "You're disgusting";
        } else if (randomMessageId == 3) {
            randomMessage = "Why did you try that";
        }

        Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText(randomMessage, Owner.transform.position);
    }

    public override void OnEquip() {
        Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Nooo don't pick that up!", Owner.transform.position);
    }

    public override void OnDequip() {
        Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("I need a shower...", Owner.transform.position);
    }
}
