using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentalPotion : HeldPassive
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

        Owner.state.Speed += Random.Range(-3, 3) / 10f;
        Owner.state.FireRate += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileSpeed += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileDamage += Random.Range(-3, 3) / 10f;

    }
}

