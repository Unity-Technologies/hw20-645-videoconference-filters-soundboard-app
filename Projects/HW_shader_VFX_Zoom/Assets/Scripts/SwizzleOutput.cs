using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwizzleOutput : MonoBehaviour
{
    public RenderTexture inputTexture, outputTexture;
    public int width = 1920, height = 1080, fps = 60;

    public ComputeShader shader;
    private RenderTexture tempTexture;
    int kernelHandle;
    // Start is called before the first frame update
    void Start()
    {
        tempTexture = new RenderTexture(width, height, 24);
        tempTexture.enableRandomWrite = true;
        tempTexture.Create();
        // targetTexture.enableRandomWrite = true;
        kernelHandle = shader.FindKernel("CSMain");
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.Blit(inputTexture, tempTexture);
        shader.SetTexture(kernelHandle, "Result", tempTexture);
        shader.Dispatch(kernelHandle, width / 8, height / 8, 1);
        Graphics.Blit(tempTexture, outputTexture);
    }
}
