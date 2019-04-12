using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnPickup : MonoBehaviour
{
    public AudioClip clip;

    private void Awake() {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.PlayOneShot(clip);
    }
}
