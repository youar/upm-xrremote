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

namespace XRRemote
{
    public class ClientReceiver : CustomNdiReceiver
    {
        public static ClientReceiver Instance { get; private set; } = null;
        public ServerRemotePacket remotePacket { get; private set; } = null;
        public event EventHandler OnPlanesInfoReceived;

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
            TrySetupTrackedPoseDriver();
        }


        protected override void ProcessPacketData(byte[] bytes)
        {
            ServerRemotePacket remotePacket = ObjectSerializationExtension.Deserialize<ServerRemotePacket>(bytes);
            this.remotePacket = remotePacket;
            PlanesInfoCheck(remotePacket);
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
        
        private bool TrySetupTrackedPoseDriver()
        {
            TrackedPoseDriver trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();
            if (trackedPoseDriver == null) {
                    Debug.LogErrorFormat("TrySetupTrackedPoseDriver Event: null TrackedPoseDriver on main camera");
                    return false;
            }

            if (TryGetComponent<XRRemotePoseProvider>(out XRRemotePoseProvider remotePoseProvider))
            {
                trackedPoseDriver.poseProviderComponent = remotePoseProvider;
                return true;
            }

            Debug.LogErrorFormat("TrySetupTrackedPoseDriver Event: null XRRemotePoseProvider on Ndi receiver");
            return false;
        }
    }
}
