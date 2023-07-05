//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerSender.cs" createdby="gblikas">
// 
// XR Remote
// Copyright(C) 2020  YOUAR, INC.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// https://www.gnu.org/licenses/agpl-3.0.html
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see
// <http://www.gnu.org/licenses/>.
//
// </copyright>
//-------------------------------------------------------------------------------------------------------

using System;
// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.XR.ARFoundation;
// using Klak.Ndi;
using XRRemote.Serializables;

namespace XRRemote
{   
    
    [DisallowMultipleComponent]
    [Serializable]

    public sealed class ClientSender : CustomNdiSender
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

        protected override void Start()
        {
            base.Start();
            //in OG CustomNdiSender, the following line subscribes to an 'OnCameraFrameReceived' function
            // cameraManager.frameReceived += OnCameraFrameReceived;
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

        protected RemotePacket GetPacketData()
        {
            ServerPacket packet = new ServerPacket();

            packet.cameraPose = arPoseDriver;

            if (planeSender.TryGetPlanesInfo(out SerializablePlanesInfo planesInfo)) {
                packet.planesInfo = planesInfo;
            } else {
                packet.planesInfo = null;
            }

            return packet;
        }

        protected Material GetCameraFrameMaterial()
        {
            return cameraBackground.material;
        }

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

            if (planeSender.TryGetPlanesInfo(out SerializablePlanesInfo planesInfo)) {
                testPacket.planesInfo = planesInfo;
            } else {
                testPacket.planesInfo = null;
            }

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
