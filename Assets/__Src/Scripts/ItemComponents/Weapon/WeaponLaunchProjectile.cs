using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class WeaponLaunchProjectile : NetworkBehaviour
{
    public AudioClip fireSound;
    public GameObject Projectile;
    public float LaunchAngle;
    [HideInInspector]
    public float ModifiedLaunchForce
    {
        get
        {
            return LaunchForce * weaponBase.Owner.GetComponent<PlayerStatsController>().ProjectileSpeed;
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

    public GameObject Launch(float launchDir = 0f, Vector3 torque = default) {
        if (audioSource) {
            audioSource.Stop();
            audioSource.time = 0f;
            audioSource.Play();
        }
        Vector3 newLocalLaunchDir = Quaternion.Euler(0, launchDir, 0) * LocalLaunchDir;

        GameObject proj = Instantiate(Projectile, weaponBase.Owner.launchPos.position, Quaternion.LookRotation(newLocalLaunchDir));
        proj.GetComponent<Rigidbody>().velocity = newLocalLaunchDir * LaunchForce * weaponBase.Owner.GetComponent<PlayerStatsController>().ProjectileSpeed;
        proj.GetComponent<Rigidbody>().AddRelativeTorque(torque);
        proj.GetComponent<Projectile>().OwnerGameObject = weaponBase.Owner.gameObject;

        DamageOnCollide damageOnCollide = proj.GetComponent<DamageOnCollide>();
        if (damageOnCollide) {
            damageOnCollide.damageModifier = weaponBase.Owner.GetComponent<PlayerStatsController>().ProjectileDamage;
        }

        ExplosiveDamageOnCollide explosiveDamageOnCollide = proj.GetComponent<ExplosiveDamageOnCollide>();
        if (explosiveDamageOnCollide) {
            explosiveDamageOnCollide.damageModifier = weaponBase.Owner.GetComponent<PlayerStatsController>().ProjectileDamage;
        }

        weaponBase.Owner.GetComponent<Animator>().SetTrigger("FireAnim");
        weaponBase.Owner.GetComponentInChildren<NetworkAnimator>().SetTrigger("FireAnim");

        return proj;
    }
}
