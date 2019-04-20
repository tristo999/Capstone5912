using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CenterMusicController : MonoBehaviour
{
    public AudioMixerSnapshot outer;
    public AudioMixerSnapshot center;
    // borrowed from a unity tutorial l o l
    public float bpm = 128;
    private float m_TransitionIn;
    private float m_TransitionOut;
    private float m_QuarterNote;

    void Start()
    {
        m_QuarterNote = 60 / bpm;
        m_TransitionIn = m_QuarterNote;
        m_TransitionOut = m_QuarterNote * 6;
    }

    // when entering center room trigger, fade into battle music
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Music"))
        {
            center.TransitionTo(m_TransitionIn);
        }
    }

    // when leaving center, play bouncy celtic music again
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Music"))
        {
            outer.TransitionTo(m_TransitionOut);
        }
    }
}
