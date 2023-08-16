// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;

// [RequireComponent(typeof(ARTrackedImageManager))]

// public class TestImageRecScript : MonoBehaviour
// {
//     private ARTrackedImageManager _trackedImagesManager;
//     // public GameObject[] ArPrefabs;
//     // private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new ();
//     public Text text;


//     void Awake()
//     {
//         _trackedImagesManager = GetComponent<ARTrackedImageManager>();
//     }

//     private void OnEnable()
//     {
//         _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
//     }

//     private void OnDisable()
//     {
//         _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
//     }

//     private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
//     {
//         foreach (ARTrackedImage trackedImage in eventArgs.added)
//         {
//             string imageName = trackedImage.referenceImage.name;
//             text.text = $"{imageName} Detected";
//         }
//         foreach (ARTrackedImage trackedImage in eventArgs.updated)
//         {
//             string imageName = trackedImage.referenceImage.name;
//             text.text = $"{imageName} Detected";
//         }
//         // foreach (ARTrackedImage trackedImage in eventArgs.updated)
//         // {
//         //     string imageName = trackedImage.referenceImage.name;

//         //     foreach (GameObject curPrefab in ArPrefabs)
//         //     {
//         //         if (string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName))
//         //         {
//         //             text.text = $"{curPrefab.name} Detected";
//         //             //instantiate prefab, parenting it to ARTrackedImage
//         //             GameObject newPrefab = Instantiate(curPrefab, trackedImage.transform);
//         //             _instantiatedPrefabs[imageName] = newPrefab;
//         //         }
//         //     }
//         // }

//         // foreach (ARTrackedImage trackedImage in eventArgs.updated)
//         // {
//         //     _instantiatedPrefabs[trackedImage.referenceImage.name]
//         //         .SetActive(trackedImage.trackingState == TrackingState.Tracking);
//         // }

//         // foreach (ARTrackedImage trackedImage in eventArgs.removed)
//         // {
//         //     Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
//         //     _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
//         //     // Or
//         //     //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(false);
//         // }
//     }
// }
