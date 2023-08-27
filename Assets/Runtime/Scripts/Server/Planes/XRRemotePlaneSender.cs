//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemotePlaneSender.cs" createdby="gblikas">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using XRRemote.Serializables;

namespace XRRemote
{
    public class XRRemotePlaneSender : MonoBehaviour
    {
        private SerializablePlanesInfo planesInfo = null;

        private ARPlaneManager arPlaneManager;

        private void Awake()
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();

            if (arPlaneManager == null) {
                if (DebugFlags.displayXRRemotePlaneStats) {
                    Debug.LogError($"XRRemotePlaneSender: Unable find to ARPlaneManager. Please make sure there is one in the scene.");
                }

                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            arPlaneManager.planesChanged += arPlaneManager_planesChanged;
        }

        private void OnDisable()
        {
            arPlaneManager.planesChanged -= arPlaneManager_planesChanged;
        }

        public bool TryGetPlanesInfo(out SerializablePlanesInfo planesInfo)
        {
            if (this.planesInfo == null) {
                planesInfo = null;
                return false;
            }

            planesInfo = this.planesInfo;
            this.planesInfo = null;

            return true;
        }

        public bool TryGetAllPlanesInfo(out SerializablePlanesInfo allPlanesInfo)
        {
            if (arPlaneManager.trackables.count == 0) {
                allPlanesInfo = null;
                return false;
            }

            allPlanesInfo = new SerializablePlanesInfo();

            //Get a list of all existing planes
            List<ARPlane> existingPlanesList = new List<ARPlane>();
            foreach (ARPlane plane in arPlaneManager.trackables) {
                existingPlanesList.Add(plane);
            }

            Debug.LogWarning($"ArPlaneManager: {arPlaneManager.trackables.count}");

            allPlanesInfo.added = GetArrayOfXRPlanes(existingPlanesList);
            allPlanesInfo.updated = null;
            allPlanesInfo.removed = null;

            allPlanesInfo.isHandshake = true;

            return true;
        }

        private SerializableARPlane[] GetArrayOfXRPlanes(List<ARPlane> arPlanes)
        {
            if (arPlanes == null || arPlanes.Count == 0) {
                return null;
            }

            SerializableARPlane[] xrPlanes = new SerializableARPlane[arPlanes.Count];

            for (int i = 0; i < arPlanes.Count; i++) {
                xrPlanes[i] = new SerializableARPlane(arPlanes[i]);
            }

            return xrPlanes;
        }

        private void arPlaneManager_planesChanged(ARPlanesChangedEventArgs arPlanesChangedEventArgs)
        {
            planesInfo = new SerializablePlanesInfo();

            planesInfo.added = GetArrayOfXRPlanes(arPlanesChangedEventArgs.added);
            planesInfo.updated = GetArrayOfXRPlanes(arPlanesChangedEventArgs.updated);
            planesInfo.removed = GetArrayOfXRPlanes(arPlanesChangedEventArgs.removed);
        }
    }
}
