using System;
using System.Collections.Generic;
using UnityEngine;
using XRRemote.Serializables;
// using System.Collections;

#if UNITY_EDITOR
namespace XRRemote
{
    /// <summary>
    /// Manages a pool of XR Plane Visuals
    /// </summary>
    public class CustomPlaneManager : MonoBehaviour
    {
        public Transform CustomPlaneVisualPrefab;

        [SerializeField] private List<CustomPlaneVisual> xrPlaneVisualList = new List<CustomPlaneVisual>();


        private void OnEnable()
        {
            if (CustomNdiReceiver.Instance == null) return;
            CustomNdiReceiver.Instance.OnPlanesInfoReceived += CustomNdiReceiver_OnPlanesInfoReceived;
        }

        private void OnDisable()
        {
            if (CustomNdiReceiver.Instance == null) return;
            CustomNdiReceiver.Instance.OnPlanesInfoReceived -= CustomNdiReceiver_OnPlanesInfoReceived;
        }

        /// <summary>
        /// Create a new visual and add to list
        /// </summary>
        private void AddVisuals(XRPlaneNdi[] planes)
        {
            Debug.Log("Planes = " + planes.Length);
            foreach (XRPlaneNdi plane in planes) {
                Transform newVisualTransform = Instantiate(CustomPlaneVisualPrefab);
                CustomPlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<CustomPlaneVisual>();

                xrRemotePlaneVisual.Setup(plane);

                xrPlaneVisualList.Add(xrRemotePlaneVisual);
            }
        }

        /// <summary>
        /// Find corresponding visual and tell it to update its data
        /// </summary>
        private void UpdateVisuals(XRPlaneNdi[] planes)
        {
            foreach (XRPlaneNdi plane in planes) {
                Predicate<CustomPlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<CustomPlaneVisual> planeVisualsToUpdate = xrPlaneVisualList.FindAll(hasTrackableId);

                // if (planeVisualsToUpdate.Count == 0) {
                //     if (DebugFlags.displayEditorConnectionStats) {
                //         Debug.LogError($"CustomPlaneManager - UpdateVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                //     }
                // } else if (planeVisualsToUpdate.Count > 1) {
                //     if (DebugFlags.displayEditorConnectionStats) {
                //         Debug.LogWarning($"CustomPlaneManager - UpdateVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                //     }
                // }

                foreach (CustomPlaneVisual planeVisual in planeVisualsToUpdate) {
                    planeVisual.Setup(plane);

                }
            }
        }

        /// <summary>
        /// Find corresponding visual and destroy the visual and remove it from the list
        /// </summary>
        private void RemoveVisuals(XRPlaneNdi[] planes)
        {
            foreach (XRPlaneNdi plane in planes) {
                Predicate<CustomPlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<CustomPlaneVisual> planeVisualsToRemove = xrPlaneVisualList.FindAll(hasTrackableId);

                // if (planeVisualsToRemove.Count == 0) {
                //     if (DebugFlags.displayEditorConnectionStats) {
                //         Debug.LogError($"CustomPlaneManager - RemoveVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                //     }
                // } else if (planeVisualsToRemove.Count > 1) {
                //     if (DebugFlags.displayEditorConnectionStats) {
                //         Debug.LogWarning($"CustomPlaneManager - RemoveVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                //     }
                // }

                foreach (CustomPlaneVisual planeVisual in planeVisualsToRemove) {
                    xrPlaneVisualList.Remove(planeVisual);
                    Destroy(planeVisual.gameObject);
                }
            }
        }

        private void CustomNdiReceiver_OnPlanesInfoReceived(object sender, EventArgs e)
        {
            if (CustomNdiReceiver.Instance.remotePacket.planesInfo.added != null) {
                AddVisuals(CustomNdiReceiver.Instance.remotePacket.planesInfo.added);
            }

            if (CustomNdiReceiver.Instance.remotePacket.planesInfo.updated != null) {
                UpdateVisuals(CustomNdiReceiver.Instance.remotePacket.planesInfo.updated);
            }

            if (CustomNdiReceiver.Instance.remotePacket.planesInfo.removed != null) {
                RemoveVisuals(CustomNdiReceiver.Instance.remotePacket.planesInfo.removed);
            }
        }
    }
}
#endif
