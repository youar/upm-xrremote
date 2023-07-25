//-------------------------------------------------------------------------------------------------------
// <copyright file="CustomNdiReceiver.cs" createdby="gblikas">
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
using UnityEngine;
using Klak.Ndi;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace XRRemote
{
    public abstract class CustomNdiReceiver : MonoBehaviour
    {
        [SerializeField] private NdiResources resources = null;
        protected NdiReceiver ndiReceiver = null;
        public CustomRawImage rawImage = null;
        protected string targetNdiSenderName = "CustomNdiSender";

        [SerializeField] protected Text receiverNameText;
        [SerializeField] protected Text debugText;   
        
        
        [Tooltip("Aspect Ratio or Pixel Count of the Mobile Device (Width/Height)")]
        public float aspectRatio;

        protected virtual void Start()
        {
            ndiReceiver = gameObject.AddComponent<NdiReceiver>();
            ndiReceiver.SetResources(resources);
            ConnectToNdi();
        }
        

        private void Update()
        {

            RenderTexture rt = ndiReceiver.texture;
            if (rt == null)
            {
                ConnectToNdi();
            }
            else
            {
                ReceiveTexture(rt);
                if (!MetadataNullCheck())
                {
                    ProcessPacketData(DeserializePacket());
                    NullMetadata();
                }
            }
        }

        protected abstract void ReceiveTexture(RenderTexture texture);
        protected abstract void ProcessPacketData(byte[] data);


        private bool MetadataNullCheck()
        {
            return (ndiReceiver.metadata == null);
        }

        private void NullMetadata()
        {
            ndiReceiver.metadata = null;
        }

        private byte[] DeserializePacket()
        {
            string base64 = ndiReceiver.metadata.Substring(9, ndiReceiver.metadata.Length - 9 - 3);
            byte[] data = Convert.FromBase64String(base64); 
            return data;
        }
        
        private string FindNdiName()
        {
            string returnedName = NdiFinder.sourceNames.FirstOrDefault(s => s.Contains(targetNdiSenderName));
            return returnedName;
        }

        private void ConnectToNdi()
        {
            string ndiName = FindNdiName();
            
            if (!string.IsNullOrWhiteSpace(ndiName) && ndiReceiver.ndiName != ndiName)
            {
                ndiReceiver.ndiName = ndiName;
                receiverNameText.text = ndiName;
            } 
            else 
            {
                if (DebugFlags.displayXRRemoteConnectionStats) 
                {
                    Debug.LogError($"Can't connect to " + targetNdiSenderName);
                } 
            }
        }

    }
}
