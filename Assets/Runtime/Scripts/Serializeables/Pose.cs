//-------------------------------------------------------------------------------------------------------
// <copyright file="Pose.cs" createdby="gblikas">
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
using UnityEngine.XR.ARFoundation;

namespace XRRemote.Serializables 
{

    using UnityEngine.SpatialTracking;

    [Serializable]
    public class Pose : IEquatable<Pose>
    {
        public float3 position;
        public float4 rotation;

        public bool Equals(Pose o)
        {
            return position.Equals(o.position)
                && rotation.Equals(o.rotation);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return base.Equals((Pose)obj);
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() + rotation.GetHashCode();
        }

        public override string ToString()
        {
            return $"Pose:(p:{position}, r:{rotation})";
        }

        public static implicit operator Pose(UnityEngine.Pose p)
        {
            return new Pose()
            {
                position = new float3(p.position.x, p.position.y, p.position.z),
                rotation = new float4(p.rotation.x, p.rotation.y, p.rotation.z, p.rotation.w)
            };
        }

        public static implicit operator Pose(ARPoseDriver arPoseDriver)
        {
            Pose pose = new Pose();
            var transform = arPoseDriver.transform;
            var position1 = transform.position;
            pose.position = new float3(
                position1.x,
                position1.y,
                position1.z);
            var rotation1 = transform.rotation;
            pose.rotation = new float4(
                rotation1.x,
                rotation1.y,
                rotation1.z,
                rotation1.w);
            
            return pose;
        }

        public static implicit operator UnityEngine.Pose(Pose p)
        {
            Quaternion quaternion = new Quaternion(p.rotation.x, p.rotation.y, p.rotation.z, p.rotation.w);
            return new UnityEngine.Pose(
                new Vector3(p.position.x, p.position.y, p.position.z),
                quaternion);
        }

        public static Pose FromTransform(Transform t)
        {
            var q = t.localRotation;
            float3 position = new float3(t.localPosition.x, t.localPosition.y, t.localPosition.z);
            return new Pose()
            {
                position = position,
                rotation = new float4(q.x, q.y, q.z, q.w),
            };
        }

    }
}
