//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteCameraProvider.cs" createdby="gblikas">
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

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//#if UNITY_EDITOR

//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;

//public class XRRemoteCameraProvider : XRCameraSubsystem
//{
//    protected override IProvider CreateProvider() => new XRRemoteProvider();

//    private class XRRemoteProvider : IProvider
//    {

//        public XRRemoteProvider()
//        {

//        }

//        public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
//        {
//            ARRemote.XREditorClient connection = ARRemote.XREditorClient.Instance;
//            if (connection == null)
//            {
//                cameraFrame = default(XRCameraFrame);
//                return false;
//            }


//            ARRemote.CameraFrameEvent remoteFrame = connection.xrRemoteCameraFrameEvent; 
//            if (remoteFrame.timestampNs == default(long))
//            {
//                cameraFrame = default(XRCameraFrame);
//                return false;
//            }

//            const XRCameraFrameProperties properties =
//                XRCameraFrameProperties.Timestamp
//                | XRCameraFrameProperties.ProjectionMatrix
//                | XRCameraFrameProperties.DisplayMatrix;

//            cameraFrame = new ARRemote.CameraFrame()
//            {
//                timestampNs = remoteFrame.timestampNs,
//                averageBrightness = 0,
//                averageColorTemperature = 0,
//                colorCorrection = default(Color),
//                projectionMatrix = remoteFrame.projectionMatrix,
//                displayMatrix = remoteFrame.displayMatrix,
//                trackingState = TrackingState.Tracking,
//                nativePtr = default(System.IntPtr),
//                properties = properties,
//                averageIntensityInLumens = 0,
//                exposureDuration = 0,
//                exposureOffset = 0
//            };
//            return true;
//        }
//    }
//}
//#endif 