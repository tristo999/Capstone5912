using RockVR.Video;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public KeyCode screenShotKey;
    public KeyCode recordKey;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(screenShotKey))
        {
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
        } else if (Input.GetKeyDown(recordKey))
        {
            if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.NOT_START)
                VideoCaptureCtrl.instance.StartCapture();
            else
                VideoCaptureCtrl.instance.StopCapture();
        }
    }
}
