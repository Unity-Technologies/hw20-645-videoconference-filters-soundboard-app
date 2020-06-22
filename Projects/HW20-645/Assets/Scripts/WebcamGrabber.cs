using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamGrabber : MonoBehaviour
{
    public RenderTexture targetTexture;
    public int width = 1920, height = 1080, fps = 30;
    WebCamTexture webcamTexture;

    void Start()
    {
        SetWebCamTexture();
    }

    void Update()
    {
        Graphics.Blit(webcamTexture, targetTexture);
    }

    void SetWebCamTexture()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
        WebCamDevice[] devices = WebCamTexture.devices;

        webcamTexture = new WebCamTexture(devices[0].name, this.width, this.height, this.fps);
        webcamTexture.Play();
    }
}
