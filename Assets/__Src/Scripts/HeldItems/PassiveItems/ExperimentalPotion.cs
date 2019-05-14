using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExperimentalPotion : HeldPassive
{

    public override void OnEquip() {        

        Owner.state.Speed += Random.Range(-3, 3) / 10f;
        Owner.state.FireRate += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileSpeed += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileDamage += Random.Range(-3, 3) / 10f;

        base.OnEquip();
    }
}

