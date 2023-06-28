//-------------------------------------------------------------------------------------------------------
// <copyright file="TextureDescriptor.cs" createdby="gblikas">
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
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace XRRemote.Serializables 
{
    /// <summary>
    /// HACK: Neet the unsafe struct cast
    /// since XRTextureDescriptor is private struct!
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureDescriptor : IEquatable<TextureDescriptor>
    {
        public IntPtr nativeTexture;
        public int width;
        public int height;
        public int mipmapCount;
        public TextureFormat format;
        public int propertyNameId;

        public TextureDescriptor(Texture2D tex, int propertyNameId)
        {
            nativeTexture = tex.GetNativeTexturePtr();
            width = tex.width;
            height = tex.height;
            mipmapCount = tex.mipmapCount;
            format = tex.format;
            this.propertyNameId = propertyNameId;
        }

        public bool Equals(TextureDescriptor other)
        {
            return nativeTexture.Equals(other.nativeTexture)
                && width.Equals(other.width)
                && height.Equals(other.height)
                && mipmapCount.Equals(other.mipmapCount)
                && format.Equals(other.format)
                && propertyNameId.Equals(other.propertyNameId);
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct TextureDescriptorUnion
        {
            [FieldOffset(0)] public TextureDescriptor a;
            [FieldOffset(0)] public XRTextureDescriptor b;
        }


        public static implicit operator XRTextureDescriptor(TextureDescriptor d)
        {
            var union = new TextureDescriptorUnion()
            {
                a = d,
            };
            return union.b;
        }

        public static implicit operator TextureDescriptor(XRTextureDescriptor d)
        {
            var union = new TextureDescriptorUnion()
            {
                b = d,
            };
            return union.a;
        }
    }
}
