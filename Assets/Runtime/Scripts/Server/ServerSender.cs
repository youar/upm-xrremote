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
using System.Linq;

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

        Texture2D texture = null;
     
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

        public Texture2D UpdateToXRCpuImage(XRCpuImage xRCpuImage){
            if(texture == null || texture.width != xRCpuImage.width || texture.height != xRCpuImage.height){
                if(texture != null) Destroy(texture); 
                texture = new Texture2D(xRCpuImage.width, xRCpuImage.height, xRCpuImage.format.AsTextureFormat(), false);
            }

            Debug.Log($"[UpdateToXRCpuImage] xRCpuImage.format.AsTextureFormat(): {xRCpuImage.format.AsTextureFormat()}");
            
            var conversionParams = new XRCpuImage.ConversionParams(xRCpuImage, xRCpuImage.format.AsTextureFormat(),XRCpuImage.Transformation.MirrorX);
            
            var textureData = texture.GetRawTextureData<byte>(); 
            var convertedDataSize = xRCpuImage.GetConvertedDataSize(conversionParams);
            if( textureData.Length != convertedDataSize){
                Debug.LogError($"failed to convert: size-mismatch: convertedDataSize {convertedDataSize}, textureData.Length {textureData.Length}");
                Destroy(texture);
                return null;
            }

            xRCpuImage.Convert(conversionParams, textureData);
            texture.Apply();
            return texture; 
        }


        protected override RemotePacket GetPacketData()
        {
            ServerRemotePacket packet = new ServerRemotePacket();

            packet.cameraPose = arPoseDriver;
            packet.cameraIntrinsics = cameraManager.TryGetIntrinsics(out XRCameraIntrinsics intrinsics) ? new SerializableXRCameraIntrinsics(intrinsics) : null;

            if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage xrCpuImage))
            {
                var byteArray = UpdateToXRCpuImage(xrCpuImage).GetRawTextureData();
                SerializableDepthImage xrDepthImage = new SerializableDepthImage(xrCpuImage, byteArray);
                
                rawImage.texture = occlusionManager.environmentDepthTexture;
                packet.depthImage = xrDepthImage;
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
