using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;

public class AddObjectOnInput : MonoBehaviour
{
    [SerializeField] private GameObject m_PlacedPrefab;

    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    public GameObject spawnedObject { get; private set; }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARRaycastManager m_RaycastManager;


    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    /// <summary>
        /// Add object or move existing one. Used in testing touch input & raycasting on device
        /// </summary>
    public void AddObject(InputAction.CallbackContext context)
    {
        var touchPosition = context.ReadValue<Vector2>();

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
            } else {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }
}
