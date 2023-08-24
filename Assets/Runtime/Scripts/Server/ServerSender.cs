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
using XRRemote.Serializables;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public class ServerSender : CustomNdiSender
    {    
        [SerializeField] private XRRemotePlaneSender planeSender = null;
        [SerializeField] private ARCameraManager cameraManager = null;
        [SerializeField] private ARPoseDriver arPoseDriver = null;
        [SerializeField] private ARCameraBackground cameraBackground = null;
        [SerializeField] private AROcclusionManager occlusionManager = null;
        [SerializeField] private RawImage rawImage = null;
     
        private void Awake()
        {   
            if (Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("cannot use CustomNdiSender in Editor.");
                return;
            }
            ndiSenderName = "ServerSender";
        }

        protected override void Start()
        {
            base.Start();
            cameraManager.frameReceived += OnCameraFrameReceived;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }
        
        private void OnValidate()
        {
            if (cameraManager == null)
            {
                cameraManager = FindObjectOfType<ARCameraManager>();
            }
        }

        protected override RemotePacket GetPacketData()
        {
            ServerRemotePacket packet = new ServerRemotePacket();

            packet.cameraPose = arPoseDriver;
            packet.cameraIntrinsics = cameraManager.TryGetIntrinsics(out XRCameraIntrinsics intrinsics) ? new SerializableXRCameraIntrinsics(intrinsics) : null;

            occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage xrCpuImage);
            
            SerializableDepthImage xrDepthImage = new SerializableDepthImage(xrCpuImage);
            // XRCpuImage depTex = occlusionManager.environmentDepthTexture;
            // Debug.Log("deptex is readable: " + depTex.isReadable);
            if (xrDepthImage != null)
            {
                Debug.Log("depth image  width = " + xrDepthImage.width);
                Debug.Log("depTex height = " + xrDepthImage.height);
                Debug.Log("depTex planeCount = " + xrDepthImage.planeCount);
                Debug.Log("depTex length = " + xrDepthImage.texData.Length);
                Debug.Log("depTex format = " + xrDepthImage.format.ToString());
                rawImage.texture = occlusionManager.environmentDepthTexture;
                packet.depthImage = xrDepthImage;
                Debug.Log("texData length = " + packet.depthImage.texData.Length);


                // SerializableDepthImage depthImage = new SerializableDepthImage(depTex);
                // Texture2D reconstructedImage = depthImage.ReconstructDepthImageFromSerializableDepthImage();
                // rawImage.texture = reconstructedImage;

                // rawImage.texture = depTex;`
                // Debug.Log("Occlusion Manager environment depth texture length: " + depTex.GetRawTextureData().Length);
                
                // packet.depthImage = depthImage;
                // //[review]
                // // Debug.Log("serialized depthImage texData Length: " + depthImage.texData[depthImage.texData.Length -100]);
                // Debug.Log("serialized depthImage Width: " + depthImage.width);
                // Debug.Log("serialized depthImage height: " + depthImage.height);
                
                // // Debug.Log("IN THE PACKET - serialized depthImage: " + packet.depthImage);
            }
            else
            {
                packet.depthImage = null;
            }
            
            if (planeSender.TryGetPlanesInfo(out SerializablePlanesInfo planesInfo)) {
                packet.planesInfo = planesInfo;
            } else {
                packet.planesInfo = null;
            }

            return packet;
        }

        protected override Material GetCameraFrameMaterial()
        {
            return cameraBackground.material;
        }
    }
}
