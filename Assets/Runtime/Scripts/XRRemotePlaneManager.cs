using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace XRRemote
{
    public class XRRemotePlaneManager : MonoBehaviour
    {
        public Transform XRPlaneCenterVisualPrefab;

        List<Transform> xrPlaneCenterVisualList = new List<Transform>();


        private void Update()
        {
            if (XREditorClient.Instance.centerPoints  == null) return;
            if (XREditorClient.Instance.centerPoints.Length == 0) return;

            UpdateVisuals(XREditorClient.Instance.centerPoints);
        }

        private void UpdateVisuals(float3[] planes)
        {
            DisableAllVisuals();

            for (int i = 0; i < planes.Length; i++) {
                if (i < xrPlaneCenterVisualList.Count) {
                    xrPlaneCenterVisualList[i].transform.position = planes[i].ToVector3();

                    xrPlaneCenterVisualList[i].gameObject.SetActive(true);
                } else {
                    Transform newVisual = Instantiate(XRPlaneCenterVisualPrefab);

                    newVisual.position = planes[i].ToVector3();

                    xrPlaneCenterVisualList.Add(newVisual);
                }
            }
        }

        private void DisableAllVisuals()
        {
            foreach (Transform xrPlaneCenterVisualTransform in xrPlaneCenterVisualList) {
                xrPlaneCenterVisualTransform.gameObject.SetActive(false);
            }
        }
    }
}
#endif
