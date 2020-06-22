using UnityEngine;

namespace Scripts.Utils
{
    public class WebcamInput : MonoBehaviour
    {
        private WebCamTexture m_wc;
        public string m_CameraName;
        public RenderTexture output;
        private Texture2D m_cameraTexture;
        private Color32[] data;

        void setUpCamera(string deviceName)
        {
            m_wc.deviceName =deviceName;
            m_wc.requestedWidth = 512;
            m_wc.requestedHeight = 512;

            m_wc.Play();
            m_cameraTexture = new Texture2D(m_wc.width, m_wc.height);
            data = new Color32[m_wc.width * m_wc.height];
        }

        void Start()
        {
            if (output == null)
            {
                Debug.LogError("WebcamInput behaviour: please set an output RenderTexture !");
                return;
            }
            WebCamDevice[] devices = WebCamTexture.devices;
            m_wc = new WebCamTexture();

            if (devices.Length > 0)
            {
                string deviceName = "";
                if (devices.Length == 1)
                {
                    deviceName = devices[0].name;
                }
                else
                {
                    foreach (var device in devices)
                    {
                        if (device.name == m_CameraName)
                        {
                            deviceName = device.name;
                            break;
                        }
                    }
                }
                setUpCamera(deviceName);
            }
        }

        void Update()
        {
            m_wc.GetPixels32(data);
            m_cameraTexture.SetPixels32(data);
            m_cameraTexture.Apply();
            Graphics.Blit(m_cameraTexture, output, new Vector2(1,1), new Vector2(0, 0));
        }
    }
}