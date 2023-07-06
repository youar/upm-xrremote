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
using System.Collections;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR;
using XRRemote;

namespace XRRemote
{
    public abstract class CustomNdiReceiver : MonoBehaviour
    {
        [SerializeField] 
        private NdiResources resources = null;
        private NdiReceiver ndiReceiver = null;
        // public static CustomNdiReceiver Instance = null;
        public CustomRawImage rawImage = null;

        
        [Tooltip("Aspect Ratio or Pixel Count of the Mobile Device (Width/Height)")]
        public float aspectRatio;

        protected virtual void Start()
        {
            ndiReceiver = gameObject.AddComponent<NdiReceiver>();
            ndiReceiver.SetResources(resources);
            var ndiName = FindNdiName();
            if (!string.IsNullOrWhiteSpace(ndiName))
            {
                ndiReceiver.ndiName = ndiName;
            }
        }

      

        private void Update()
        {
            var rt = ndiReceiver.texture;
            if (rt == null)
            {
                var ndiName = FindNdiName();
                if (!string.IsNullOrWhiteSpace(ndiName) && ndiReceiver.ndiName != ndiName)
                {
                    ndiReceiver.ndiName = ndiName;
                }
            }
            else
            {
                //add texture to rawImage
                rawImage.texture = rt;

                if (!MetadataNullCheck())
                {
                    ProcessPacketData(DeserializePacket());
                    NullMetadata();
                }
            }
        }

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
        
        private static string FindNdiName()
        {
            return NdiFinder.sourceNames.FirstOrDefault();
        }
    }
}
