using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PipelineKeyer : MonoBehaviour
{
    public ComputeShader computeShader;
    private int m_CSMainKernelID;
    private int m_CSFinalCombineCSID;
    
    public RenderTexture m_GarbageMask;
    public Texture m_inputTexture;
    public RenderTexture OutputRT;
    public RenderTexture OutputFrontCCM_RT;

    private RenderTexture m_frontCCM;
    private RenderTexture m_resultTex;
    private RenderTexture m_finalResultTex;
    
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

    private MeshRenderer m_meshRenderer;
    private Material m_mat;
    public int m_Width;
    public int m_Height;

    private const int WARP_SIZE = 8;
    private int m_WarpX;
    private int m_WarpY;
    private Color32[] data;
    
    
    static RenderTexture AllocateBuffer(int componentCount, int width, int height)
    {
        var format = RenderTextureFormat.ARGBFloat;
        if (componentCount == 1) format = RenderTextureFormat.RFloat;
        if (componentCount == 2) format = RenderTextureFormat.RGFloat;

        var rt = new RenderTexture(width, height, 0, format);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    [Serializable]
    public class KeyNode
    {
        public ComputeShader cs;
        public RenderTexture m_InputFM;
        public RenderTexture m_ResultFM;
        protected RenderTexture m_TmpResTex;
        protected int m_Width;
        protected int m_Height;
        protected int m_WarpX;
        protected int m_WarpY;
   
        protected int m_csID;
           
        public virtual void  ConfigureShaderParams()
        {
        }

        public virtual void Execute()
        {
            cs.Dispatch(m_csID, m_WarpX, m_WarpY, 1);
            Graphics.Blit(m_TmpResTex, m_ResultFM, new Vector2(1,1), new Vector2(0, 0));

        }
        public virtual void InitKeyNode( int width, int height)
        {
        }
    }
    
    [Serializable]
    public class EdgeExpandNode : KeyNode
    {
  
        public float m_Amount;
        
        public override void InitKeyNode( int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
            m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);
            m_csID = cs.FindKernel("CSMain");
            m_TmpResTex = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTex.enableRandomWrite = true;
            m_TmpResTex.Create();
           
            ConfigureShaderParams();
        }

           
        public override void  ConfigureShaderParams()
        {
            cs.SetTexture(m_csID, "inputFM", m_InputFM);
            cs.SetTexture(m_csID, "resultFM", m_TmpResTex);
            cs.SetFloat("amount", m_Amount);
        }        
    }
    
    [Serializable]
    public class BlurMatteNode : KeyNode
    {
        private RenderTexture m_TmpResTexX;
        private RenderTexture m_TmpResTexY;
        private RenderTexture m_TmpResTexInY;
        private int m_csIDX;
        private int m_csIDY;
        public float m_Amount;
        
        public  override void InitKeyNode(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
            m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);
            m_csIDX = cs.FindKernel("CSBlurX");
            m_csIDY = cs.FindKernel("CSBlurY");
            
            m_TmpResTexX = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTexX.enableRandomWrite = true;
            m_TmpResTexX.Create();
            
            m_TmpResTexY = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTexY.enableRandomWrite = true;
            m_TmpResTexY.Create();

            m_TmpResTexInY = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTexInY.enableRandomWrite = true;
            m_TmpResTexInY.Create();

            ConfigureShaderParams();

        }

        public  override void ConfigureShaderParams()
        {
            cs.SetFloat("amount", m_Amount);

            cs.SetTexture(m_csIDX, "inputTexX", m_InputFM);
            cs.SetTexture(m_csIDX, "resultTexX", m_TmpResTexX);
            cs.SetTexture(m_csIDY, "resultTexX", m_TmpResTexX);
            // can't write into a RenderTexture Assets cs.SetTexture(m_csID, "resultFM", m_ResultFM);

            
            cs.SetTexture(m_csIDY, "inputTexY", m_TmpResTexInY);
            cs.SetTexture(m_csIDY, "resultTexY", m_TmpResTexY);
        }

        public  override void Execute()
        {
            cs.Dispatch(m_csIDX, m_WarpX, m_WarpY, 1);
            Graphics.Blit(m_TmpResTexX, m_TmpResTexInY, new Vector2(1,1), new Vector2(0, 0));
            cs.Dispatch(m_csIDY, m_WarpX, m_WarpY, 1);

            Graphics.Blit(m_TmpResTexY, m_ResultFM, new Vector2(1,1), new Vector2(0, 0));
        }
    }
    
    [Serializable]
    public class ShrinkMatteNode : KeyNode
    {
        public bool m_EnableShrink;
        public RenderTexture m_InputPreBlur;

        
        public  override void InitKeyNode(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
            m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);
            m_csID = cs.FindKernel("CSMain");
            
            m_TmpResTex = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTex.enableRandomWrite = true;
            m_TmpResTex.Create();
            
            ConfigureShaderParams();

        }

        public  override void ConfigureShaderParams()
        {
            cs.SetInt("enable_shrink", m_EnableShrink ? 1 : 0);
            cs.SetTexture(m_csID, "inputFM", m_InputFM);
            cs.SetTexture(m_csID, "inputFM_Pass0", m_InputPreBlur);
            cs.SetTexture(m_csID, "resultFM", m_TmpResTex);
        }

        public  override void Execute()
        {
            cs.Dispatch(m_csID, m_WarpX, m_WarpY, 1);
            Graphics.Blit(m_TmpResTex, m_ResultFM, new Vector2(1,1), new Vector2(0, 0));
        }
    }
    
    [Serializable]
    public class KeyFinalCombineNode : KeyNode
    {
        public enum PassOutputTypeEnum 
        {
            Result,
            Matte,
            Front,
            FrontCC,
            GMask,
            EdgeExpand,
            BlurMatte,
            ShrinkMatte
        }
        public PassOutputTypeEnum m_PassOutputType;

        public RenderTexture[] m_InputPass = new RenderTexture[7];

        public  override void InitKeyNode(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
            m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);
            m_csID = cs.FindKernel("CSMain");
            m_TmpResTex = PipelineKeyer.AllocateBuffer(4, m_Width, m_Height);
            m_TmpResTex.enableRandomWrite = true;
            m_TmpResTex.Create();
           
            ConfigureShaderParams();
        }

 
        public  override void ConfigureShaderParams()
        {

            cs.SetInt("type", (int)m_PassOutputType);
            cs.SetTexture(m_csID, "inputFM", m_InputFM);
            cs.SetTexture(m_csID, "inputFM_Pass0", m_InputPass[0]);
            cs.SetTexture(m_csID, "inputFM_Pass1", m_InputPass[1]);
            cs.SetTexture(m_csID, "inputFM_Pass2", m_InputPass[2]);
            cs.SetTexture(m_csID, "inputFM_Pass3", m_InputPass[3]);
            cs.SetTexture(m_csID, "inputFM_Pass4", m_InputPass[4]);
            cs.SetTexture(m_csID, "inputFM_Pass5", m_InputPass[5]);
            cs.SetTexture(m_csID, "inputFM_Pass6", m_InputPass[6]);


            cs.SetTexture(m_csID, "resultFM", m_TmpResTex);
        }

        public  override void Execute()
        {
            cs.Dispatch(m_csID, m_WarpX, m_WarpY, 1);
            Graphics.Blit(m_TmpResTex, m_ResultFM, new Vector2(1,1), new Vector2(0, 0));
        }
    }
    
    public EdgeExpandNode EdgeExpand;
    public BlurMatteNode BlurMatte;
    public ShrinkMatteNode ShrinkMatte;
    public KeyFinalCombineNode KeyCombine;

    // Start is called before the first frame update
    void Start()
    {
        if (m_inputTexture == null)
        {
            Debug.LogError("PipelineKey behaviour: please set an input Texture !");
        }
        m_Width = m_inputTexture.width;
        m_Height = m_inputTexture.height;

        m_WarpX = Mathf.CeilToInt((float)m_Width / WARP_SIZE);
        m_WarpY = Mathf.CeilToInt((float)m_Height / WARP_SIZE);

        m_CSMainKernelID = computeShader.FindKernel("CSMain");
        m_resultTex = AllocateBuffer(4, m_Width, m_Height);
        m_resultTex.enableRandomWrite = true;
        m_resultTex.Create();
        
        m_frontCCM = AllocateBuffer(4, m_Width, m_Height);
        m_frontCCM.enableRandomWrite = true;
        m_frontCCM.Create();
        
        m_finalResultTex = AllocateBuffer(4, m_Width, m_Height);
        m_finalResultTex.enableRandomWrite = true;
        m_finalResultTex.Create();
     
        
        computeShader.SetTexture(m_CSMainKernelID, "resultTex", m_resultTex);
        computeShader.SetTexture(m_CSMainKernelID, "garbageMask", m_GarbageMask);
        computeShader.SetTexture(m_CSMainKernelID, "frontCCM", m_frontCCM);

        EdgeExpand.InitKeyNode(m_Width, m_Height);
        EdgeExpand.ConfigureShaderParams();

        BlurMatte.InitKeyNode(m_Width, m_Height);
        BlurMatte.ConfigureShaderParams();
        
        ShrinkMatte.InitKeyNode(m_Width, m_Height);
        ShrinkMatte.ConfigureShaderParams();

        KeyCombine.InitKeyNode(m_Width, m_Height);
        KeyCombine.ConfigureShaderParams();
        
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetTexture(m_CSMainKernelID, "inputTex", m_inputTexture);
        computeShader.SetTexture(m_CSMainKernelID, "garbageMask", m_GarbageMask);
        computeShader.SetTexture(m_CSMainKernelID, "frontCCM", m_frontCCM);
        computeShader.SetTexture(m_CSMainKernelID, "resultTex", m_resultTex);

        computeShader.SetInt("use_despill", m_UseDeSpill ? 1: 0);
        computeShader.SetInt("use_gmask", m_UseGMask ? 1 : 0);
        computeShader.SetInt("use_invert_key", m_UseInvertKey ? 1 : 0);
        computeShader.SetInt("use_invert_gmask", m_UseInvertGMask ? 1 : 0);
        int outputType = (int) m_OutputType;
        computeShader.SetInt("type", (int)outputType);
 
        computeShader.SetVector("input_keyin_color", m_Color);
        computeShader.SetFloat("range", m_Range);
        computeShader.SetFloat("dist_mul", m_DistMul);
        computeShader.SetFloat("tola", m_TolA/20.0f);
        computeShader.SetFloat("tolb", m_TolB/20.0f);
       
        computeShader.SetFloat("despill_amount", m_DespillAmount);


        computeShader.Dispatch(m_CSMainKernelID, m_WarpX, m_WarpY, 1);
        Graphics.Blit(m_resultTex, OutputRT, new Vector2(1,1), new Vector2(0, 0));
        Graphics.Blit(m_frontCCM, OutputFrontCCM_RT, new Vector2(1,1), new Vector2(0, 0));
        
        EdgeExpand.ConfigureShaderParams();
        EdgeExpand.Execute();
        
        BlurMatte.ConfigureShaderParams();
        BlurMatte.Execute();
        
        ShrinkMatte.ConfigureShaderParams();
        ShrinkMatte.Execute();

        KeyCombine.ConfigureShaderParams();
        KeyCombine.Execute();
    }
}
