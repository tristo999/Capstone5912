using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponCooldown))]
[RequireComponent(typeof(WeaponLaunchProjectile))]
public class FireballWand : Weapon
{
    public float BaseLaunchVelocity;
    public float MaxLaunchVelocity;
    public int PointsInArc;

    private float currentVelocity;
    // private LineRenderer line; // Disabled for now.

    private bool beganFiring;

    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProjectile;
    private WeaponUses uses;

    private Transform ownerLaunchPos;

    private void Awake() {
        cooldown = GetComponent<WeaponCooldown>();
        launchProjectile = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireDown() {
        currentVelocity = BaseLaunchVelocity * Owner.state.ProjectileSpeed;

        // Need to late instantiate this so that Owner is available.
        if (ownerLaunchPos == null) {
            ownerLaunchPos = Owner.GetComponent<PlayerInventoryController>().launchPos;
        }
    }

    public override void FireHold() {
        if (!cooldown.Ready) return;
        if (!beganFiring) {
            Owner.state.Speed -= 0.66f;
            beganFiring = true;
        }
        if (currentVelocity < MaxLaunchVelocity * Owner.state.ProjectileSpeed) currentVelocity += 60f * Owner.state.ProjectileSpeed * Time.deltaTime;

        /* 
        Vector3[] positions = new Vector3[PointsInArc];
        float timeToImpact = TimeOfImpact(launchProjectile.LocalLaunchDir * launchProjectile.LaunchForce);
        float step = timeToImpact / PointsInArc;
        for (int i = 0; i < PointsInArc; i++) {
            positions[i] = ownerLaunchPos.position + launchProjectile.LocalLaunchDir * launchProjectile.LaunchForce * i * step + Physics.gravity * i * i * step * step * .5f;
        }

        line.SetPositions(positions);
        */
    }

    public override void FireRelease() {
        if (!cooldown.Ready) return;
        if (beganFiring) {
            // Sometimes firerelease gets called twice so we need to check to make sure the speed up is not applied twice.
            Owner.state.Speed += 0.66f;
        }
        cooldown.ResetCooldown();
        beganFiring = false;
        // line.SetPositions(new Vector3[PointsInArc]);
        if (Owner.entity.isOwner) {
            launchProjectile.LaunchForce = currentVelocity;
            launchProjectile.Launch();
            uses.Use();
            Owner.state.FireAnim();
        }
    }

    public override void OnEquip() {
        // line = GetComponent<LineRenderer>();
        // line.positionCount = PointsInArc;
    }

    public override void OnDequip() {
        if (beganFiring) {
            Owner.state.Speed -= 0.66f;
        }
    }

    private float TimeOfImpact(Vector3 dir) {
        float time = 0f;
        bool collided = false;

        while (!collided) {
            time += .025f;
            if (!ownerLaunchPos) {
                ownerLaunchPos = Owner.GetComponent<PlayerInventoryController>().launchPos;
            }
            Vector3 pos = ownerLaunchPos.position + dir * time + Physics.gravity * time * time * .5f;
            collided = Physics.CheckSphere(pos, .2f, ~(1 << 12)) || time > 1000;
        }

        return time;
    }
}
