//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerReceiver.cs" createdby="gblikas">
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
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;
using XRRemote.Serializables;
using System.Collections.Generic;


namespace XRRemote
{
    public class ServerReceiver : CustomNdiReceiver
    {
        public static ServerReceiver Instance {get; private set;} = null;
        [SerializeField] public ClientRemotePacket remotePacket {get; private set;} = null;
        public List<SerializableXRReferenceImage> serializedTextures {get; private set;}
        public event EventHandler OnImageLibraryReceived;
     
        private void Awake()
        {
            if (Application.isEditor)
            {
                Destroy(gameObject);
                Debug.LogError("cannot use ServerReceiver in Editor.");
                return;
            }

            Instance = this;
            targetNdiSenderName = "ClientSender";
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void ImageLibraryCheck(ClientRemotePacket remotePacket)
        {
            if (remotePacket.referenceImageLibraryTextures != null)
            {
                if (serializedTextures == null) 
                {
                    serializedTextures = remotePacket.referenceImageLibraryTextures;
                    OnImageLibraryReceived?.Invoke(this, EventArgs.Empty);  
                }
            }
        } 

        protected override void ProcessPacketData(byte[] bytes) 
        {
            Debug.Log("ServerReceiver: Processing packet data.");
            ClientRemotePacket remotePacket = ObjectSerializationExtension.Deserialize<ClientRemotePacket>(bytes);
            this.remotePacket = remotePacket;
            ImageLibraryCheck(remotePacket);
        }

        // protected override void ReceiveDepthTexture(Texture2D depthImage)
        // {
        //     return;
        // }
        protected override void ReceiveTexture(RenderTexture texture)
        {
            //eventually, add received UI Overlay Texture actions here
            return;
        }
    }
}
