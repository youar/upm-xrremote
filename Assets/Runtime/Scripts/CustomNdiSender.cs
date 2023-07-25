//-------------------------------------------------------------------------------------------------------
// <copyright file="CustomNdiSender.cs" createdby="gblikas">
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
using UnityEngine.UI;
// using XRRemote.Serializables;

namespace XRRemote
{   
    [DisallowMultipleComponent]
    [Serializable]

    public abstract class CustomNdiSender : MonoBehaviour
    {    
        [SerializeField] 
        protected NdiResources resources = null;
        private int frameCount = 0;
        public Image ndiSenderVisualizer = null;
        protected NdiSender ndiSender = null;
        protected RenderTexture renderTexture;
        protected CommandBuffer commandBuffer;
        protected string ndiSenderName = "CustomNdiSender";
        public event EventHandler OnInitNdi;

        protected virtual void Start()
        {
            frameCount = 0;
            commandBuffer = new CommandBuffer();
            commandBuffer.name = ndiSenderName;
        }

        protected virtual void OnDestroy()
        {
            frameCount = 0;
            commandBuffer?.Dispose();
        }

        private void SetTexture(int width, int height)
        {
            if (renderTexture == null)
            {
                //set texture
                InitNdi(width, height);
            }
        }

        private string SerializeMetadata(RemotePacket packet)
        {
            byte[] serializedData = ObjectSerializationExtension.SerializeToByteArray(packet); 
            return "<![CDATA[" + Convert.ToBase64String(serializedData) + "]]>";
        }

        protected abstract RemotePacket GetPacketData();
        
        protected void CommandBufferActions(Material background)
        {
            commandBuffer.Blit(null, renderTexture, background);
            Graphics.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear(); 
        }

        protected abstract Material GetCameraFrameMaterial();

        protected void OnCameraFrameReceived()
        {
            Material material = GetCameraFrameMaterial();
            SetTexture(material.mainTexture.width, material.mainTexture.height);
   
            ndiSender.metadata = SerializeMetadata(GetPacketData());

            CommandBufferActions(material);
            frameCount++;
        } 

        protected void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            OnCameraFrameReceived();
        } 

        private void InitNdi(int width, int height)
        {
            Debug.Log($"Init NDI width: {width} height: {height}");
            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            // var name = string.Format("CustomNdiSender");
            var go = new GameObject(ndiSenderName);
            go.transform.SetParent(transform, false);
            ndiSender = go.AddComponent<NdiSender>();
            ndiSender.SetResources(resources);
            ndiSender.captureMethod = CaptureMethod.Texture;
            ndiSender.keepAlpha = false;
            ndiSender.ndiName = ndiSenderName;
            ndiSender.sourceTexture = renderTexture;

            if (ndiSenderVisualizer != null)
            {
                ndiSenderVisualizer.material.mainTexture = renderTexture;
            }
            OnInitNdi?.Invoke(this, EventArgs.Empty);
        }
    }
}
