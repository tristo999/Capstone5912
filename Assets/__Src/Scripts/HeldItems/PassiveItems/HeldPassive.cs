using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPassive : HeldItem {
    protected ItemDefinition item;

    public virtual void OnEquip() {
        GameObject obj = GetComponent<GameObject>();
        item = ItemManager.Instance.items[Id];

        if (item.HealthModifier > 0) {
            Owner.state.Health += item.HealthModifier;
            Owner.GetComponent<PlayerStatsController>().ui.AddDamageText(-item.HealthModifier, Owner.transform.position);
        }
        if (!FloatRoughlyZero(item.SpeedModifier)) Owner.state.Speed += item.SpeedModifier;
        if (!FloatRoughlyZero(item.FireRateModifier)) Owner.state.FireRate += item.FireRateModifier;
        if (!FloatRoughlyZero(item.ProjectileSpeedModifier)) Owner.state.ProjectileSpeed += item.ProjectileSpeedModifier;
        if (!FloatRoughlyZero(item.DamageModifier)) Owner.state.ProjectileDamage += item.DamageModifier;

        Destroy(obj);
    }

    private bool FloatRoughlyZero(float val) {
        return Math.Abs(val) < 0.0001f;
    }
}
