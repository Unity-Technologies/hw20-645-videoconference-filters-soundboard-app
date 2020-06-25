using System;
using System.Runtime.InteropServices;
using System.Text;

public struct OpenFaceWrapperInfo
{
#if UNITY_STANDALONE_WIN
    public const string LibraryPath = "UnityOpenFaceWrapper";
#else
    public const string LibraryPath = "N/A";
#endif
}

internal struct UnityOpenFaceWrapper
{
    [DllImport(OpenFaceWrapperInfo.LibraryPath)]
    public static extern bool OpenFaceSetup(string exePath);
    
    [DllImport(OpenFaceWrapperInfo.LibraryPath)]
    public static extern bool OpenFaceGetFeatures(byte[] pixels, int width, int height, StringBuilder jsonData, int jsonDataLength);
    
    [DllImport(OpenFaceWrapperInfo.LibraryPath)]
    public static extern bool OpenFaceClose();
}
