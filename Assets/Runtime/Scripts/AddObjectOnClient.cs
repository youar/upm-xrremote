using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRRemote;

#if UNITY_EDITOR
namespace XRRemote
{
    public class AddObjectOnClient : MonoBehaviour
    {
        [SerializeField] private LayerMask planeLayerMask;
        public GameObject spawnedObject { get; private set; }
        [SerializeField] private GameObject m_PlacedPrefab;

        private void Start()
        {
            XRRemote.XRRemoteInputSystem.Instance.OnTouch += XRRemoteInputSystem_OnTouch;
        }

        private void XRRemoteInputSystem_OnTouch(object sender, EventArgs e)
        {
            Debug.Log("Input happened! " + XRRemoteInputSystem.Instance.touchPosition);

            Vector3 touchPosition = XRRemoteInputSystem.Instance.touchPosition;

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, planeLayerMask)) {
                Debug.Log("Hit detected at " + raycastHit.point);
                AddObject(raycastHit);
            } else {
                Debug.Log("No hit...");
            } 
        }

        /// <summary>
        /// Add object or move existing one. Used in testing touch input & raycasting on Unity client
        /// </summary>
        private void AddObject(RaycastHit raycastHit)
        {
            if (spawnedObject == null) {
                spawnedObject = Instantiate(m_PlacedPrefab, raycastHit.point, Quaternion.identity);
            } else {
                spawnedObject.transform.position = raycastHit.point;
            }
        }
    }
}
#endif
