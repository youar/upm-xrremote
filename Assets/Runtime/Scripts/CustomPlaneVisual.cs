using System.Collections;
using System.Collections.Generic;
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
    public class CustomPlaneVisual : MonoBehaviour
    {
        public TrackableId trackableId;
        public MeshFilter arDefaultPlaneMeshFilter;

        //Set up the plane's data from XRPlane received in packet
        public void Setup(XRPlaneNdi plane)
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
