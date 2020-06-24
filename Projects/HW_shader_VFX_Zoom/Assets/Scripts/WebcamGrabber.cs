using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamGrabber : MonoBehaviour
{
    public RenderTexture targetTexture;
    public int width = 1920, height = 1080, fps = 60;
    WebCamTexture webcamTexture;
    public ComputeShader shader;
    private RenderTexture tempTexture;
    int kernelHandle;

    public bool swizzleChannels = false;

    public Renderer mesh = null;
    private string property = "_Main_tex";
    void Start()
    {
        tempTexture = new RenderTexture(width, height, 24);
        tempTexture.enableRandomWrite = true;
        tempTexture.Create();
        // targetTexture.enableRandomWrite = true;
        kernelHandle = shader.FindKernel("CSMain");

        SetWebCamTexture();

        if (mesh)
        {
            if(mesh.material.HasProperty(property))
            mesh.material.SetTexture(property, targetTexture);
        }
    }

    void Update()
    {
        if (swizzleChannels)
        {
            Graphics.Blit(webcamTexture, tempTexture);
            shader.SetTexture(kernelHandle, "Result", tempTexture);
            shader.Dispatch(kernelHandle, width / 8, height / 8, 1);
            Graphics.Blit(tempTexture, targetTexture);
        }
        else
        {
            Graphics.Blit(webcamTexture, targetTexture);
        }
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
