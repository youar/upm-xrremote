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
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public class ServerSender : CustomNdiSender
    {    
        [SerializeField] private CustomPlaneSender planeSender = null;
        [SerializeField] private ARCameraManager cameraManager = null;
        [SerializeField] private ARPoseDriver arPoseDriver = null;
        [SerializeField] private ARCameraBackground cameraBackground = null;
     
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
