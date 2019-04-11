using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDebuff : Debuff
{
    public override string Name { get; set; }

    public override float EffectLength { get; set; }

    public float healthMod { get; set; }

    public float speedMod { get; set; }

    public float projectileMod { get; set; }

    public float firerateMod { get; set; }

    public float damageMod { get; set; }

    public override void OnGiven(IPlayerState Owner)
    {
        if (!FloatRoughlyZero(healthMod))
        {
            Owner.Health += healthMod;
        }
        if (!FloatRoughlyZero(speedMod)) Owner.Speed += speedMod;
        if (!FloatRoughlyZero(firerateMod)) Owner.FireRate += firerateMod;
        if (!FloatRoughlyZero(projectileMod)) Owner.ProjectileSpeed += projectileMod;
        if (!FloatRoughlyZero(damageMod)) Owner.ProjectileDamage += damageMod;
    }

    public  override void OnRemoved(IPlayerState Owner)
    {
        if (!FloatRoughlyZero(healthMod))
        {
            Owner.Health -= healthMod;
        }
        if (!FloatRoughlyZero(speedMod)) Owner.Speed -= speedMod;
        if (!FloatRoughlyZero(firerateMod)) Owner.FireRate -= firerateMod;
        if (!FloatRoughlyZero(projectileMod)) Owner.ProjectileSpeed -= projectileMod;
        if (!FloatRoughlyZero(damageMod)) Owner.ProjectileDamage -= damageMod;
    }

    public override void OnUpdate(IPlayerState playerState)
    {

    }

    private bool FloatRoughlyZero(float val)
    {
        return Math.Abs(val) < 0.0001f;
    }
}
