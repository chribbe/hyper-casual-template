using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRecorder : MonoBehaviour
{
    public string folder = "ScreenshotFolder";
    public int frameRate = 25;
    // Start is called before the first frame update
    void Awake()
    {
        Time.captureFramerate = this.frameRate;
        Debug.Log(Time.captureFramerate);

    }
    void Start()
    {
        Time.captureFramerate = this.frameRate;
        System.IO.Directory.CreateDirectory(folder);

    }

    // Update is called once per frame
    void Update()
    {

        string name = string.Format("{0}/image_{1:D04}.png", folder, Time.frameCount);
        ScreenCapture.CaptureScreenshot(name);
    }
}
