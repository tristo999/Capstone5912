using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPassive : HeldItem
{
    private float speed;
    private float health;
    private float damage;

    public virtual void OnEquip() {
        GameObject obj = GetComponent<GameObject>();
        ItemDefinition item = ItemManager.Instance.items[Id];
        speed = item.SpeedModifier;
        health = item.HealthModifier;
        damage = item.DamageModifier;

        if(speed < -0.001 || speed > 0.001) { 
            Owner.state.Speed += speed;

            Owner.GetComponent<PlayerStatsController>().ui.AddStatText(
                $"{(speed >= 0 ? "+" : "-")}{speed * 100}% speed", Owner.transform.position);
        }

        if(health > 0) { 
            Owner.state.Health += health;

            Owner.GetComponent<PlayerStatsController>().ui.AddHealText(health, Owner.transform.position);
        }

        if(damage > 0) { 
            Owner.state.Health -= damage;

            Owner.GetComponent<PlayerStatsController>().ui.AddDamageText(damage, Owner.transform.position);
        }

        Debug.Log("CONSUMED");

        Destroy(obj);
    }
}
