using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace XRRemote
{
    /// <summary>
    /// Manages a pool of XR Plane Visuals
    /// </summary>
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

            //Enable a visual for each plane sent in the packet
            for (int i = 0; i < planes.Length; i++) {
                if (i < xrPlaneVisualList.Count) {
                    //Use previously created visual
                    xrPlaneVisualList[i].Setup(planes[i]);
                } else {
                    //Create a new visual and add to list
                    Transform newVisualTransform = Instantiate(XRRemotePlaneVisualPrefab);
                    XRRemotePlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<XRRemotePlaneVisual>();

                    xrRemotePlaneVisual.Setup(planes[i]);

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
