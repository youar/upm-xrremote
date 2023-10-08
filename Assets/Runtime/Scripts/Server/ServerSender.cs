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
using XRRemote.Input;

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
        [SerializeField] private XRRemoteInputReader inputReader = null;

        private int packetsSent = 0;

        private PlaneManagerState clientPlaneManagerState = PlaneManagerState.InActive;
     
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

            TrySetUpInputReader();
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

            packet.packetId = packetsSent++;
            packet.frameInfo = Time.frameCount;
            
            packet.cameraPose = arPoseDriver;
            
            packet.cameraIntrinsics = cameraManager.TryGetIntrinsics(out XRCameraIntrinsics intrinsics) ? new SerializableXRCameraIntrinsics(intrinsics) : null;

            packet.planesInfo = GetPlanesInfoFromSender();

            if (inputReader.TryGetLastInputNormalized(out Vector2 touchPositionNormalized)) {
                packet.touchPositionNormalized = new SerializableFloat2(touchPositionNormalized);
            } else {
                packet.touchPositionNormalized = null;
            }

            return packet;
        }

        protected override Material GetCameraFrameMaterial()
        {
            return cameraBackground.material;
        }

        public void SetPlaneManagerState(PlaneManagerState state)
        {
            clientPlaneManagerState = state;
        }

        private bool TrySetUpInputReader()
        {
            if (inputReader != null) return true;

            inputReader = FindObjectOfType<XRRemoteInputReader>();
            if (inputReader != null) return true;

            if (DebugFlags.displayXRRemoteConnectionStats) {
               Debug.LogError(string.Format($"{gameObject.name}: XRRemoteInputReader not found"));
            }
            return false;
        }

        private SerializablePlanesInfo GetPlanesInfoFromSender()
        {
            if (clientPlaneManagerState == PlaneManagerState.WaitingForHandshake) {
                if (planeSender.TryGetAllPlanesInfo(out SerializablePlanesInfo planesInfo)) {
                    return planesInfo;
                }
            } else if (clientPlaneManagerState == PlaneManagerState.ReadyToReceivePlanes) {
                if (planeSender.TryGetPlanesInfo(out SerializablePlanesInfo planesInfo)) {
                    return planesInfo;
                }
            }

            return null;
        }
    }
}
