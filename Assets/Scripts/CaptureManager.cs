
using FFmpegOut;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public KeyCode screenShotKey;
    public KeyCode recordKey;
    public GameObject recordingText;

    private Camera cam;
    private CameraCapture cap;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cap = cam.GetComponent<CameraCapture>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(screenShotKey))
        {
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
        } else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(recordKey))
        {
            if (!cap.enabled)
            {
                cap.enabled = true;
                recordingText.SetActive(true);
            } else
            {
                cap.enabled = false;
                recordingText.SetActive(false);
            }
        }
    }
}
