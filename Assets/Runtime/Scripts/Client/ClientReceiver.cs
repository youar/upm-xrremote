//-------------------------------------------------------------------------------------------------------
// <copyright file="ClientReceiver.cs" createdby="gblikas">
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
using System.Linq;
using System.Collections;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.Rendering;
using XRRemote.Serializables;
using UnityEngine.XR.ARSubsystems;


namespace XRRemote
{
    public class ClientReceiver : CustomNdiReceiver
    {
        public static ClientReceiver Instance { get; private set; } = null;
        public ServerRemotePacket remotePacket { get; private set; } = null;
        public XRCameraIntrinsics cameraIntrinsics {get; private set;}
        public event EventHandler OnPlanesInfoReceived;
        public event EventHandler OnInputDataReceived;

        [Tooltip("Camera that will render the NDI video")]
        [SerializeField] private Camera receivingCamera;
        private CommandBuffer videoCommandBuffer;
        private bool videoCommandBufferInitialized = false;
        
        // [SerializeField] 
        private Material commandBufferMaterial;

        private void Awake()
        {
            // It works only in Editor!
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("cannot use CustomNdiReceiver in Editor.");
                return;
            }

            if (ClientReceiver.Instance != null)
            {
                Debug.LogError("CustomNdiReceiver must be only one in the scene.");
            }

            ClientReceiver.Instance = this;

            targetNdiSenderName = "ServerSender";
        }

        protected override void Start()
        {
            base.Start();
            InitializeCommandBuffer();
            TrySetupTrackedPoseDriver();
        }

        private void OnDestroy()
        {
            if (videoCommandBuffer != null)
            {
                receivingCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque,videoCommandBuffer);
            }
            videoCommandBufferInitialized = false;
        }

        protected override void ReceiveTexture(RenderTexture texture)
        {
            commandBufferMaterial.SetTexture("_MainTex", texture);                
        }


        protected override void ProcessPacketData(byte[] bytes)
        {
            
            ServerRemotePacket remotePacket = ObjectSerializationExtension.Deserialize<ServerRemotePacket>(bytes);
            
            this.remotePacket = remotePacket;
            this.cameraIntrinsics = remotePacket.cameraIntrinsics.ToXRCameraIntrinsics();


            PlanesInfoCheck(remotePacket);

            if (remotePacket.touchPositionNormalized != null) {
                OnInputDataReceived?.Invoke(this, EventArgs.Empty);
            }
        }

        private void PlanesInfoCheck(ServerRemotePacket remotePacket)
        {
            if (remotePacket.planesInfo != null) 
            {
                OnPlanesInfoReceived?.Invoke(this, EventArgs.Empty);
            } 
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void InitializeCommandBuffer()
        {
            if (videoCommandBufferInitialized) return;
            videoCommandBuffer = new CommandBuffer();
            commandBufferMaterial = Resources.Load("XRVideoMaterial") as Material;
            // commandBufferMaterial = new Material(Shader.Find("Unlit/XRRemoteVideo"));
            videoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, commandBufferMaterial);
            receivingCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, videoCommandBuffer);
            videoCommandBufferInitialized = true;
        }
        

        private bool TrySetupTrackedPoseDriver()
        {
            TrackedPoseDriver trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();
            if (trackedPoseDriver == null) {
                    if (DebugFlags.displayEditorConnectionStats) {
                        Debug.LogError("TrySetupTrackedPoseDriver Event: null TrackedPoseDriver on main camera");
                    }
                    return false;
            }

            if (TryGetComponent<XRRemotePoseProvider>(out XRRemotePoseProvider remotePoseProvider))
            {
                trackedPoseDriver.poseProviderComponent = remotePoseProvider;
                return true;
            }

            if (DebugFlags.displayEditorConnectionStats) {
                Debug.LogError("TrySetupTrackedPoseDriver Event: null XRRemotePoseProvider on Ndi receiver");
            }
            return false;
        }

    }
}
