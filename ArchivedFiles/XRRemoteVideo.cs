//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteVideo.cs" createdby="gblikas">
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    using UnityEngine.Rendering;
    using UnityEngine.Serialization;

    public class XRRemoteVideo : MonoBehaviour
    {

        public static class Keys
        {
            public static class TextureRGBMaterial
            {
                public readonly static string mainTexture = "_MainTex";
            }

            public static class TexutureYUV420Material
            {
                public readonly static string mainTexture = "_MainTex";
                public readonly static string cbcr = "_CRCB";
            }

            public static class TextureYUVMaterial
            {
                public readonly static string mainTexture = "_textureY";
                public readonly static string cbcr = "_textureCbCr";
                public readonly static string displayTransform = "_DisplayTransform";
            }
        }

        /// <summary>
        /// fill out later with sharder variables. 
        /// </summary>
        public class XRRemoteMaterial
        {
            List<string> shaderKeys = new List<string>();

        }

        public enum MaterialType
        {
            YUV,
            YUV420,
            RGB
        }

        public Dictionary<MaterialType, string> materials = new Dictionary<MaterialType, string>()
        {
            {
                MaterialType.YUV,
                "Materials/YUVMaterial"
            },
            {
                MaterialType.YUV420,
                "Materials/YUV420Material"
            },
            {
                MaterialType.RGB,
                "RGBMaterial"
            }
        };

        /// <summary>
        /// Material representing the underlying
        /// video stream, piped to device camera
        /// to which this is attached. 
        /// </summary>
        public Material remoteMaterial;

        /// <summary>
        /// select the given material 
        /// </summary>
        public MaterialType materialType;

        private CommandBuffer m_VideoCommandBuffer;
        private UnityEngine.Texture2D _videoTextureRGB;
        private UnityEngine.Texture2D _videoTextureY;
        private UnityEngine.Texture2D _videoTextureCbCr;
        private Matrix4x4 _displayTransform;

        private bool bCommandBufferInitialized;

        // Start is called before the first frame update
        void Start()
        {
            bCommandBufferInitialized = false;

            if (remoteMaterial == null)
            {
                remoteMaterial = Resources.Load(materials[materialType]) as Material;
                if (remoteMaterial == null)
                {
                    Debug.LogError($"remoteMaterial == null", remoteMaterial);
                }
            }

        }
        void OnDestroy()
        {
            if (m_VideoCommandBuffer != null)
            {
                GetComponent<Camera>().RemoveCommandBuffer(
                    CameraEvent.BeforeForwardOpaque,
                    m_VideoCommandBuffer);
            }
            bCommandBufferInitialized = false;
        }

        void InitializeCommandBuffer()
        {
            m_VideoCommandBuffer = new CommandBuffer();
            m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, remoteMaterial);
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
            bCommandBufferInitialized = true;
        }

        public void OnPreRender()
        {
            if (!bCommandBufferInitialized) { InitializeCommandBuffer(); }

            switch (materialType)
            {
                case MaterialType.YUV:
                    if (_videoTextureY != null)
                        remoteMaterial.SetTexture(Keys.TextureYUVMaterial.mainTexture, _videoTextureY);
                    if (_videoTextureCbCr != null)
                        remoteMaterial.SetTexture(Keys.TextureYUVMaterial.cbcr, _videoTextureCbCr);
                    if (_displayTransform != null)
                        remoteMaterial.SetMatrix(Keys.TextureYUVMaterial.displayTransform, _displayTransform);
                    break;

                case MaterialType.YUV420:
                    if (_videoTextureRGB != null)
                        remoteMaterial.SetTexture(Keys.TexutureYUV420Material.mainTexture, _videoTextureRGB);
                    break;

                case MaterialType.RGB:
                    if (_videoTextureRGB != null)
                        remoteMaterial.SetTexture(Keys.TextureRGBMaterial.mainTexture, _videoTextureRGB);
                    break;

                default:
                    break;
            }
        }


        public void SetRGBTexture(UnityEngine.Texture2D RGBTex)
        {
            if(_videoTextureRGB != null)
            {
                Destroy(_videoTextureRGB); 
            }
            _videoTextureRGB = RGBTex;
        }
        public void SetYTexure(UnityEngine.Texture2D YTex)
        {
            if (_videoTextureY != null)
            {
                Destroy(_videoTextureY);
            }
            _videoTextureY = YTex;
        }
        public void SetUVTexure(UnityEngine.Texture2D UVTex)
        {
            if (_videoTextureCbCr != null)
            {
                Destroy(_videoTextureCbCr);
            }
            _videoTextureCbCr = UVTex;
        }
        public void SetDisplayMatrix(UnityEngine.Matrix4x4 matrix4X4)
        {
            _displayTransform = matrix4X4;
        }
    }
}