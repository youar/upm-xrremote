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
    public class XRRemotePlaneManager : MonoBehaviour
    {
        [SerializeField] private Transform XRRemotePlaneVisualPrefab;

        [SerializeField] private List<XRRemotePlaneVisual> xrPlaneVisualList = new List<XRRemotePlaneVisual>();


        private void OnEnable()
        {
            if (ClientReceiver.Instance == null) return;
            ClientReceiver.Instance.OnPlanesInfoReceived += CustomNdiReceiver_OnPlanesInfoReceived;
        }

        private void OnDisable()
        {
            if (ClientReceiver.Instance == null) return;
            ClientReceiver.Instance.OnPlanesInfoReceived -= CustomNdiReceiver_OnPlanesInfoReceived;
        }

        /// <summary>
        /// Create a new visual and add to list
        /// </summary>
        private void AddVisuals(SerializableXRPlaneNdi[] planes)
        {
            // Debug.Log("Planes = " + planes.Length);
            foreach (SerializableXRPlaneNdi plane in planes) {
                Transform newVisualTransform = Instantiate(XRRemotePlaneVisualPrefab);
                XRRemotePlaneVisual xrRemotePlaneVisual = newVisualTransform.GetComponent<XRRemotePlaneVisual>();

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
                Predicate<XRRemotePlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<XRRemotePlaneVisual> planeVisualsToUpdate = xrPlaneVisualList.FindAll(hasTrackableId);

                if (planeVisualsToUpdate.Count == 0) {
                    if (DebugFlags.displayXRRemotePlaneStats) {
                        Debug.LogError($"XRRemotePlaneManager - UpdateVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                    }
                } else if (planeVisualsToUpdate.Count > 1) {
                    if (DebugFlags.displayXRRemotePlaneStats) {
                        Debug.LogWarning($"XRRemotePlaneManager - UpdateVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                    }
                }

                foreach (XRRemotePlaneVisual planeVisual in planeVisualsToUpdate) {
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
                Predicate<XRRemotePlaneVisual> hasTrackableId = p => p.trackableId.Equals(plane.trackableId);

                List<XRRemotePlaneVisual> planeVisualsToRemove = xrPlaneVisualList.FindAll(hasTrackableId);

                if (planeVisualsToRemove.Count == 0) {
                    if (DebugFlags.displayXRRemotePlaneStats) {
                        Debug.LogError($"XRRemotePlaneManager - RemoveVisuals: No plane could be found with TrackableId " + plane.trackableId.ToString());
                    }
                } else if (planeVisualsToRemove.Count > 1) {
                    if (DebugFlags.displayXRRemotePlaneStats) {
                        Debug.LogWarning($"XRRemotePlaneManager - RemoveVisuals: Multiple planes found with TrackableId " + plane.trackableId.ToString());
                    }
                }

                foreach (XRRemotePlaneVisual planeVisual in planeVisualsToRemove) {
                    xrPlaneVisualList.Remove(planeVisual);
                    Destroy(planeVisual.gameObject);
                }
            }
        }

        private void CustomNdiReceiver_OnPlanesInfoReceived(object sender, EventArgs e)
        {            
            if (ClientReceiver.Instance.remotePacket.planesInfo.added != null) {
                AddVisuals(ClientReceiver.Instance.remotePacket.planesInfo.added);
            }

            if (ClientReceiver.Instance.remotePacket.planesInfo.updated != null) {
                UpdateVisuals(ClientReceiver.Instance.remotePacket.planesInfo.updated);
            }

            if (ClientReceiver.Instance.remotePacket.planesInfo.removed != null) {
                RemoveVisuals(ClientReceiver.Instance.remotePacket.planesInfo.removed);
            }
        }
    }
}
#endif
