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
using System.Collections;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public class ClientSender : CustomNdiSender
    {    
        // [SerializeField] public Camera uiCamera = null;
        public static ClientSender Instance { get; private set; }

        //exists just for testing UI image
        [SerializeField] private Material renderMaterial;
        [SerializeField] private RenderTexture uiRenderTexture;

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

        private void OnEnable()
        {
            StartCoroutine(SendData());
        }

        private void OnDisable()
        {
            StopCoroutine(SendData());
        }

        protected override Material GetCameraFrameMaterial()
        {
            renderMaterial.mainTexture = uiRenderTexture;
            return renderMaterial;
        }

        protected override RemotePacket GetPacketData()
        {
            ClientRemotePacket packet = new ClientRemotePacket();
            if (XRRemoteImageManager.Instance.serializedLibrary != null)
            {
                Debug.Log("Sending image library..."); //[delete]
                packet.referenceImageLibraryTextures = XRRemoteImageManager.Instance.serializedLibrary;
            } 
            if (UIRenderer.Instance != null) packet.debugMode = UIRenderer.Instance.debugMode;
            return packet;
        }

        private IEnumerator SendData()
        {
            while (true)
            {           
                    
                yield return new WaitForSeconds(0.1f);     
                Debug.Log("Sending data...");    
                OnCameraFrameReceived();
            }
        }
        
    }
}
