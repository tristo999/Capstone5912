using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : HeldPassive
{
    public override void OnEquip() {
        int stat = Random.Range(0, 4);
        
        switch(stat){
            case 0:
                Owner.state.Speed += 5f;
                break;
            case 1:
                Owner.state.FireRate += 5f;
                break;
            case 2:
                Owner.state.ProjectileSpeed += 5f;
                break;
            case 3:
                Owner.state.ProjectileDamage += 5f;
                break;
            default:
                Owner.state.Health += 5f;
                break;
        }
        
        base.OnEquip();
    }
}
