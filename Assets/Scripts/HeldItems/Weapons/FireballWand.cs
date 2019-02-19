using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballWand : Weapon
{
    public float BaseLaunchVelocity;
    public float MaxLaunchVelocity;
    public float Angle;
    public int PointsInArc;
    public GameObject projectile;

    private float currentVelocity;
    private LineRenderer line;
    private Vector3 spawnPos;

    private bool beganFiring;

    private float timer;
    public float FireTime = 1.25f;

    public override void FireDown() {
        currentVelocity = BaseLaunchVelocity * Owner.state.ProjectileSpeed;
    }

    public override void FireHold() {
        if (timer > 0) return;
        if (!beganFiring) {
            Owner.state.Speed -= 0.75f;
            beganFiring = true;
        }
        spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * .6f;
        if (currentVelocity < MaxLaunchVelocity * Owner.state.ProjectileSpeed)
            currentVelocity += 0.05f * Owner.state.ProjectileSpeed;
        Vector3[] positions = new Vector3[PointsInArc];
        Vector3 dir = (Quaternion.AngleAxis(-Angle, transform.right) * transform.forward).normalized * currentVelocity + Owner.GetComponent<Rigidbody>().velocity * .8f;
        float timeToImpact = TimeOfImpact(dir);
        float step = timeToImpact / PointsInArc;
        for (int i = 0; i < PointsInArc; i++) {
            positions[i] = spawnPos + dir * i * step + Physics.gravity * i * i * step * step * .5f;
        }

        line.SetPositions(positions);
    }

    public override void FireRelease() {
        if (timer > 0) return;
        if (beganFiring) {
            // Sometimes firerelease gets called twice so we need to check to make sure the speed up is not applied twice.
            Owner.state.Speed += 0.75f;
        }
        beganFiring = false;

        BoltEntity proj = BoltNetwork.Instantiate(projectile, spawnPos, Quaternion.identity);
        proj.GetComponent<FireballWandProjectile>().owner = transform.parent.gameObject;
        proj.GetComponent<Rigidbody>().velocity = (Quaternion.AngleAxis(-Angle, transform.right) * transform.forward).normalized * currentVelocity;

        currentVelocity = 0f;
        line.SetPositions(new Vector3[PointsInArc]);
        timer = FireTime * Owner.state.FireRate;
    }

    private void Update() {
        timer -= Time.deltaTime;
    }

    public override void OnEquip() {
        line = GetComponent<LineRenderer>();
        line.positionCount = PointsInArc;
        spawnPos = transform.position + transform.forward * .3f + Vector3.up * .8f;
    }

    private float TimeOfImpact(Vector3 dir) {
        float time = 0f;
        bool collided = false;

        while (!collided) {
            time += .05f;
            Vector3 pos = spawnPos + dir * time + Physics.gravity * time * time * .5f;
            collided = Physics.CheckSphere(pos, .15f, ~(1 << 12)) || time > 500;
        }

        return time;
    }
}
