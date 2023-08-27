//-------------------------------------------------------------------------------------------------------
// <copyright file="ClientSender.cs" createdby="gblikas">
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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using XRRemote.Serializables;
using System.Collections;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public sealed class ClientSender : CustomNdiSender
    {    
        [SerializeField] 
        public Camera uiCamera = null;
        public static ClientSender Instance { get; private set; }

        //exists just for testing UI image
        public Material renderMaterial;

        private XRRemotePlaneManager planeManager = null;

        private void Awake()
        {
            // It works only in Editor!
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("Cannot use ClientSender in non-Editor environment.");
                return;
            }

            if (ClientSender.Instance != null)
            {
                Debug.LogError("Only 1 Ndi Sender may exist in the scene.");
                return;
            }

            ClientSender.Instance = this;
            ndiSenderName = "ClientSender";
        }

        protected override void Start()
        {
            base.Start();
            ClientSender.Instance.OnInitNdi += ClientSender_OnNdiInitialized;
            StartCoroutine(SendData());
           
           TrySetupPlaneManager();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClientSender.Instance.OnInitNdi -= ClientSender_OnNdiInitialized;
            StopCoroutine(SendData());
        }


        private void OnValidate()
        {
            if (uiCamera == null)
            {
                //may pull wrong camera if multiple in scene?? refine later
                uiCamera = FindAnyObjectByType<Camera>();
            }
        } 

        private void ClientSender_OnNdiInitialized(object sender, EventArgs e)
        {
            //set ui camera to render to NDI Init texture
            uiCamera.targetTexture = renderTexture;
        }

        protected override Material GetCameraFrameMaterial()
        {
            renderMaterial.mainTexture = new RenderTexture(200, 200, 0, RenderTextureFormat.ARGB32);
            return renderMaterial;
        }

        protected override RemotePacket GetPacketData()
        {
            ClientRemotePacket packet = new ClientRemotePacket();
            packet.debugMode = UIRenderer.Instance.debugMode;

            if (planeManager != null) {
                packet.planeManagerState = planeManager.State;
            } 

            return packet;
        }

        private bool TrySetupPlaneManager()
        {
            if (planeManager != null) return true;

            planeManager = FindAnyObjectByType<XRRemotePlaneManager>();
            if (planeManager == null) {
                if (DebugFlags.displayXRRemotePlaneStats) {
                    Debug.LogWarning($"ClientSend: Unable to find XRRemotePlaneManager. Please make sure there is one in the scene if you want planes info.");
                }
                return false;
            }

            return true;
        }


        //[review] slow update on server side.... something to do with this??
        private IEnumerator SendData()
        {
            while (true)
            {
                OnCameraFrameReceived();
                yield return new WaitForSeconds(0.1f);        
            }
        }
    }
}
