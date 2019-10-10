using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : HeldPassive
{

    public override void OnEquip() {

        int stat = Random.Range(0, 5);
        
        switch(stat){
            case 0:
                Owner.GetComponent<PlayerStatsController>().Speed += 0.1f;
                break;
            case 1:
                Owner.GetComponent<PlayerStatsController>().FireRate += 0.2f;
                break;
            case 2:
                Owner.GetComponent<PlayerStatsController>().ProjectileSpeed += 0.2f;
                break;
            case 3:
                Owner.GetComponent<PlayerStatsController>().ProjectileDamage += 0.15f;
                break;
            case 4:
                Owner.GetComponent<PlayerStatsController>().Health += 15f;
                Owner.GetComponent<PlayerStatsController>().ui.AddDamageText(-5f, Owner.transform.position, true);
                break;
            default:
                Owner.GetComponent<PlayerStatsController>().Health += 15f;
                Owner.GetComponent<PlayerStatsController>().ui.AddDamageText(-5f, Owner.transform.position, true);
                break;
        }

        base.OnEquip();
    }
}
