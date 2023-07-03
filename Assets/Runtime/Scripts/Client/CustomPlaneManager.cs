//-------------------------------------------------------------------------------------------------------
// <copyright file="CustomPlaneManager.cs" createdby="gblikas">
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
using System.Collections.Generic;
using UnityEngine;
using XRRemote.Serializables;

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
        private void AddVisuals(SerializableXRPlaneNdi[] planes)
        {
            Debug.Log("Planes = " + planes.Length);
            foreach (SerializableXRPlaneNdi plane in planes) {
                Transform newVisualTransform = Instantiate(CustomPlaneVisualPrefab);
                CustomPlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<CustomPlaneVisual>();

                xrRemotePlaneVisual.Setup(plane);

                xrPlaneVisualList.Add(xrRemotePlaneVisual);
            }
        }

        /// <summary>
        /// Find corresponding visual and tell it to update its data
        /// </summary>
        private void UpdateVisuals(SerializableXRPlaneNdi[] planes)
        {
            foreach (SerializableXRPlaneNdi plane in planes) {
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
        private void RemoveVisuals(SerializableXRPlaneNdi[] planes)
        {
            foreach (SerializableXRPlaneNdi plane in planes) {
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
