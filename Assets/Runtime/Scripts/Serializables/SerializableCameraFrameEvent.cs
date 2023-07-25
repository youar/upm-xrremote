//-------------------------------------------------------------------------------------------------------
// <copyright file="SerializableCameraFrameEvent.cs" createdby="gblikas">
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

namespace XRRemote.Serializables 
{
    [Serializable]
    public class SerializableCameraFrameEvent : IEquatable<SerializableCameraFrameEvent>
    {
        // public ARLightEstimationData lightEstimation;
        public long timestampNs;
        public SerializablePose projectionMatrix;
        public SerializablePose displayMatrix;

        public bool Equals(SerializableCameraFrameEvent o)
        {
            return timestampNs.Equals(o.timestampNs)
                && projectionMatrix.Equals(o.projectionMatrix)
                && displayMatrix.Equals(o.displayMatrix);
        }

        public override string ToString()
        {
            return $"[time: {timestampNs}, projection: {projectionMatrix}, display: {displayMatrix}]";
        }

        // public static int DataSize => sizeof(long) + Marshal.SizeOf(typeof(Matrix4x4)) * 2;
    }
}
