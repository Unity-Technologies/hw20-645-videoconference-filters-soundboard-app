using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YUVChromaKeyWebCamDespill : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture m_GarbageMask;
    [Tooltip("Can be a Texture2D or a RenderTexture")]
    public Texture m_inputTexture;
    public RenderTexture OutputRT;
    private RenderTexture m_resultTex;
    public Color m_Color;
    public float m_DespillAmount;
    public float m_DistMul;
    public float m_Range;
    public float m_TolA;
    public float m_TolB;
    public enum OutputTypeEnum 
    {
        Result,
        Matte,
        Front,
        FrontCC,
        GMask
    }
    public OutputTypeEnum m_OutputType;

    public bool m_UseDeSpill = true;
    public bool m_UseInvertKey = true;
    public bool m_UseGMask = false;
    public bool m_UseInvertGMask = true;

    private WebCamTexture m_wc;

    private int m_CSMainKernelID;
    private MeshRenderer m_meshRenderer;
    private Material m_mat;
    public int m_Width = 0;
    public int m_Height = 0;

    private const int WARP_SIZE = 8;
    private int m_WarpX = 0;
    private int m_WarpY = 0;
    private Color32[] data;

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
        if (m_inputTexture == null)
        {
            Debug.LogError("YUVChromaKeyWebCamDespill behaviour: please set an input Texture !");
        }
        m_Width = m_inputTexture.width;
        m_Height = m_inputTexture.height;

        m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
        m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);

        m_CSMainKernelID = computeShader.FindKernel("CSMain");
        m_resultTex = AllocateBuffer(4, m_Width, m_Height);
        m_resultTex.enableRandomWrite = true;
        m_resultTex.Create();
        computeShader.SetTexture(m_CSMainKernelID, "resultTex", m_resultTex);
        computeShader.SetTexture(m_CSMainKernelID, "garbageMask", m_GarbageMask);
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetTexture(m_CSMainKernelID, "FG", m_inputTexture);
        Vector4 aVec = new Vector4(m_Color.r, m_Color.g, m_Color.b, m_Color.a);

        computeShader.SetInt("use_despill", m_UseDeSpill ? 1: 0);
        computeShader.SetInt("use_gmask", m_UseGMask ? 1 : 0);
        computeShader.SetInt("use_invert_key", m_UseInvertKey ? 1 : 0);
        computeShader.SetInt("use_invert_gmask", m_UseInvertGMask ? 1 : 0);
        computeShader.SetInt("type", (int)m_OutputType);
 
        computeShader.SetVector("input_keyin_color", m_Color);
        computeShader.SetFloat("range", m_Range);
        computeShader.SetFloat("dist_mul", m_DistMul);
        computeShader.SetFloat("tola", m_TolA/10.0f);
        computeShader.SetFloat("tolb", m_TolB/10.0f);
       
        computeShader.SetFloat("despill_amount", m_DespillAmount);


        computeShader.Dispatch(m_CSMainKernelID, m_WarpX, m_WarpY, 1);
        Graphics.Blit(m_resultTex, OutputRT, new Vector2(1,1), new Vector2(0, 0));
    }
}
