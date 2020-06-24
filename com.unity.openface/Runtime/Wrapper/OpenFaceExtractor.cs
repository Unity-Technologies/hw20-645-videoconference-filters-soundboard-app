using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
public static class ExtensionMethod
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        var activeTex = RenderTexture.active;
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = activeTex;
        return tex;
    }
}

[Serializable]
public class OpenFaceJSONData
{
    public string name;
    public List<OpenFaceSingleFace> faces;
}

[Serializable]
/// Head pose is stored in the following format (X, Y, Z, rot_x, roty_y, rot_z),
/// translation is in millimeters with respect to camera centre (positize Z away from camera),
/// rotation is in radians around X,Y,Z axes with the convention R = Rx * Ry * Rz, left-handed positive sign
public class OpenFacePose
{
    public double x;
    public double y;
    public double z;
    public double rotX;
    public double rotY;
    public double rotZ;
}

[Serializable]
// A single landmark in 2D coordinates
public class OpenFaceLandmark2D
{
    public double x;
    public double y;
}

[Serializable]
// All the landmark info
public class OpenFaceLandmarks
{
    public bool success = false;
    public List<OpenFaceLandmark2D> landmarks2d;
}

[Serializable]
public class OpenFaceSingleFace
{
    public int id = -1;
    public Vector3 gaze0;
    public Vector3 gaze1;
    public OpenFacePose pose;
    public OpenFaceLandmarks landmarks;
}

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
        var result = UnityOpenFaceWrapper.OpenFaceSetup();
        if (!result)
            Debug.LogError($"Error setting up OpenFace wrapper");
    }

    // Update is called once per frame
    void Update()
    {
        // Get the facial recognition features
        if (sourceTexture != null)
        {
            Texture2D tex = sourceTexture.toTexture2D();
            var pixels = tex.GetRawTextureData();
            StringBuilder sb = new StringBuilder(65536);
            var result = UnityOpenFaceWrapper.OpenFaceGetFeatures(pixels, tex.width, tex.height, sb, sb.Capacity);
            if (sb.ToString() != "")
            {
                // Parse JSON
                var openFaceData = JsonUtility.FromJson<OpenFaceJSONData>(sb.ToString());
                var countFaces = openFaceData.faces == null ? 0 : openFaceData.faces.Count;
                //Debug.LogWarning($"Found {countFaces} faces in the image.");
            }
        }
    }

    void OnApplicationQuit()
    {
        var result = UnityOpenFaceWrapper.OpenFaceClose();
        if (!result)
            Debug.LogError($"Error closing OpenFace wrapper");
    }
}
