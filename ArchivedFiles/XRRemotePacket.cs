//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemotePacket.cs" createdby="gblikas">
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

namespace XRRemote
{
    using UnityEngine.XR.ARSubsystems;

    [Serializable]
    public class XRFrameReadyPacket {
        public bool value;
    }
    
    [Serializable]
    public class ARSessionHandShakePacket {
        public bool value;
    }
    
    [Serializable]
    public class EditorARKitSessionInitialized {
        public bool value;
    }

    [Serializable]
    public class XRRemotePacket
    {
        public CameraFrameEvent cameraFrame = new CameraFrameEvent();

        public FaceInfo face = new FaceInfo();

        public XRRemote.Pose trackedPose = new XRRemote.Pose();

        public XRRemote.SerializeableTexture2D texture;

        public PlanesInfo planesInfo = new PlanesInfo(); 

        public float3 touchPosition = null;
        public float3 touchPositionNormalized = null;
    }



    [Serializable]
    public class FaceMesh
    {
        public TrackableId id;
        public byte[] vertices; // NativeArray<Vector3>
        public byte[] normals; // NativeArray<Vector3>
        public byte[] indices; // NativeArray<int>
        public byte[] uvs;    // NativeArray<Vector2>
        public byte[] coefficients;

        public override string ToString()
        {
            return $"Mesh {id} verts: {vertices.Length} norms: {normals.Length} indices: {indices.Length} uvs: {uvs.Length}";
        }
    }

    [Serializable]
    public class FaceInfo
    {
        public XRFace[] added;
        public XRFace[] updated;
        public TrackableId[] removed;
        public FaceMesh[] meshes;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("FaceInfo");
            foreach (var f in added)
            {
                sb.AppendLine($"ADD: {f}");
            }
            foreach (var f in updated)
            {
                sb.AppendLine($"UPDATE: {f}");
            }
            foreach (var f in removed)
            {
                sb.AppendLine($"REMOVE: {f}");
            }
            foreach (var m in meshes)
            {
                sb.AppendLine($"MESHED: {m}");
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public class PlanesInfo
    {
        public XRPlane[] added;
        public XRPlane[] updated;
        public XRPlane[] removed;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("PlanesInfo");
            foreach (var f in added)
            {
                sb.AppendLine($"added: {f}");
            }
            foreach (var f in updated)
            {
                sb.AppendLine($"updated: {f}");
            }
            foreach (var f in removed)
            {
                sb.AppendLine($"removed: {f}");
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public class SerializableRenderTexture : IEquatable<SerializableRenderTexture>
    {
        [SerializeField]
        public RenderTexture renderTexture;

        public SerializableRenderTexture(SerializableRenderTexture serializableRenderTexture)
        {
            this.renderTexture = serializableRenderTexture.renderTexture;
        }

        public bool Equals(SerializableRenderTexture other)
        {
            return renderTexture.Equals(other.renderTexture);
        }
    }

    [Serializable]
    public class SerializeableTexture2D : IEquatable<SerializeableTexture2D>
    {
        public int width;

        public int height;

        public byte[] rawTextureData;

        [SerializeField]
        public UnityEngine.TextureFormat format;

        public SerializeableTexture2D(int width, int height, UnityEngine.TextureFormat textureFormat)
        {
            this.width = width;
            this.height = height;
            this.format = textureFormat;
            this.rawTextureData = null;
        }
        public SerializeableTexture2D(int width, int height, byte[] rawTexture, UnityEngine.TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            this.width = width;
            this.height = height;
            this.rawTextureData = rawTexture;
            this.format = textureFormat;
        }

        public bool Equals(SerializeableTexture2D other)
        {
            return rawTextureData.Equals(other)
                && format.Equals(other.format)
                && width == other.width
                && height == other.height;
        }

        public static UnityEngine.Texture2D FromSerializeableTexture2D(XRRemote.SerializeableTexture2D arRemoteTexture2D)
        {
            UnityEngine.Texture2D texture2D = new UnityEngine.Texture2D(
                arRemoteTexture2D.width,
                arRemoteTexture2D.height);
            if (arRemoteTexture2D.rawTextureData != null && arRemoteTexture2D.rawTextureData.Length != 0)
            {
                texture2D.LoadImage(arRemoteTexture2D.rawTextureData);
                texture2D.Apply();
            }
            else
            {
                Debug.LogFormat(
                    $"(width, height, Lenght ) " +
                    $"({arRemoteTexture2D.width}, {arRemoteTexture2D.height}, {arRemoteTexture2D.rawTextureData.Length})");
            }
            return texture2D;
        }
    }

}