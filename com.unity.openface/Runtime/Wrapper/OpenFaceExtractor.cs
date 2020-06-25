using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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
    
    public static Texture2D DrawCircle(this Texture2D tex, Color color, int x, int y, int radius = 3)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
        for (int v = y - radius; v < y + radius + 1; v++)
            if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                tex.SetPixel(u, v, color);

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
    // If the image is flipped vertically, facial recognition will not work as expected
    //public bool flipWebcamImage;
    public RenderTexture sourceTexture;
    public RenderTexture targetTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Test call
        string OpenFaceStuffPath = Application.dataPath + "/../../../com.unity.openface/OpenFace/model"; // TODO: find another less hacky way
        var result = UnityOpenFaceWrapper.OpenFaceSetup(OpenFaceStuffPath);
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
                
                // Draw something on the texture
                if (countFaces == 1 && openFaceData.faces[0].landmarks.success)
                {
                    // See https://github.com/TadasBaltrusaitis/OpenFace/wiki/Output-Format for landmark IDs
                    var landmarkNoseTip = openFaceData.faces[0].landmarks.landmarks2d[33];
                    tex.DrawCircle(Color.red, (int)landmarkNoseTip.x, (int)landmarkNoseTip.y, 30);
                    
                    // Save to file
                    bool writeToFile = false;
                    if (writeToFile)
                    {
                        byte[] bytes = tex.EncodeToPNG();
                        File.WriteAllBytes(Application.dataPath + "/../Capture.png", bytes);
                    }

                    // Now load the Texture2d into the target RenderTexture
                    var lastActive = RenderTexture.active;
                    targetTexture = new RenderTexture(tex.width, tex.height, 32, GraphicsFormat.R8G8B8A8_UNorm);
                    RenderTexture.active = targetTexture;
                    // Copy your texture ref to the render texture
                    Graphics.Blit(tex, targetTexture);
                    RenderTexture.active = lastActive;
                }
                else
                {
                    targetTexture = sourceTexture;
                }
            }
            else
            {
                targetTexture = sourceTexture;
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
