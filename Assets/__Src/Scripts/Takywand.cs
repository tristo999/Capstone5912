using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponLaunchProjectile))]
public class Takywand : Weapon
{
   // public AudioClip takyonScream;

    private float[] beatTimings = { 0.0f, 0.68f, 1.13f, 1.57f, 1.8f, 2.02f, 2.24f, 2.47f};
    private float timer = 0.0f;
    private int currentBeat = 0;
    private bool firing = false;
    private WeaponLaunchProjectile launchProj;
    private WeaponUses uses;

    private void Awake() {
        launchProj = GetComponent<WeaponLaunchProjectile>();
        uses = GetComponent<WeaponUses>();
    }

    public override void FireDown() {
        
    }

    public override void FireHold() {
        if (!Owner.entity.isOwner) return;
        if (currentBeat > beatTimings.Length || firing == false) {
            if (!launchProj.audioSource.isPlaying) {
                firing = true;
                currentBeat = 0;
                timer = 0.0f;
            }
        } else if (currentBeat < beatTimings.Length && timer > beatTimings[currentBeat]) {
            uses.Use();
            launchProj.Launch();
            currentBeat++;
        }
    }

    public override void FireRelease() {
        firing = false;
        timer = 0f;
        currentBeat = 0;
    }

    public override void OnEquip() {
        //launchProj.audioSource.PlayOneShot(takyonScream);
    }

    public override void OnDequip() { }

    // Update is called once per frame
    void Update()
    {
        if (firing) 
            timer += Time.deltaTime;
    }
}
