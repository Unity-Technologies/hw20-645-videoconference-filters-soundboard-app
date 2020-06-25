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
// A single landmark in 2D coordinates
public class OpenFaceLandmark3D
{
    public double x;
    public double y;
    public double z;
}

[Serializable]
// All the landmark info
public class OpenFaceLandmarks
{
    public bool success = false;
    public List<OpenFaceLandmark2D> landmarks2d;
    public List<OpenFaceLandmark3D> landmarks3d;
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
    public bool writeOpenFaceImages = false;
    public bool drawLandmarksIn2D = true;
    public bool drawLandmarksIn3D = false;

    private List<GameObject> faceIn3D = new List<GameObject>(); 
    
    // Status info in inspector
    public float frameRateFPS;
    private DateTime lastRunTime = DateTime.Now;
    public int numberFacesFound = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Test call
        string OpenFaceStuffPath = Application.dataPath + "/../../../com.unity.openface/OpenFace/model"; // TODO: find another less hacky way
        var result = UnityOpenFaceWrapper.OpenFaceSetup(OpenFaceStuffPath);
        if (!result)
            Debug.LogError($"Error setting up OpenFace wrapper");

        for (int i = 0; i < 68; i++) // num available landmarks
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            faceIn3D.Add(sphere);
        }
    }

    // Draw stuff in the 2D image
    private void DrawInTexture(Texture2D tex, List<OpenFaceLandmark2D> landmarks2d)
    {
        // Update Y axis because OpenFace flips the image
        foreach (var landmark in landmarks2d)
            landmark.y = tex.height - landmark.y;

        if (drawLandmarksIn2D)
        {
            foreach (var landmark in landmarks2d)
                tex.DrawCircle(Color.yellow, (int)landmark.x, (int)landmark.y, 10);
        }
                    
        // Draw nose
        // See https://github.com/TadasBaltrusaitis/OpenFace/wiki/Output-Format for landmark IDs
        var landmarkNoseTip = landmarks2d[33];
        tex.DrawCircle(Color.red, (int)landmarkNoseTip.x, (int)landmarkNoseTip.y, 60);
    }

    // Draw in 3D in Unity
    private void DrawIn3D(List<OpenFaceLandmark3D> landmarks3d)
    {
        for (int i=0; i < landmarks3d.Count; i++)
        {
            var landmark = landmarks3d[i];
            faceIn3D[i].transform.position = new Vector3((float)landmark.x, (float)landmark.y, (float)landmark.z);   
        }
    }

    // Update is called once per frame
    void Update()
    {    
        // Get the facial recognition features
        if (sourceTexture != null)
        {
            Texture2D tex = sourceTexture.toTexture2D();
            
            if (writeOpenFaceImages)
            {
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + $"/Captures/OpenFaceToProcess_{Time.frameCount}.png", bytes);
            }
            
            var pixels = tex.GetRawTextureData();
            
            StringBuilder sb = new StringBuilder(65536);
            var result = UnityOpenFaceWrapper.OpenFaceGetFeatures(pixels, tex.width, tex.height, sb, sb.Capacity);
            if (sb.ToString() != "")
            {
                // Parse JSON
                var openFaceData = JsonUtility.FromJson<OpenFaceJSONData>(sb.ToString());
                numberFacesFound = openFaceData.faces == null ? 0 : openFaceData.faces.Count;
                //Debug.LogWarning($"Found {countFaces} faces in the image.");
                
                // Draw something on the texture
                if (numberFacesFound == 1 && openFaceData.faces[0].landmarks.success)
                {
                    DrawInTexture(tex, openFaceData.faces[0].landmarks.landmarks2d);
                    
                    // Draw in 3D;\
                    if (drawLandmarksIn3D)
                        DrawIn3D(openFaceData.faces[0].landmarks.landmarks3d);
                    
                    // Save to file
                    if (writeOpenFaceImages)
                    {
                        byte[] bytes = tex.EncodeToPNG();
                        File.WriteAllBytes(Application.dataPath + $"/Captures/OpenFaceEdited_{Time.frameCount}.png", bytes);
                    }

                    tex.Apply();
                    // Now load the Texture2d into the target RenderTexture
                    var lastActive = RenderTexture.active;
                    //targetTexture = new RenderTexture(tex.width, tex.height, 32, GraphicsFormat.R8G8B8A8_UNorm);
                    RenderTexture.active = targetTexture;
                    // Copy your texture ref to the render texture
                    Graphics.Blit(tex, targetTexture);
                    RenderTexture.active = lastActive;
                    
                    // Draw into targetTexture
                }
                else
                {
                    Graphics.Blit(sourceTexture, targetTexture);
                    //targetTexture = sourceTexture;
                }
            }
            else
            {
                Graphics.Blit(sourceTexture, targetTexture);
                //targetTexture = sourceTexture;
            }
            
            // Update tracking FPS
            var newTime = DateTime.Now;
            frameRateFPS = 1000.0f / (newTime - lastRunTime).Milliseconds;
            lastRunTime = newTime;
        }
        else
        {
            numberFacesFound = 0;
        }
    }

    void OnApplicationQuit()
    {
        var result = UnityOpenFaceWrapper.OpenFaceClose();
        if (!result)
            Debug.LogError($"Error closing OpenFace wrapper");
    }
}