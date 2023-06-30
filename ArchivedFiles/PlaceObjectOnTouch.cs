using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRRemote;

#if UNITY_EDITOR
namespace XRRemote
{
    public class PlaceObjectOnTouch : MonoBehaviour
    {
        [SerializeField] private LayerMask planeLayerMask;
        public GameObject spawnedObject { get; private set; }
        [SerializeField] private GameObject placedPrefab;

        private void OnEnable()
        {
            XRRemote.XRRemoteInputSystem.Instance.OnTouch += XRRemoteInputSystem_OnTouch;
        }

        private void OnDisable()
        {
            XRRemote.XRRemoteInputSystem.Instance.OnTouch -= XRRemoteInputSystem_OnTouch;
        }

        private void XRRemoteInputSystem_OnTouch(object sender, EventArgs e)
        {
            Vector3 touchPosition = XRRemoteInputSystem.Instance.touchPosition;

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, planeLayerMask)) {
                AddObject(raycastHit);
            }
        }

        /// <summary>
        /// Add object or move existing one. Used in testing touch input & raycasting on Unity client
        /// </summary>
        private void AddObject(RaycastHit raycastHit)
        {
            if (spawnedObject == null) {
                spawnedObject = Instantiate(placedPrefab, raycastHit.point, Quaternion.identity);
            } else {
                spawnedObject.transform.position = raycastHit.point;
            }
        }
    }
}
#endif
