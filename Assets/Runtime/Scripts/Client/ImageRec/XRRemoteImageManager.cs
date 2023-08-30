//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteImagemanager.cs" createdby="cSustrich">
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
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using XRRemote.Serializables;
using System.Linq;

namespace XRRemote
{   
    public class XRRemoteImageManager : MonoBehaviour
    {
        [Tooltip("The native AR Tracked Image Manager component attached to AR Session Origin.")]

        private static XRRemoteImageManager _instance;
        public static XRRemoteImageManager Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<XRRemoteImageManager>();
                }
                return _instance;
            }
        
        }
        
        [SerializeField] private ARTrackedImageManager manager;
        public ARTrackedImageManager Manager => manager;

        [SerializeField] private Transform trackedImagePrefab;
        private bool hasUserPrefab => manager.trackedImagePrefab != null;

        public List<SerializableXRReferenceImage> serializedLibrary {get; private set;}
        private Dictionary<SerializableTrackableId, GameObject> currentlyTracking = new Dictionary<SerializableTrackableId, GameObject>();

        public event EventHandler OnTrackedImagesChanged;

        public void OnEnable()
        {
            if (CheckDependencies())
            {
                StartCoroutine(UpdateLibrary());
            } 
            ClientReceiver.Instance.OnTrackedImagesReceived += ClientReceiver_OnTrackedImagesReceived;
        }

        public void OnDisable()
        {
            StopAllCoroutines();
            ClientReceiver.Instance.OnTrackedImagesReceived -= ClientReceiver_OnTrackedImagesReceived;
        }

        private bool CheckDependencies()
        {
            if (manager == null)
            {
                manager = FindObjectOfType<ARTrackedImageManager>();
            }

            if (manager == null)
            {
                Debug.LogError("XRRemoteImageManager: ARTrackedImageManager not found.");
                return false;
            } 
            if (manager.referenceLibrary == null)
            {
                Debug.LogError("XRRemoteImageManager: No reference library found on ARTrackedImageManager.");
                return false;
            } 
            return true;
        }

        private IEnumerator UpdateLibrary()
        {
            while (true)
            {
                XRReferenceImageLibrary imageLibrary = manager.referenceLibrary as XRReferenceImageLibrary;
                if (CheckLibraryValidity(imageLibrary))
                {
                    SerializeImageLibrary(imageLibrary);
                    StopAllCoroutines();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private bool CheckLibraryValidity(XRReferenceImageLibrary imageLibrary)
        {
            for (int i = 0; i < imageLibrary.count; i++)
            {
                //check for issues with library entry that would cause errors on server
                bool emptyTextureError = imageLibrary[i].textureGuid.Equals(Guid.Empty);
                bool sizeError = imageLibrary[i].specifySize && imageLibrary[i].size.ToString() == "(0.00, 0.00)";
                if (emptyTextureError || sizeError)
                {
                    if (emptyTextureError)
                    {
                        Debug.LogWarning("XRRemoteImageManager: Reference Image with no texture found.");
                    }
                    if (sizeError)
                    {
                        Debug.LogWarning($"XRRemoteImageManager: Reference Image with specified size (0, 0) found at index {i}.");
                    }
                    return false;
                }
            }
            return true;
        }

        private void SerializeImageLibrary(XRReferenceImageLibrary library)
        {
            List<SerializableXRReferenceImage> serializedLibrary = new List<SerializableXRReferenceImage>();
            for (int i = 0; i < library.count; i++)
            {
                SerializableXRReferenceImage serializedTexture = new SerializableXRReferenceImage(library[i]);
                if (serializedTexture.texData != null) serializedLibrary.Add(serializedTexture);
            }

            this.serializedLibrary = serializedLibrary;
            
            if (serializedLibrary.Count != library.count)
            {
                Debug.LogWarning("XRRemoteImageManager: Some images not sent to device. See individual Errors for further details.");
            }
        }

        private void ClientReceiver_OnTrackedImagesReceived(object sender, EventArgs e)
        {
            List<SerializableARTrackedImage> receivedList = ClientReceiver.Instance.remotePacket.trackedImages;

            var trackableIds = receivedList.Select(entry => entry.trackableId);

            //delete no longer tracked images
            var trackedImagesToDelete = currentlyTracking.Keys.Except(trackableIds);
            foreach (var entry in trackedImagesToDelete)
            {
                GameObject trackedImage = currentlyTracking[entry];
                currentlyTracking.Remove(entry);
                Destroy(trackedImage);
            }

            //add new tracked images
            var trackedImagesToAdd = trackableIds.Except(currentlyTracking.Keys);
            foreach (var entry in trackedImagesToAdd)
            {
                SerializableARTrackedImage trackedImage = receivedList.Find(item => item.trackableId.Equals(entry));
                if (hasUserPrefab)
                {
                    //spawn new user prefab
                }
                else
                {
                    Transform newTrackedImage = Instantiate(trackedImagePrefab);//spawn new default prefab
                }

                // GameObject newTrackedImage = Instantiate(trackedImagePrefab, trackedImage.pose.position, trackedImage.pose.rotation);
                // newTrackedImage.transform.localScale = new Vector3(trackedImage.size.x, trackedImage.size.y, 1f);
                // newTrackedImage.transform.parent = transform;
                // currentlyTracking.Add(entry, newTrackedImage);
                // GameObject go1; // = something;
                // go1.transform.SetParent(Manager.gameObject.transform);
                //[review] check if manager moves
            }

            // trackedimageschanged
            //     added : 
            //     updated:
            //     deleted: 






            // var  = ClientReceiver.Instance.remotePacket.trackedImages
            //     .Where(item => currentlyTracking.ContainsKey(item.trackableId));

            

            
                
        }

    }
}
