using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using XRRemote;

#if UNITY_EDITOR
namespace XRRemote
{
    using UnityEngine.XR.ARFoundation;
    
    public class XRRemotePlaneVisual : MonoBehaviour
    {
        public Transform xrPlaneCenterVisual;
        public MeshFilter arDefaultPlaneMeshFilter;

        public void Setup(XRPlane plane)
        {
            xrPlaneCenterVisual.position = plane.center.ToVector3();

            NativeArray<Vector2> boundary = new NativeArray<Vector2>(plane.boundary.Length, Allocator.Temp);
            for (int j = 0; j < plane.boundary.Length; j++) {
                Vector2 localPosition = plane.boundary[j].ToVector2();
                boundary[j] = new Vector2(localPosition.x, localPosition.y);
            }

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

            gameObject.SetActive(true);
        }
    }
}
#endif
