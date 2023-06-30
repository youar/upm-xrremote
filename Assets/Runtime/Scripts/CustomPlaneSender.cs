using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using XRRemote.Serializables;

namespace XRRemote
{
    public class CustomPlaneSender : MonoBehaviour
    {
        private PlanesInfo planesInfo = null;

        private ARPlaneManager arPlaneManager;

        private void Awake()
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();

            if (arPlaneManager == null) {
                // if (DebugFlags.displayEditorConnectionStats) {
                //     Debug.LogError($"XRRemotePlaneSender: Unable to ARPlaneManager. Please make sure there is one in the scene.");
                // }

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

        private XRPlaneNdi[] GetArrayOfXRPlanes(List<ARPlane> arPlanes)
        {
            if (arPlanes == null || arPlanes.Count == 0) {
                return null;
            }

            XRPlaneNdi[] xrPlanes = new XRPlaneNdi[arPlanes.Count];

            for (int i = 0; i < arPlanes.Count; i++) {
                xrPlanes[i] = new XRPlaneNdi(arPlanes[i]);
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
