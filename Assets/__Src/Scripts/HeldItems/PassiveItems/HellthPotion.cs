using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellthPotion : HeldPassive
{
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

        ready = true;
    }
}
