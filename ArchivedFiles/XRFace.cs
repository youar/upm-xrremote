//-------------------------------------------------------------------------------------------------------
// <copyright file="XRFace.cs" createdby="gblikas">
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
    public struct XRFace : IEquatable<XRFace>
    {
        public TrackableId trackableId;
        public Pose pose;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public Pose leftEyePose;
        public Pose rightEyePose;
        public float3 fixationPoint;


        public bool Equals(XRFace o)
        {
            return trackableId.Equals(o.trackableId)
                && pose.Equals(o.pose)
                && trackingState.Equals(o.trackingState)
                && nativePtr.Equals(o.nativePtr)
                && leftEyePose.Equals(o.leftEyePose)
                && rightEyePose.Equals(o.rightEyePose)
                && fixationPoint.Equals(o.fixationPoint);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[XRFace] id: {trackableId} ");
            // sb.Append($"pose: {pose} ");
            sb.Append($"state: {trackingState} ");
            // sb.Append($"ptr: {nativePtr} ");
            // sb.Append($"left: {leftEyePose} ");
            // sb.Append($"right: {rightEyePose} ");
            // sb.Append($"fixa: {fixationPoint} ");

            return sb.ToString();
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct XRFaceUnion
        {
            [FieldOffset(0)] public XRFace a;
            [FieldOffset(0)] public UnityEngine.XR.ARSubsystems.XRFace b;
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.XRFace(XRFace f)
        {
            var union = new XRFaceUnion()
            {
                a = f,
            };
            return union.b;
        }

        public static implicit operator XRFace(UnityEngine.XR.ARSubsystems.XRFace f)
        {
            var union = new XRFaceUnion()
            {
                b = f,
            };
            return union.a;
        }
    }
}
