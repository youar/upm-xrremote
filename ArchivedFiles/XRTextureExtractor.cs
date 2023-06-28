//-------------------------------------------------------------------------------------------------------
// <copyright file="XRTextureExtractor.cs" createdby="gblikas">
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
    public class XRTextureExtractor : MonoBehaviour
    {
        private static XRTextureExtractor _instance;
        public static XRTextureExtractor instance
        {
            get
            {
                if(_instance == null)
                    _instance = FindObjectOfType<XRTextureExtractor>();
                return _instance;
            }
        }

        public int width { get; private set; }
        public int height { get; private set; }

        public Texture texture { get; private set; }

        private string XRRemoteExtractionMessage(string baseMessage)
        {
            return string.Format("XRTextureExtractor {0}", baseMessage);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);

            if(texture == null) {
                width = source.width;
                height = source.height;
                texture = new RenderTexture(width, height, 0);

                if (DebugFlags.displayXRExtractTextureStats)
                {
                    Debug.LogFormat(
                        XRRemoteExtractionMessage(
                            string.Format("OnRenderImage: create texture [{0},{1}]", width, height)));
                }
            }

            Graphics.Blit(source, texture as RenderTexture);
        }





    }
}