using System;
using System.Runtime.InteropServices;
using System.Text;

public struct OpenFaceWrapperInfo
{
#if UNITY_STANDALONE_WIN
    public const string LibraryPath = "UnityOpenFaceWrapper";
    //public const string LibraryPath = "Packages/com.unity.openface/Plugins/x64/UnityOpenFaceWrapper.dll";
#else
    public const string LibraryPath = "N/A";
#endif
}

internal struct UnityOpenFaceWrapper
{
    [DllImport(OpenFaceWrapperInfo.LibraryPath)]
    public static extern int GetFeatures();
}
