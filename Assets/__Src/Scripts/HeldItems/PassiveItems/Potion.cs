using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : HeldPassive
{
    public override void OnEquip() {
        int stat = Random.Range(0, 5);
        
        switch(stat){
            case 0:
                Owner.state.Speed += 0.1f;
                break;
            case 1:
                Owner.state.FireRate += 0.25f;
                break;
            case 2:
                Owner.state.ProjectileSpeed += 0.3f;
                break;
            case 3:
                Owner.state.ProjectileDamage += 0.15f;
                break;
            case 4:
                Owner.state.Health += 5f;
                break;
            default:
                Owner.state.Health += 5f;
                break;
        }
        
        base.OnEquip();
    }
}
