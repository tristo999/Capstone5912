using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaunchProjectile : MonoBehaviour
{
    public AudioClip fireSound;
    public BoltEntity Projectile;
    public Transform LaunchPosition;
    public float LaunchAngle;
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
            return (Quaternion.AngleAxis(-LaunchAngle, weaponBase.Owner.transform.right) * weaponBase.Owner.transform.forward).normalized;
        }
    }
    public float LaunchForce;
    private Weapon weaponBase;
    public AudioSource audioSource;

    private void Awake() {
        weaponBase = GetComponent<Weapon>();
        if (fireSound) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = fireSound;
        }
    }

    public void Launch(float launchDir = 0f, Vector3 torque = default) {
        if (audioSource) {
            audioSource.Stop();
            audioSource.time = 0f;
            audioSource.Play();
        }
        Vector3 newLocalLaunchDir = Quaternion.Euler(0, launchDir, 0) * LocalLaunchDir;
        BoltEntity proj = BoltNetwork.Instantiate(Projectile, LaunchPosition.position, transform.rotation);
        proj.GetComponent<Rigidbody>().velocity = newLocalLaunchDir * LaunchForce * weaponBase.Owner.state.ProjectileSpeed;
        proj.GetComponent<Rigidbody>().AddRelativeTorque(torque);
        proj.GetState<IProjectileState>().Owner = weaponBase.Owner.entity;
        weaponBase.Owner.state.FireAnim();
    }
}
