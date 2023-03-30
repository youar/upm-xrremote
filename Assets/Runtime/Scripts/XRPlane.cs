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
using UnityEngine.XR.ARFoundation;

namespace XRRemote
{
    //using UnityEngine.XR.ARSubsystems;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XRPlane : IEquatable<XRPlane>
    {
        public ARKitStream.Internal.TrackableId trackableId;
        public Pose pose;
        public float3 center;
        public float3 centerInPlaneSpace;
        public float3 normal;
        public int trackingState;
        public float vertexChangedThreshold;
        public float2[] boundary;
        public float2 size;
        public bool isSubsumed;

        public XRPlane(ARPlane arPlane)
        {
            trackableId = arPlane.trackableId;
            pose = Pose.FromTransform(arPlane.transform);
            center = new float3(arPlane.center);
            centerInPlaneSpace = new float3(arPlane.centerInPlaneSpace);
            normal = new float3(arPlane.normal);
            trackingState = (int)arPlane.trackingState;
            vertexChangedThreshold = arPlane.vertexChangedThreshold;
            size = new float2(arPlane.size);
            isSubsumed = (arPlane.subsumedBy != null);

            //Save the boundary array as a float2 array
            Vector2[] boundaryPoints = arPlane.boundary.ToArray();
            boundary = new float2[boundaryPoints.Length];
            for (int j = 0; j < boundaryPoints.Length; j++) {
                boundary[j] = new float2(arPlane.boundary[j]);
            }
        }

        public bool Equals(XRPlane o)
        {
            return trackableId.Equals(o.trackableId);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[XRPlane] id: {trackableId} ");
            sb.Append($"pose: {pose} ");
            sb.Append($"center: {center} ");
            sb.Append($"state: {trackingState} ");
            sb.Append($"vertexChangedThreshold: {vertexChangedThreshold} ");
            sb.Append($"boundary: {boundary} ");

            return sb.ToString();
        }
    }
}
