using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

#if UNITY_EDITOR
namespace XRRemote
{
    using UnityEngine.XR.ARFoundation;

    public class XRRemotePlaneManager : MonoBehaviour
    {
        public Transform XRRemotePlaneVisualPrefab;

        List<XRRemotePlaneVisual> xrPlaneVisualList = new List<XRRemotePlaneVisual>();


        private void Update()
        {
            if (XREditorClient.Instance.planesInfo  == null) return; //XRRemotePacket not sent yet

            UpdateVisuals(XREditorClient.Instance.planesInfo.xrPlanes);
        }

        private void UpdateVisuals(XRPlane[] planes)
        {
            DisableAllVisuals();

            for (int i = 0; i < planes.Length; i++) {
                if (i < xrPlaneVisualList.Count) {
                    xrPlaneVisualList[i].xrPlaneCenterVisual.position = planes[i].center.ToVector3();

                    NativeArray<Vector2> boundary = new NativeArray<Vector2>(planes[i].boundary.Length, Allocator.Temp);
                    for (int j = 0; j < planes[i].boundary.Length; j++) {
                        Vector2 localPosition = planes[i].boundary[j].ToVector2();
                        boundary[j] = new Vector2(localPosition.x, localPosition.y);
                    }

                    xrPlaneVisualList[i].arDefaultPlaneMeshFilter.gameObject.transform.position = planes[i].pose.position.ToVector3();
                    xrPlaneVisualList[i].arDefaultPlaneMeshFilter.gameObject.transform.rotation = new Quaternion(planes[i].pose.rotation.x, planes[i].pose.rotation.y, planes[i].pose.rotation.z, planes[i].pose.rotation.w);


                    ARPlaneMeshGenerators.GenerateMesh(
                        xrPlaneVisualList[i].arDefaultPlaneMeshFilter.mesh, 
                        planes[i].pose,
                        boundary
                    );

                    xrPlaneVisualList[i].gameObject.SetActive(true);
                } else {
                    Transform newVisualTransform = Instantiate(XRRemotePlaneVisualPrefab);
                    XRRemotePlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<XRRemotePlaneVisual>();

                    xrRemotePlaneVisual.xrPlaneCenterVisual.position = planes[i].centerInPlaneSpace.ToVector3();

                    NativeArray<Vector2> boundary = new NativeArray<Vector2>(planes[i].boundary.Length, Allocator.Temp);
                    for (int j = 0; j < planes[i].boundary.Length; j++) {
                        Vector2 localPosition = planes[i].boundary[j].ToVector2();
                        boundary[j] = new Vector2(planes[i].centerInPlaneSpace.x - localPosition.x, planes[i].centerInPlaneSpace.z - localPosition.y);
                    }

                    ARPlaneMeshGenerators.GenerateMesh(
                        xrRemotePlaneVisual.arDefaultPlaneMeshFilter.mesh, 
                        planes[i].pose,
                        boundary
                    );

                    xrPlaneVisualList.Add(xrRemotePlaneVisual);
                }
            }
        }

        private void DisableAllVisuals()
        {
            foreach (XRRemotePlaneVisual xrRemotePlaneVisual in xrPlaneVisualList) {
                xrRemotePlaneVisual.gameObject.SetActive(false);
            }
        }
    }
}
#endif
