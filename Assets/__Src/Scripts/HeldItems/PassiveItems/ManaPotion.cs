using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : HeldPassive {

    public AudioClip sound;
    public AudioSource audioSource;
    bool ready;

    private void Awake(){
        audioSource = gameObject.AddComponent<AudioSource>();
        ready = false;
    }

    public void Update(){
        if(ready && !audioSource.isPlaying){
            base.OnEquip();
        }
    }

    public override void OnEquip() {
        
        if (sound != null) {
            audioSource.Stop();
            audioSource.clip = sound;
            audioSource.time = 0f;
            audioSource.Play();
        }

        Weapon wep = Owner.GetComponent<PlayerInventoryController>().wizardWeapon;
        if (wep) {
            WeaponUses wepUses = wep.GetComponent<WeaponUses>();
            if (wepUses && wepUses.AmountUsed > 0) {
                wepUses.AmountUsed = 0;
                Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Weapon uses restored!", Owner.transform.position);
            }
        }

        ActiveItem active = Owner.GetComponent<PlayerInventoryController>().activeItem;
        if (active) {
            ActiveUses activeUses = active.GetComponent<ActiveUses>();
            if (activeUses && activeUses.AmountUsed > 0) {
                activeUses.AmountUsed = 0;
                Owner.GetComponent<PlayerStatsController>().ui.AddFloatingMessageText("Active uses restored!", Owner.transform.position);
            }
        }

        ready = true;
    }
}
