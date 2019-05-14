using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundOnPickup : MonoBehaviour
{
    public AudioClip clip;

    private void Awake() {
        AudioMixer mixer = Resources.Load("AudioMixer") as AudioMixer;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        source.PlayOneShot(clip);
    }
}
