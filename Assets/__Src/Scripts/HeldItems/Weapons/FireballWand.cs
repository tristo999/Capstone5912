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
    private LineRenderer line;

    private bool beganFiring;

    private WeaponCooldown cooldown;
    private WeaponLaunchProjectile launchProjectile;

    private void Awake() {
        cooldown = GetComponent<WeaponCooldown>();
        launchProjectile = GetComponent<WeaponLaunchProjectile>();
    }

    public override void FireDown() {
        launchProjectile.LaunchForce = BaseLaunchVelocity;
    }

    public override void FireHold() {
        if (!cooldown.Ready) return;
        if (!beganFiring) {
            Owner.state.Speed -= 0.75f;
            beganFiring = true;
        }
        if (currentVelocity < MaxLaunchVelocity * Owner.state.ProjectileSpeed)
            launchProjectile.LaunchForce += 0.05f;
        Vector3[] positions = new Vector3[PointsInArc];
        float timeToImpact = TimeOfImpact(launchProjectile.LaunchPosition.forward);
        float step = timeToImpact / PointsInArc;
        for (int i = 0; i < PointsInArc; i++) {
            positions[i] = launchProjectile.LaunchPosition.position + launchProjectile.LaunchPosition.forward * i * step + Physics.gravity * i * i * step * step * .5f;
        }

        line.SetPositions(positions);
    }

    public override void FireRelease() {
        if (!cooldown.Ready) return;
        if (beganFiring) {
            // Sometimes firerelease gets called twice so we need to check to make sure the speed up is not applied twice.
            Owner.state.Speed += 0.75f;
        }
        cooldown.ResetCooldown();
        beganFiring = false;
        line.SetPositions(new Vector3[PointsInArc]);
        if (Owner.entity.isOwner) {
            launchProjectile.Launch();
            Owner.state.FireAnim();
            currentVelocity = 0f;
        }
    }

    public override void OnEquip() {
        line = GetComponent<LineRenderer>();
        line.positionCount = PointsInArc;
    }

    private float TimeOfImpact(Vector3 dir) {
        float time = 0f;
        bool collided = false;

        while (!collided) {
            time += .025f;
            Vector3 pos = launchProjectile.LaunchPosition.position + launchProjectile.LocalLaunchDir * time + Physics.gravity * time * time * .5f;
            collided = Physics.CheckSphere(pos, .15f, ~(1 << 12)) || time > 1000;
        }

        return time;
    }
}
