//-------------------------------------------------------------------------------------------------------
// <copyright file="XRTextureHelper.cs" createdby="gblikas">
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{

    public static class XRTextureHelper
    {
        public static string GetTextureInfo(Texture2D texture2D)
        {
            string info = string.Empty;

            string name =
                string.Format("name: {0}\n", texture2D.name != null ? texture2D.name : string.Empty);
            string textureFormat =
                string.Format("format: {0}\n", texture2D.format.ToString());
            string size =
                string.Format("(width, height): ({0}, {1})\n", texture2D.width, texture2D.height);

            info = string.Format(
                "TextureInfo:\n {0} {1} {2}",
                name, textureFormat, size);

            return info;
        }

        public static void DisplayTextureInfo(Texture2D texture)
        {
            string info = GetTextureInfo(texture);
            Debug.Log(info);
        }



        public static Dictionary<TextureFormat, int> formatDepth = new Dictionary<TextureFormat, int>() {
            {
                TextureFormat.R8,
                1
            },
            {
                TextureFormat.RG16,
                2
            },
            {
                TextureFormat.RGB24,
                3
            },
            {
                TextureFormat.RGBA32,
                4
            },
            {
                TextureFormat.ARGB32,
                4
            }
        };

        public static bool TryGetTextureFromGPU(Texture2D gpuTexutre2D, ref RenderTexture renderTexture, ref Texture2D cpuTexture2D)
        {
            try
            {
                if (gpuTexutre2D == null) return false;
                Graphics.Blit(gpuTexutre2D, renderTexture);

                RenderTexture.active = renderTexture;
                cpuTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                cpuTexture2D.Apply();
                RenderTexture.active = null;
                return true;

            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(
                        string.Format("TryGetTextureFromGPU Event: failure {0}", e.Message));
                return false;
            }
        }

        public static bool TryGetCPUTextureFromTexture(Texture texture, ref Texture2D cpuTexture2D, RenderTexture renderTexture)
        {
            try
            {
                if (texture == null) return false;
                Graphics.Blit(texture, renderTexture);

                RenderTexture.active = renderTexture;
                cpuTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                cpuTexture2D.Apply();
                RenderTexture.active = null;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(
                    string.Format("TryGetCPUTextureFromTexture Event: failure {0}", e.Message));
                return false;
            }
        }

        public static bool CompositeYUV420(Texture2D gpuYTexture2D, Texture2D gpuCRCBTexture2D, ref Texture2D cpuTexture2D, ref RenderTexture renderTexture, Material material)
        {
            try
            {
                material.SetTexture("_Y", gpuYTexture2D);
                material.SetTexture("_CRCB", gpuCRCBTexture2D);

                Graphics.Blit(null, renderTexture, material);

                RenderTexture.active = renderTexture;
                cpuTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                cpuTexture2D.Apply();
                RenderTexture.active = null;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(
                        string.Format("CompositeYUV420 Event: failure {0}", e.Message));
                return false;
            }
        }

        public static bool ReadBackgroundMaterialTexture(string property, ref Texture2D cpuTexture2D, Material material, RenderTexture renderTexture)
        {
            try
            {
                if (material == null)
                    return false;

                Texture2D texture = material.GetTexture(property) as Texture2D;

                Graphics.Blit(texture, renderTexture, material);

                RenderTexture.active = renderTexture;
                cpuTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                cpuTexture2D.Apply();
                RenderTexture.active = null;

                return true;
            }
            catch (Exception e)
            {

                Debug.LogErrorFormat(
                        string.Format("ReadBackgroundMaterialTexture Event: failure {0}", e.Message));
                return false;
            }
        }
    }
}