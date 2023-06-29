using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
// using XRRemote;
using XRRemote.Serializables;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public sealed class CustomNdiSender : MonoBehaviour
    {    
        [SerializeField] private CustomPlaneSender planeSender = null;
        [SerializeField] private ARCameraManager cameraManager = null;
        [SerializeField] private ARPoseDriver arPoseDriver = null;
        [SerializeField] private ARCameraBackground cameraBackground = null;
        [SerializeField] private NdiResources resources = null;

        private int frameCount = 0;
        public MeshRenderer ndiSenderVisualizer = null;
        private NdiSender ndiSender = null;
        private RenderTexture renderTexture;
        private CommandBuffer commandBuffer;

        private void Awake()
        {
            if (Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("cannot use CustomNdiSender in Editor.");
                return;
            }
        }

        private void Start()
        {
            frameCount = 0;
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "CustomNdiSender";
            cameraManager.frameReceived += OnCameraFrameReceived;
        }

        private void OnDestroy()
        {
            frameCount = 0;
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnCameraFrameReceived;
            }
            
            commandBuffer?.Dispose();
        }
        
        private void OnValidate()
        {
            if (cameraManager == null)
            {
                cameraManager = FindObjectOfType<ARCameraManager>();
            }
        }

        // CustomNdiSender.cs
        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            if (renderTexture == null)
            {
                //Set texture
                int width = cameraBackground.material.mainTexture.width; 
                int height = cameraBackground.material.mainTexture.height;
                InitNdi(width, height);
            }

            //Set metadata
            RemotePacket testPacket = new RemotePacket();
            testPacket.cameraPose = arPoseDriver;

            if (planeSender.TryGetPlanesInfo(out PlanesInfo planesInfo)) {
                testPacket.planesInfo = planesInfo;
            } else {
                testPacket.planesInfo = null;
            }

            // if (poseSender.TryGetPoseInfo(out XRRemote.Serializables.Pose pose)) {
            //     testPacket.cameraPose = pose;
            // } else {
            //     testPacket.cameraPose = null;
            // }

            //Serialize metadata
            byte[] serializedData = ObjectSerializationExtension.SerializeToByteArray(testPacket); 
            ndiSender.metadata = "<![CDATA[" + Convert.ToBase64String(serializedData) + "]]>";
            
            commandBuffer.Blit(null, renderTexture, cameraBackground.material);
            Graphics.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            
            frameCount++;
        } 

        private void InitNdi(int width, int height)
        {
            Debug.Log($"Init NDI width: {width} height: {height}");
            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var name = string.Format("CustomNdiSender");
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);

            ndiSender = go.AddComponent<NdiSender>();
            ndiSender.SetResources(resources);
            ndiSender.captureMethod = CaptureMethod.Texture;
            ndiSender.keepAlpha = false;
            ndiSender.ndiName = "CustomNdiSender";
            ndiSender.sourceTexture = renderTexture;

            if (ndiSenderVisualizer != null)
            {
                ndiSenderVisualizer.material.mainTexture = renderTexture;
            }
        }
    }
}
