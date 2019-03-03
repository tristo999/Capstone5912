using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaunchProjectile : MonoBehaviour
{
    public BoltEntity Projectile;
    public Transform LaunchPosition;
    public Vector3 LaunchDirection;
    [HideInInspector]
    public float ModifiedLaunchForce
    {
        get
        {
            return LaunchForce * weaponBase.Owner.state.ProjectileSpeed;
        }
    }
    [HideInInspector]
    public Vector3 LocalLaunchDir
    {
        get
        {
            return Quaternion.Euler(LaunchDirection) * weaponBase.Owner.transform.forward;
        }
    }
    public float LaunchForce;
    private Weapon weaponBase;

    private void Awake() {
        weaponBase = GetComponent<Weapon>();
    }

    public void Launch() {
        BoltEntity proj = BoltNetwork.Instantiate(Projectile, LaunchPosition.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().velocity = LocalLaunchDir * LaunchForce * weaponBase.Owner.state.ProjectileSpeed;
        proj.GetState<IProjectileState>().Owner = weaponBase.Owner.entity;
        weaponBase.Owner.state.FireAnim();
    }
}
