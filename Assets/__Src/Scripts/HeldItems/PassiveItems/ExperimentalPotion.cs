using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExperimentalPotion : HeldPassive
{
    public AudioClip sound;
    public AudioSource audioSource;
    bool ready;
    AudioMixer mixer;

    private void Awake(){
        audioSource = gameObject.AddComponent<AudioSource>();
        mixer = Resources.Load("AudioMixer") as AudioMixer;
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
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            audioSource.clip = sound;
            audioSource.time = 0f;
            audioSource.Play();
        }

        Owner.state.Speed += Random.Range(-3, 3) / 10f;
        Owner.state.FireRate += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileSpeed += Random.Range(-3, 3) / 10f;
        Owner.state.ProjectileDamage += Random.Range(-3, 3) / 10f;

        ready = true;
    }
}

