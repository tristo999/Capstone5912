using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPassive : HeldItem
{
    private float speed;
    private float health;
    private float damage;

    public virtual void OnEquip() {
        GameObject obj = this.GetComponent<GameObject>();
        ItemDefinition item = ItemManager.Instance.items[this.Id];
        speed = item.SpeedModifier;
        health = item.HealthModifier;
        damage = item.DamageModifier;

        // modify state with all modifiers
        // speed
        if(speed > 0)
        {
            // timed speed mod
            Owner.state.Speed += speed;
            float time = 0;
            while(time < 10)
            {
                time += Time.deltaTime;
            }
            Owner.state.Speed -= speed;
        }

        // health
        if(health > 0)
        {
            // add to health
            Owner.state.Health += health;
        }

        // damage
        if(damage > 0)
        {
            // take damage
            Owner.state.Health -= damage;
        }

        //TODO add equip animation??

        // destroy current
        Destroy(obj);
    }
}
