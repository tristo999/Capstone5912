using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExperimentalPotion : HeldPassive
{

    public override void OnEquip() {        

        Owner.GetComponent<PlayerStatsController>().Speed += Random.Range(-3, 3) / 10f;
        Owner.GetComponent<PlayerStatsController>().FireRate += Random.Range(-3, 3) / 10f;
        Owner.GetComponent<PlayerStatsController>().ProjectileSpeed += Random.Range(-3, 3) / 10f;
        Owner.GetComponent<PlayerStatsController>().ProjectileDamage += Random.Range(-3, 3) / 10f;

        base.OnEquip();
    }
}

