using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldPassive : HeldItem {
    protected ItemDefinition item;

    public virtual void OnEquip() {
        item = ItemManager.Instance.items[Id];

        if (!FloatRoughlyZero(item.HealthModifier)) {
            Owner.state.Health += item.HealthModifier;
            Owner.GetComponent<PlayerStatsController>().ui.AddDamageText(-item.HealthModifier, Owner.transform.position, true);
        }
        if (!FloatRoughlyZero(item.SpeedModifier)) Owner.state.Speed += item.SpeedModifier;
        if (!FloatRoughlyZero(item.FireRateModifier)) Owner.state.FireRate += item.FireRateModifier;
        if (!FloatRoughlyZero(item.ProjectileSpeedModifier)) Owner.state.ProjectileSpeed += item.ProjectileSpeedModifier;
        if (!FloatRoughlyZero(item.DamageModifier)) Owner.state.ProjectileDamage += item.DamageModifier;

        Destroy(gameObject, 1.0f);
    }

    private bool FloatRoughlyZero(float val) {
        return Math.Abs(val) < 0.0001f;
    }
}
