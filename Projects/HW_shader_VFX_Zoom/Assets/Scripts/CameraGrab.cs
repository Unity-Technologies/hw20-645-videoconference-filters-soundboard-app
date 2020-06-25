using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraGrab : MonoBehaviour
{
    public Camera targetCamera = null;

    public RenderTexture outputTexture = null;
    private void OnEnable()
    {
        if (targetCamera)
            CameraCaptureBridge.AddCaptureAction(targetCamera, AddCaptureCommands);
        else
            Debug.Log("CameraGrab WARNING MISSING camera ");
    }

    private void OnDisable()
    {
        if (targetCamera)
            CameraCaptureBridge.RemoveCaptureAction(targetCamera, AddCaptureCommands);
        else
            Debug.Log("CameraGrab WARNING MISSING camera ");
    }
    protected void AddCaptureCommands(RenderTargetIdentifier source, CommandBuffer cb)
    {
        //Debug.Log("AddCaptureCommands");
        if (outputTexture)
        {
            var tid = Shader.PropertyToID("_MainTex");
            cb.GetTemporaryRT(tid, outputTexture.width, outputTexture.height, 0, FilterMode.Bilinear);
            cb.Blit(source, tid);
            cb.Blit(tid, outputTexture);
            cb.ReleaseTemporaryRT(tid);
        }
        else
            Debug.Log("CameraGrab WARNING MISSING outputTexture ");
        //if (source == BuiltinRenderTextureType.CurrentActive)
        //{
        //    var tid = Shader.PropertyToID("_MainTex");
        //    cb.GetTemporaryRT(tid, m_RenderTexture.width, m_RenderTexture.height, 0, FilterMode.Bilinear);
        //    cb.Blit(source, tid);
        //    cb.Blit(tid, m_RenderTexture, copyMaterial);
        //    cb.ReleaseTemporaryRT(tid);
        //}
        //else
        //    cb.Blit(source, m_RenderTexture, copyMaterial);
    }




    //private Material copyMaterial
    //{
    //    get
    //    {
    //        if (m_CopyMaterial == null)
    //        {
    //            m_CopyMaterial = new Material(copyShader);
    //            if (m_CaptureAlpha)
    //                m_CopyMaterial.EnableKeyword("TRANSPARENCY_ON");
    //            if (flipVertically && !Options.useCameraCaptureCallbacks)
    //                m_CopyMaterial.EnableKeyword("VERTICAL_FLIP");
    //        }
    //        return m_CopyMaterial;
    //    }
    //}
    //private Shader copyShader
    //{
    //    get
    //    {
    //        if (m_CopyShader == null)
    //            m_CopyShader = Shader.Find("Hidden/Recorder/Inputs/CameraInput/Copy");
    //        return m_CopyShader;
    //    }
    //}

}
