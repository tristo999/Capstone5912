using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffHandler : Bolt.EntityBehaviour<IPlayerState>
{
    List<Debuff> playerDebuffs = new List<Debuff>();
    PlayerUI ui;

    public override void Attached() {
        ui = GetComponent<PlayerUI>();
    }

    public void GrantDebuff(Debuff debuff) {
        ui.AddFloatingMessageText("Got Debuff: " + debuff.Name, transform.position);
        playerDebuffs.Add(debuff);
        debuff.OnGiven(state);
        debuff.GrantedTime = BoltNetwork.Time;
    }

    public void RemoveDebuff(Debuff debuff) {
        ui.AddFloatingMessageText("Debuff Removed: " + debuff.Name, transform.position);
        playerDebuffs.Remove(debuff);
        debuff.OnRemoved(state);
    }

    public override void SimulateOwner() {
        foreach (Debuff debuff in playerDebuffs) {
            if (BoltNetwork.Time - debuff.GrantedTime > debuff.EffectLength) {
                RemoveDebuff(debuff);
            } else {
                debuff.OnUpdate(state);
            }
        }
    }
}
