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
using UnityEngine.SpatialTracking;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.UI;
using XRRemote.Serializables;

namespace XRRemote
{
    [SerializeField]
    public class ClientReceiver : CustomNdiReceiver
    
    {
        public static ClientReceiver Instance { get; private set; } = null;
        public ServerRemotePacket remotePacket { get; private set; } = null;
        public XRCameraIntrinsics cameraIntrinsics {get; private set;}
        public event EventHandler OnPlanesInfoReceived;
        public event EventHandler OnInputDataReceived;

        private Camera receivingCamera;
        private CommandBuffer videoCommandBuffer;
        private bool videoCommandBufferInitialized = false;       
        public RawImage DepthImage;

        
        // [SerializeField] 
        private Material commandBufferMaterial;

        [Tooltip("List of AR Cameras that will render the NDI video")]
        [HideInInspector][SerializeField] private List<ARCameraManager> cameraManagerList = new List<ARCameraManager>();

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

        private void OnEnable()
        {
            StartCoroutine(SetReceivingCamera());
            TrySetupTrackedPoseDriver();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnDisable()
        {
            if (videoCommandBuffer != null && receivingCamera != null)
            {
                receivingCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, videoCommandBuffer);
            }
            videoCommandBufferInitialized = false;
            StopCoroutine(SetReceivingCamera());
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

            Debug.Log($"texData Length: {remotePacket.depthImage.texData?.Count()}");
            Debug.Log($"texFormat: {remotePacket.depthImage.format}");

            if (remotePacket.depthImage.texData == null)
            {
                Debug.Log("texData is null");
            }
            else
            {
                Debug.Log("texData is not null");
                
                // Texture2D newDepthImage = remotePacket.depthImage.ReconstructTexture2DFromSerializableDepthImage(remotePacket.depthImage);
                // Debug.Log("serialized depthImage texData byte value: " + remotePacket.depthImage.texData[remotePacket.depthImage.texData.Length -100]);
                // Debug.Log("newDepthImage width = " + newDepthImage.width + "newDepthImage.height = " + newDepthImage.height + "newDepthImage.format = " + newDepthImage.format);

                //texture2d stuff

                bool isSupported = SystemInfo.SupportsTextureFormat(TextureFormat.R16);
                Debug.Log("Is R16 supported: " + isSupported);
                //

                byte[] byteArray = remotePacket.depthImage.texData;
                
                ushort[] ushortArray = new ushort[byteArray.Length / 2];

                for (int i = 0; i < byteArray.Length; i += 2)
                {
                    ushortArray[i / 2] = BitConverter.ToUInt16(byteArray, i);
                }
                
                Texture2D texture = new Texture2D(remotePacket.depthImage.width, remotePacket.depthImage.height, TextureFormat.R16, false);

                byte[] textureBytes = new byte[ushortArray.Length * 2];
                Buffer.BlockCopy(ushortArray, 0, textureBytes, 0, textureBytes.Length);
                texture.LoadRawTextureData(textureBytes);
                texture.Apply();
                
                GameObject depthImageGO = GameObject.Find("Quad");
                
                    Material rawImageMaterial = depthImageGO.GetComponent<Renderer>().material;

                    if (rawImageMaterial != null)
                    {
                        Debug.Log("RawImage component found on the DepthImage GameObject.");
                        rawImageMaterial.mainTexture = texture;
                    }
                    else
                    {
                        Debug.LogError("No RawImage component found on the DepthImage GameObject.");
                    }               
            }
        
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

        private IEnumerator SetReceivingCamera()
        {
            while(true)
            {
                if (cameraManagerList.Count == 0)
                {
                    Debug.Log("XRRemote: Empty camera manager list");
                    yield return new WaitForSeconds(5f);
                }

                int activeCameraCount = 0;
                cameraManagerList
                    .Where(manager => manager != null).ToList()
                    .ForEach(manager => {
                        var camera = manager.gameObject.GetComponent<Camera>();
                        if(camera.isActiveAndEnabled) 
                        {
                            activeCameraCount++;
                            if (receivingCamera == null)
                            {
                                receivingCamera = camera;
                                InitializeCommandBuffer();
                            }
                            else if (receivingCamera!=camera)
                            {
                                receivingCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque,videoCommandBuffer);
                                receivingCamera = camera;
                                InitializeCommandBuffer();
                            }
                        }
                    });

                if (activeCameraCount > 1)
                {
                    Debug.LogError("XRRemote: Multiple active receiving cameras found");
                }

                yield return new WaitForSeconds(5f);
            }
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

        //required for editor script
        public void AddCameraManager()
        {
            cameraManagerList.Add(null);
        }
    }
}
