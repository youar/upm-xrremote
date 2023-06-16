using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace XRRemote 
{
    public class XRRemotePlaneSender : MonoBehaviour
    {
        private PlanesInfo planesInfo = null;

        private ARPlaneManager arPlaneManager;

        private void Awake()
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();

            if (arPlaneManager == null) {
                if (DebugFlags.displayEditorConnectionStats) {
                    Debug.LogError($"XRRemotePlaneSender: Unable to ARPlaneManager. Please make sure there is one in the scene.");
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

        public bool TryGetPlanesInfo(out PlanesInfo planesInfo)
        {
            if (this.planesInfo == null) {
                planesInfo = null;
                return false;
            }

            planesInfo = this.planesInfo;
            this.planesInfo = null;

            return true;
        }

        public bool TryGetAllPlanesInfo(out PlanesInfo allPlanesInfo)
        {
            if (arPlaneManager.trackables.count == 0) {
                allPlanesInfo = null;
                return false;
            }

            allPlanesInfo = new PlanesInfo();
            
            //Get a list of all existing planes
            List<ARPlane> existingPlanesList = new List<ARPlane>();
            foreach (ARPlane plane in arPlaneManager.trackables) {
                existingPlanesList.Add(plane);
            }

            allPlanesInfo.added = GetArrayOfXRPlanes(existingPlanesList);
            allPlanesInfo.updated = null;
            allPlanesInfo.removed = null;

            return true;
        }

        private XRPlane[] GetArrayOfXRPlanes(List<ARPlane> arPlanes)
        {
            if (arPlanes == null || arPlanes.Count == 0) {
                return null;
            }

            XRPlane[] xrPlanes = new XRPlane[arPlanes.Count];

            for (int i = 0; i < arPlanes.Count; i++) {
                xrPlanes[i] = new XRPlane(arPlanes[i]);
            }

            return xrPlanes;
        }

        private void arPlaneManager_planesChanged(ARPlanesChangedEventArgs arPlanesChangedEventArgs)
        {
            planesInfo = new PlanesInfo();

            planesInfo.added = GetArrayOfXRPlanes(arPlanesChangedEventArgs.added);
            planesInfo.updated = GetArrayOfXRPlanes(arPlanesChangedEventArgs.updated);
            planesInfo.removed = GetArrayOfXRPlanes(arPlanesChangedEventArgs.removed);
        }
    }
}
