﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyResultFinalCombine : MonoBehaviour
{
    public ComputeShader computeShader;

    public RenderTexture FrontCC;

    public RenderTexture Matte;
    public int m_Width = 0;
    public int m_Height = 0;

    private const int WARP_SIZE = 8;
    private int m_WarpX = 0;
    private int m_WarpY = 0;
    
    private RenderTexture m_resultTex;
    public RenderTexture OutputRT;
    private int m_CSMainKernelID;

    RenderTexture AllocateBuffer(int componentCount, int width, int height)
    {
        var format = RenderTextureFormat.ARGBFloat;
        if (componentCount == 1) format = RenderTextureFormat.RFloat;
        if (componentCount == 2) format = RenderTextureFormat.RGFloat;

        var rt = new RenderTexture(width, height, 0, format);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        m_Width = FrontCC.width;
        m_Height = FrontCC.height;

        m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
        m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);

        m_CSMainKernelID = computeShader.FindKernel("CSMain");
        m_resultTex = AllocateBuffer(4, m_Width, m_Height);
        m_resultTex.enableRandomWrite = true;
        m_resultTex.Create();
        computeShader.SetTexture(m_CSMainKernelID, "resultTex", m_resultTex);
        computeShader.SetTexture(m_CSMainKernelID, "frontCC", FrontCC);
        computeShader.SetTexture(m_CSMainKernelID, "matte", Matte);
        

    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetTexture(m_CSMainKernelID, "finalResultTex", m_resultTex);
        computeShader.SetTexture(m_CSMainKernelID, "frontCC", FrontCC);
        computeShader.SetTexture(m_CSMainKernelID, "matte", Matte);

        computeShader.Dispatch(m_CSMainKernelID, m_WarpX, m_WarpY, 1);
        Graphics.Blit(m_resultTex, OutputRT, new Vector2(1,1), new Vector2(0, 0));

    }
}