using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballWand : WizardWeapon
{
    public float BaseLaunchVelocity;
    public float MaxLaunchVelocity;
    public float Angle;
    public int PointsInArc;
    public GameObject projectile;

    private float currentVelocity;
    private LineRenderer line;
    private Vector3 spawnPos;

    public override void FireDown() {
        currentVelocity = BaseLaunchVelocity;
    }

    public override void FireHold() {
        spawnPos = transform.position + transform.forward * .3f + Vector3.up * .8f;
        if (currentVelocity < MaxLaunchVelocity)
            currentVelocity += 0.05f;
        Vector3[] positions = new Vector3[PointsInArc];
        Vector3 dir = (Quaternion.AngleAxis(-Angle, transform.right) * transform.forward).normalized * currentVelocity;
        float timeToImpact = TimeOfImpact(dir);
        float step = timeToImpact / PointsInArc;
        for (int i = 0; i < PointsInArc; i++) {
            positions[i] = spawnPos + dir * i * step + Physics.gravity * i * i * step * step * .5f;
        }

        line.SetPositions(positions);
    }

    public override void FireRelease() {
        spawnPos = transform.position + transform.forward * .3f + Vector3.up * .8f;
        BoltEntity proj = BoltNetwork.Instantiate(projectile, spawnPos, Quaternion.identity);
        proj.GetComponent<FireballProjectile>().owner = transform.parent.gameObject;
        proj.GetComponent<Rigidbody>().velocity = (Quaternion.AngleAxis(-Angle, transform.right) * transform.forward).normalized * currentVelocity;

        currentVelocity = 0f;
        line.SetPositions(new Vector3[PointsInArc]);
    }

    public override void OnEquip(PlayerMovementController player) {
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
            collided = Physics.CheckSphere(pos, .2f) || time > 10f;
        }

        return time;
    }
}
