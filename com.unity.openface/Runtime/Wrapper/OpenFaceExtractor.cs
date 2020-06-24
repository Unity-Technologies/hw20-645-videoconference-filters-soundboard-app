using System;
using UnityEngine;

/// <summary>
/// A behaviour to extract information from a texture
/// </summary>
public class OpenFaceExtractor : MonoBehaviour
{
    public RenderTexture sourceTexture;
    public RenderTexture targetTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Test call
        int wtf = UnityOpenFaceWrapper.GetFeatures();
        Debug.LogWarning($"GetFeatures() returned {wtf}");
    }

    // Update is called once per frame
    void Update()
    {
        // Get the video frame
    }
}
