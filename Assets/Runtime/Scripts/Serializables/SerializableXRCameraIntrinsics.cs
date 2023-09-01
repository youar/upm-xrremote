//------------------------------------------------------------------------------------------------------- 
// <copyright file="SerializableXRCameraIntrinsics.cs" createdby="razieleron"> //  
// XR Remote // Copyright(C) 2020  YOUAR, INC. // 
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU Affero General Public License as published by 
// the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version. // 
// https://www.gnu.org/licenses/agpl-3.0.html // 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the 
// GNU Affero General Public License for more details. 
// You should have received a copy of the GNU Affero General Public License 
// along with this program. If not, see // <http://www.gnu.org/licenses/>. // 
// </copyright> 
//-------------------------------------------------------------------------------------------------------  
using System;
using UnityEngine.XR.ARSubsystems;
namespace XRRemote.Serializables

{
    [Serializable]
    public class SerializableXRCameraIntrinsics : IEquatable<SerializableXRCameraIntrinsics>, IFormattable
    {
        public SerializableFloat2 focalLength;
        public SerializableFloat2 principalPoint;
        public SerializableVector2Int resolution;

        /// <summary>XRCameraIntrinsics zero value.</summary>         
        public static readonly SerializableXRCameraIntrinsics zero;

        public SerializableXRCameraIntrinsics(SerializableFloat2 focalLength, SerializableFloat2 principalPoint, SerializableVector2Int resolution)
        {
            this.focalLength = focalLength;
            this.principalPoint = principalPoint;
            this.resolution = resolution;
        }

        public SerializableXRCameraIntrinsics(XRCameraIntrinsics intrinsics)
        {
            this.focalLength = new SerializableFloat2(intrinsics.focalLength);
            this.principalPoint = new SerializableFloat2(intrinsics.principalPoint);
            this.resolution = new SerializableVector2Int(intrinsics.resolution);
        }

        public bool Equals(SerializableXRCameraIntrinsics rhs) 
        { 
            return focalLength == rhs.focalLength && principalPoint == rhs.principalPoint && resolution == rhs.resolution; 
        }

        public string ToString(string format, IFormatProvider formatProvider) 
        { 
            return string.Format("XRCameraIntrinsics({0}f, {1}f, {2}f, {3}f, {4}f, {5}f, {6}f)", 
                    focalLength.x.ToString(format, formatProvider), 
                    focalLength.y.ToString(format, formatProvider), 
                    principalPoint.x.ToString(format, formatProvider), 
                    principalPoint.y.ToString(format, formatProvider), 
                    resolution.x.ToString(format, formatProvider), 
                    resolution.y.ToString(format, formatProvider)); 
        }
        
        public XRCameraIntrinsics ToXRCameraIntrinsics() 
        { 
            return new XRCameraIntrinsics(focalLength.ConvertFromSerializableFloat2ToVector2(), 
                                        principalPoint.ConvertFromSerializableFloat2ToVector2(), 
                                        resolution.ConvertFromSerializableVector2IntToVector2Int()); 
        }
    }
}