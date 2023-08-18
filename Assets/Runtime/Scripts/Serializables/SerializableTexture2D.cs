//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableFloat2.cs" createdby="gblikas">
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

namespace XRRemote.Serializables 
{
    [Serializable]
    public class SerializableTexture2D
    {
        public byte[] texData;
        public string texFormat;
        public int width;
        public int height;
    

        public SerializableTexture2D(Texture2D tex)
        {
            texData = tex.GetRawTextureData();
            texFormat = tex.format.ToString();
            width = tex.width;
            height = tex.height;
        }

        public Texture2D ReconstructFromSerializableTexture2D()
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RHalf, false);
            tex.LoadRawTextureData(texData);
            tex.Apply();
            return tex;
        }

        // SerializableTexture2D sTex = new SerializableTexture2D(depthImage);
        // Texture2D tex = sTex.ReconstructFromSerializableTexture2D();
        
    }
}
