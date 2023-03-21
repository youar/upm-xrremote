using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace XRRemote
{
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
                    xrPlaneVisualList[i].Setup(planes[i]);
                } else {
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
