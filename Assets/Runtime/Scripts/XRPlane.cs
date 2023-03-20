//-------------------------------------------------------------------------------------------------------
// <copyright file="XRPlane.cs" createdby="gblikas">
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


namespace XRRemote
{
    using UnityEngine.XR.ARSubsystems;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XRPlane : IEquatable<XRPlane>
    {
        //public TrackableId trackableId;
        public Pose pose;
        public float3 center;
        public float3 centerInPlaneSpace;
        public float3 normal;
        public int trackingState;
        public float vertexChangedThreshold;
        public float2[] boundary;


        public bool Equals(XRPlane o)
        {
            return  center.Equals(o.center)
                //&& trackableId.Equals(o.trackableId)
                && pose.Equals(o.pose)
                && normal.Equals(o.normal)
                && trackingState.Equals(o.trackingState)
                && vertexChangedThreshold.Equals(o.vertexChangedThreshold)
                && boundary.Equals(o.boundary);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            //sb.Append($"[XRPlane] id: {trackableId} ");
            sb.Append($"pose: {pose} ");
            sb.Append($"center: {center} ");
            sb.Append($"state: {trackingState} ");
            sb.Append($"vertexChangedThreshold: {vertexChangedThreshold} ");
            sb.Append($"boundary: {boundary} ");

            return sb.ToString();
        }
    }
}
