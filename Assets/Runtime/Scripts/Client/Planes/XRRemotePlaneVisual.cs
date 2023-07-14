//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemotePlaneVisual.cs" createdby="gblikas">
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
using Unity.Collections;
using UnityEngine;
using XRRemote.Serializables;
using UnityEngine.XR.ARFoundation;

#if UNITY_EDITOR
namespace XRRemote
{
    /// <summary>
    /// Using to visualize planes in the Unity editor that are detected by the AR Plane Manager on the server
    /// </summary>
    public class XRRemotePlaneVisual : MonoBehaviour
    {
        public SerializableTrackableId trackableId;
        public MeshFilter arDefaultPlaneMeshFilter;

        //Set up the plane's data from XRPlane received in packet
        public void Setup(SerializableXRPlaneNdi plane)
        {
            trackableId = plane.trackableId;

            //Retrieve the boundary points
            NativeArray<Vector2> boundary = new NativeArray<Vector2>(plane.boundary.Length, Allocator.Temp);
            for (int j = 0; j < plane.boundary.Length; j++) {
                Vector2 localPosition = plane.boundary[j].ToVector2();
                boundary[j] = new Vector2(localPosition.x, localPosition.y);
            }

            //Move the plane to correct position and rotation using the plane's pose
            Transform arPlaneTransform = arDefaultPlaneMeshFilter.gameObject.transform;
            arPlaneTransform.position = plane.pose.position.ToVector3();
            arPlaneTransform.rotation = new Quaternion(
                plane.pose.rotation.x, 
                plane.pose.rotation.y, 
                plane.pose.rotation.z, 
                plane.pose.rotation.w
            );

            ARPlaneMeshGenerators.GenerateMesh(
                arDefaultPlaneMeshFilter.mesh, 
                plane.pose,
                boundary
            );

            //Update the collider with the new mesh
            MeshCollider meshCollider = arDefaultPlaneMeshFilter.gameObject.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = arDefaultPlaneMeshFilter.mesh;
        }
    }
}
#endif
