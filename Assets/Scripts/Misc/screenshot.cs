using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaptureScreenshot : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip clickNoise;
    public String filepath;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ScreenCapture.CaptureScreenshot(filepath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
            audioSource.clip = clickNoise;
            audioSource.Play();
        }

    }
}