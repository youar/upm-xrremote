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

        [SerializeField] private List<XRRemotePlaneVisual> xrPlaneVisualList = new List<XRRemotePlaneVisual>();


        private void OnEnable()
        {
            XREditorClient.Instance.OnPlanesInfoReceived += XREditorClient_OnPlanesInfoReceived;
        }

        private void OnDisable()
        {
            if (XREditorClient.Instance == null) return;
            
            XREditorClient.Instance.OnPlanesInfoReceived -= XREditorClient_OnPlanesInfoReceived;
        }

        /// <summary>
        /// Create a new visual and add to list
        /// </summary>
        private void AddVisuals(XRPlane[] planes)
        {
            foreach (XRPlane plane in planes) {
                Transform newVisualTransform = Instantiate(XRRemotePlaneVisualPrefab);
                XRRemotePlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<XRRemotePlaneVisual>();

                xrRemotePlaneVisual.Setup(plane);

                xrPlaneVisualList.Add(xrRemotePlaneVisual);
            }
        }

        /// <summary>
        /// Find corresponding visual and tell it to update its data
        /// </summary>
        private void UpdateVisuals(XRPlane[] planes)
        {
            foreach (XRPlane plane in planes) {
                Predicate<XRRemotePlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<XRRemotePlaneVisual> planeVisualsToUpdate = xrPlaneVisualList.FindAll(hasTrackableId);

                if (planeVisualsToUpdate.Count == 0) {
                    if (DebugFlags.displayEditorConnectionStats) {
                        Debug.LogError($"XRRemotePlaneManager - UpdateVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                    }
                } else if (planeVisualsToUpdate.Count > 1) {
                    if (DebugFlags.displayEditorConnectionStats) {
                        Debug.LogWarning($"XRRemotePlaneManager - UpdateVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                    }
                }

                foreach (XRRemotePlaneVisual planeVisual in planeVisualsToUpdate) {
                    planeVisual.Setup(plane);

                    // if (plane.isSubsumed) {
                    //     planeVisual.gameObject.SetActive(false);
                    // }
                }
            }
        }

        /// <summary>
        /// Find corresponding visual and destroy the visual and remove it from the list
        /// </summary>
        private void RemoveVisuals(XRPlane[] planes)
        {
            foreach (XRPlane plane in planes) {
                Predicate<XRRemotePlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<XRRemotePlaneVisual> planeVisualsToRemove = xrPlaneVisualList.FindAll(hasTrackableId);

                if (planeVisualsToRemove.Count == 0) {
                    if (DebugFlags.displayEditorConnectionStats) {
                        Debug.LogError($"XRRemotePlaneManager - RemoveVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                    }
                } else if (planeVisualsToRemove.Count > 1) {
                    if (DebugFlags.displayEditorConnectionStats) {
                        Debug.LogWarning($"XRRemotePlaneManager - RemoveVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                    }
                }

                foreach (XRRemotePlaneVisual planeVisual in planeVisualsToRemove) {
                    xrPlaneVisualList.Remove(planeVisual);
                    Destroy(planeVisual.gameObject);
                }
            }
        }

        private void XREditorClient_OnPlanesInfoReceived(object sender, EventArgs e)
        {
            if (XREditorClient.Instance.planesInfo.added != null) {
                AddVisuals(XREditorClient.Instance.planesInfo.added);
            }

            if (XREditorClient.Instance.planesInfo.updated != null) {
                UpdateVisuals(XREditorClient.Instance.planesInfo.updated);
            }

            if (XREditorClient.Instance.planesInfo.removed != null) {
                RemoveVisuals(XREditorClient.Instance.planesInfo.removed);
            }
        }
    }
}
#endif
