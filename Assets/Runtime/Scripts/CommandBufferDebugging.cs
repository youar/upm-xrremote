using System.Linq;
using System.Collections;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR;
using XRRemote;
using UnityEngine.Rendering;


namespace XRRemote
{
    public class CommandBufferDebugging : MonoBehaviour
    {
        public static CommandBufferDebugging Instance { get; private set; } = null;
        private CommandBuffer videoCommandBuffer;
        private bool videoCommandBufferInitialized = false;
        private Material mat;
        [HideInInspector]public RenderTexture texture;

        private void Start()
        {
            Instance = this;
            InitializeCommandBuffer();
        }

        private void OnDestroy()
        {
            if (videoCommandBufferInitialized)
            {
                GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, videoCommandBuffer);
                videoCommandBufferInitialized = false;
            }
        }

        private void InitializeCommandBuffer()
        {
            if (videoCommandBufferInitialized) return;
            Debug.Log("init buffer");
            videoCommandBuffer = new CommandBuffer();
            mat = new Material(Shader.Find("Unlit/Texture"));
            videoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, mat);
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, videoCommandBuffer);
            videoCommandBufferInitialized = true;
        }

        public void OnPreRender()
        {
            Debug.Log("pre render ran");
            if (!videoCommandBufferInitialized) InitializeCommandBuffer();
            mat.SetTexture("_MainTex", texture);
        }
    }
}
