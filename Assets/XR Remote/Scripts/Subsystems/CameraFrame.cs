//-------------------------------------------------------------------------------------------------------
// <copyright file="CameraFrame.cs" createdby="gblikas">
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

	[StructLayout(LayoutKind.Sequential)]
    public struct CameraFrame : IEquatable<CameraFrame>
    {
        public long timestampNs;
        public float averageBrightness;
        public float averageColorTemperature;
        public Color colorCorrection;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 displayMatrix;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public XRCameraFrameProperties properties;
        public float averageIntensityInLumens;
        public double exposureDuration;
        public float exposureOffset;


        [StructLayout(LayoutKind.Explicit)]
        public struct CameraFrameUnion
        {
            [FieldOffset(0)] public CameraFrame a;
            [FieldOffset(0)] public XRCameraFrame b;
        }

        public bool Equals(CameraFrame o)
        {
            return timestampNs.Equals(o.timestampNs)
                && averageBrightness.Equals(o.averageBrightness)
                && averageColorTemperature.Equals(o.averageColorTemperature)
                && colorCorrection.Equals(o.colorCorrection)
                && projectionMatrix.Equals(o.projectionMatrix)
                && displayMatrix.Equals(o.displayMatrix)
                && trackingState.Equals(o.trackingState)
                && nativePtr.Equals(o.nativePtr)
                && properties.Equals(o.properties)
                && averageIntensityInLumens.Equals(o.averageIntensityInLumens)
                && exposureDuration.Equals(o.exposureDuration)
                && exposureOffset.Equals(o.exposureOffset);
        }

        public static implicit operator XRCameraFrame(CameraFrame f)
        {
            var union = new CameraFrameUnion()
            {
                a = f,
            };
            return union.b;
        }

        public static implicit operator CameraFrame(XRCameraFrame f)
        {
            var union = new CameraFrameUnion()
            {
                b = f,
            };
            return union.a;
        }
    }
}