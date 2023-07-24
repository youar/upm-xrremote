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
/*using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR;
using XRRemote;
using UnityEngine.Rendering;

namespace XRRemote
{
    public abstract class CustomNdiReceiver : MonoBehaviour
    {
        [SerializeField] 
        private NdiResources resources = null;
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
            var ndiName = FindNdiName();
            if (!string.IsNullOrWhiteSpace(ndiName))
            {
                ndiReceiver.ndiName = ndiName;
            } 
            else 
            {
                if (DebugFlags.displayXRRemoteConnectionStats) 
                {
                    Debug.LogError($"Can't connect to " + targetNdiSenderName);
                }                
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
                else 
                {
                    if (DebugFlags.displayXRRemoteConnectionStats) 
                    {
                        Debug.LogError($"Can't connect to " + targetNdiSenderName);
                    } 
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
        
        private string FindNdiName()
        {
            string returnedName = NdiFinder.sourceNames.FirstOrDefault(s => s.Contains(targetNdiSenderName));
            
            return returnedName;
        }
    }
}
*/



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
using UnityEngine.Rendering;

namespace XRRemote
{
    public abstract class CustomNdiReceiver : MonoBehaviour
    {
        [SerializeField] 
        private NdiResources resources = null;
        protected NdiReceiver ndiReceiver = null;
        public CustomRawImage rawImage = null;
        protected string targetNdiSenderName = "CustomNdiSender";
        [SerializeField] private Camera receivingCamera;
        private CommandBuffer videoCommandBuffer;
        private bool videoCommandBufferInitialized = false;
        private Material mat;
        [SerializeField] protected Text receiverNameText;
        [SerializeField] protected Text debugText;   
        
        
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
            else 
            {
                if (DebugFlags.displayXRRemoteConnectionStats) 
                {
                    Debug.LogError($"Can't connect to " + targetNdiSenderName);
                }                
            }
            // InitializeCommandBuffer();

        }
        // private void OnDestroy()
        // {
        //     if (videoCommandBuffer != null)
        //     {
        //          receivingCamera.RemoveCommandBuffer(
        //             CameraEvent.BeforeForwardOpaque,
        //             videoCommandBuffer);
        //     }
        //     videoCommandBufferInitialized = false;
        //     // Camera.onPreRender -= OnPreRender;
        // }

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
                else 
                {
                    if (DebugFlags.displayXRRemoteConnectionStats) 
                    {
                        Debug.LogError($"Can't connect to " + targetNdiSenderName);
                    } 
                }
            }
            else
            {
                //add texture to rawImage
                // rawImage.texture = rt;
                // mat.SetTexture("_MainTex",rt);
                // Debug.Log("setting Tex");
                CommandBufferDebugging.Instance.texture = rt;

                // CommandBufferImageDebug.material.mainTexture = rt;
                

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
        
        private string FindNdiName()
        {
            string returnedName = NdiFinder.sourceNames.FirstOrDefault(s => s.Contains(targetNdiSenderName));
            
            return returnedName;
        }

        // private void InitializeCommandBuffer()
        // {
        //     if (videoCommandBufferInitialized) return;
        //     Debug.Log("init buffer");
        //     videoCommandBuffer = new CommandBuffer();
        //     mat = new Material(Shader.Find("Standard"));
        //     videoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, mat);
        //     receivingCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, videoCommandBuffer);
        //     videoCommandBufferInitialized = true;
        //     // Camera.onPreRender -= OnPreRender;
        //     // Camera.onPreRender += OnPreRender;
        // }

        // public void OnPreRender()
        // {
        
        //     if (!videoCommandBufferInitialized) InitializeCommandBuffer();
        //     mat.SetTexture("_MainTex", ndiReceiver.texture);
     
        // }
      


    }
}
