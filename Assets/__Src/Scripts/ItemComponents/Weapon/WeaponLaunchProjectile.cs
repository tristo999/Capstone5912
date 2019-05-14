using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class WeaponLaunchProjectile : MonoBehaviour
{
    public AudioClip fireSound;
    public BoltEntity Projectile;
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
            AudioMixer mixer = Resources.Load("AudioMixer") as AudioMixer;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = fireSound;
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        }
    }

    public BoltEntity Launch(float launchDir = 0f, Vector3 torque = default) {
        if (audioSource) {
            audioSource.Stop();
            audioSource.time = 0f;
            audioSource.Play();
        }
        Vector3 newLocalLaunchDir = Quaternion.Euler(0, launchDir, 0) * LocalLaunchDir;
        BoltEntity proj = BoltNetwork.Instantiate(Projectile, weaponBase.Owner.launchPos.position, Quaternion.LookRotation(newLocalLaunchDir));
        proj.GetComponent<Rigidbody>().velocity = newLocalLaunchDir * LaunchForce * weaponBase.Owner.state.ProjectileSpeed;
        proj.GetComponent<Rigidbody>().AddRelativeTorque(torque);
        proj.GetState<IProjectileState>().Owner = weaponBase.Owner.entity;

        DamageOnCollide damageOnCollide = proj.GetComponent<DamageOnCollide>();
        if (damageOnCollide) {
            damageOnCollide.damageModifier = weaponBase.Owner.state.ProjectileDamage;
        }

        ExplosiveDamageOnCollide explosiveDamageOnCollide = proj.GetComponent<ExplosiveDamageOnCollide>();
        if (explosiveDamageOnCollide) {
            explosiveDamageOnCollide.damageModifier = weaponBase.Owner.state.ProjectileDamage;
        }

        weaponBase.Owner.state.FireAnim();
        return proj;
    }
}
