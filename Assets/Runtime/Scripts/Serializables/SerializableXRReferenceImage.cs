//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableXRReferenceImage.cs" createdby="cSustrich">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEditor;

namespace XRRemote.Serializables 
{
    [Serializable]
    public struct XRInfo
    {
        public string name;
        public string guid;
        public bool specifySize;
        public SerializableFloat2 size;
    }

    [Serializable]
    public class SerializableXRReferenceImage
    {
        public string texName;
        public string guid;
        public byte[] texData;
        public SerializableFloat2 texSize;
        public bool sizeSpecified;
        public SerializableFloat2 realSize;
        public string texFormat;

        public SerializableXRReferenceImage(XRReferenceImage image)
        {
#if UNITY_EDITOR

            texName = image.name;
            guid = image.textureGuid.ToString().Replace("-", "");;

            Texture2D tex = null;

            if (image.texture != null) tex = image.texture;
            else tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));

            if (tex == null)
            {
                Debug.LogError($"Failed to load texture {image.textureGuid} for image {image.name}.");
                texData = null;
                texSize = null;
                texFormat = null;
            }
            else
            {
                texData = tex.GetRawTextureData();
                texSize = new SerializableFloat2(tex.width, tex.height); 
                texFormat = tex.format.ToString();
            }

            sizeSpecified = image.specifySize;

            if (sizeSpecified)
            {
                realSize = new SerializableFloat2(image.size.x, image.size.y);
            }
            else
            {
                realSize = null;
            }
#else
            throw new Exception("Creation of SerializableTexture2D is editor only.");
#endif
        }


        public Texture2D ConvertFromSerializableXRReferenceImageToTexture2D(out XRInfo xrInfo)
        {
            Debug.Log("Reconstructing texture from serializable texture.");
            Texture2D tex = new Texture2D((int)texSize.x, (int)texSize.y, (TextureFormat)Enum.Parse(typeof(TextureFormat), texFormat), true);

            tex.LoadRawTextureData(texData);
            tex.Apply();

            xrInfo = new XRInfo();
            xrInfo.name = texName;
            xrInfo.guid = guid;
            xrInfo.specifySize = sizeSpecified;
            if (sizeSpecified)
            {
                xrInfo.size = realSize;
            }
            else xrInfo.size = null;


            return tex;
        }

    }
}
