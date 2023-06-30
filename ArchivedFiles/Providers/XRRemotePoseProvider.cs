//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemotePoseProvider.cs" createdby="gblikas">
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

using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

public class XRRemotePoseProvider : BasePoseProvider
{
    public override PoseDataFlags GetPoseFromProvider(out Pose output)
    {
        XRRemote.XREditorClient connection = XRRemote.XREditorClient.Instance;
        if (connection == null)
        {
            output = Pose.identity;
            return PoseDataFlags.NoData;
        }

        XRRemote.Pose pose = connection.xrRemoteTrackedPose;
        if (pose == null)
        {
            output = Pose.identity;
            return PoseDataFlags.NoData;
        }

        output = pose;
        return PoseDataFlags.Position | PoseDataFlags.Rotation; 
    }
}
#endif 
