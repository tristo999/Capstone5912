using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWand : Weapon
{
    public GameObject projectile;
    public float launchVelocity;

    public float FireTime = .5f;
    private float timer = 0.0f;

    public override void FireDown() {
        
    }

    public override void FireHold() {
        if (timer < 0f) {
            Vector3 spawnPos = transform.position + transform.forward * .3f;
            spawnPos.y += .8f;
            BoltEntity proj = BoltNetwork.Instantiate(projectile, spawnPos, Quaternion.identity);
            proj.GetComponent<BasicWandProjectile>().owner = Owner.gameObject;
            proj.GetComponent<Rigidbody>().velocity = transform.forward * launchVelocity * Owner.state.ProjectileSpeed + Owner.GetComponent<Rigidbody>().velocity * .8f;
            timer = FireTime * Owner.state.FireRate;
        } 
    }

    public override void FireRelease() {

    }

    public override void OnEquip() {

    }

    private void Update() {
        timer -= Time.deltaTime;
    }
}
