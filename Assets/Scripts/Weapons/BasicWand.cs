using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWand : WizardWeapon
{
    public GameObject projectile;
    public float launchVelocity;

    public override void FireDown() {
        Vector3 spawnPos = transform.position + transform.forward * .3f;
        spawnPos.y += .8f;
        BoltEntity proj = BoltNetwork.Instantiate(projectile, spawnPos, Quaternion.identity);
        proj.GetComponent<DefaultWizardProjectile>().owner = transform.parent.gameObject;
        proj.GetComponent<Rigidbody>().velocity = transform.forward * launchVelocity;
    }

    public override void FireHold() {

    }

    public override void FireRelease() {

    }

    public override void OnEquip() {
        
    }
}
