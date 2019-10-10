using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffHandler : NetworkBehaviour
{
    List<Debuff> playerDebuffs = new List<Debuff>();
    PlayerUI ui;

    public void Awake() {
        ui = GetComponent<PlayerUI>();
    }

    public void GrantDebuff(Debuff debuff) {
        ui.AddFloatingMessageText("Got Debuff: " + debuff.Name, transform.position);
        playerDebuffs.Add(debuff);
        debuff.OnGiven(GetComponent<PlayerStatsController>());
        debuff.GrantedTime = (float)NetworkTime.time;
    }

    public void RemoveDebuff(Debuff debuff) {
        ui.AddFloatingMessageText("Debuff Removed: " + debuff.Name, transform.position);
        playerDebuffs.Remove(debuff);
        debuff.OnRemoved(GetComponent<PlayerStatsController>());
    }

    public void Update() {
        if (!hasAuthority) return;
        foreach (Debuff debuff in playerDebuffs) {
            if (NetworkTime.time - debuff.GrantedTime > debuff.EffectLength) {
                RemoveDebuff(debuff);
            } else {
                debuff.OnUpdate(GetComponent<PlayerStatsController>());
            }
        }
    }
}
