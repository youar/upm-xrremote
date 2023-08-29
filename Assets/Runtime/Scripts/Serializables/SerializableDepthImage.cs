//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDepthImage.cs" createdby="razieleron">
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
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

namespace XRRemote.Serializables 
{
    [Serializable]
    public class SerializableDepthImage
    {
        public byte[] texData;
        public int width;
        public int height;
        public int planeCount;
        
        public Format format; 

        public SerializableDepthImage(XRCpuImage depthImage)
        {
            width = depthImage.width;
            height = depthImage.height;
            format = depthImage.format;
            planeCount = depthImage.planeCount;
            texData = null;
        }

        public SerializableDepthImage(XRCpuImage depthImage, byte[] byteArray)
        {
            width = depthImage.width;
            height = depthImage.height;
            format = depthImage.format;
            planeCount = depthImage.planeCount;
            texData = byteArray;
        }

        public void UpdateTexData(byte[] byteArray)
        {
            texData = byteArray;
        }
    }
}

