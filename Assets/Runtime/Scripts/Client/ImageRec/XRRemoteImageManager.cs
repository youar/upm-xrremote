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
using System.Collections.Generic;
using XRRemote.Serializables;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;

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
        private List<Texture2D> texPool = new List<Texture2D>();

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

        public void OnDestroy()
        {
            foreach (var tex in texPool)
            {
                if (tex != null) Destroy(tex);
            }
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
            var trackedImagesToDelete = currentlyTracking.Keys.Except(trackableIds).ToList();
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
                SerializableARTrackedImage remoteInstance = receivedList.Find(item => item.trackableId.Equals(entry));
                Transform prefabToSpawn = hasUserPrefab ? manager.trackedImagePrefab.transform : trackedImagePrefab;

                Transform localInstance = Instantiate(prefabToSpawn);
                localInstance.SetParent(manager.gameObject.transform);
                SetTexture(localInstance, remoteInstance);
                currentlyTracking.Add(entry, localInstance.gameObject);
            }

            //update newly added && updated tracked images   
            foreach (var entry in currentlyTracking)
            {
                SerializableARTrackedImage trackedImage = receivedList.Find(item => item.trackableId.Equals(entry.Key));
                UpdatePose(entry.Value.transform, trackedImage);
                if (!hasUserPrefab) UpdateText(entry.Value.transform, trackedImage);
            }
        }

        private void UpdatePose(Transform localInstance, SerializableARTrackedImage remoteInstance)
        {
            localInstance.transform.localScale = new Vector3(remoteInstance.size.x, 1f, remoteInstance.size.y); 
                localInstance.position = remoteInstance.pose.position.ToVector3();
                localInstance.rotation = new Quaternion(
                    remoteInstance.pose.rotation.x, 
                    remoteInstance.pose.rotation.y, 
                    remoteInstance.pose.rotation.z, 
                    remoteInstance.pose.rotation.w
                );   
        }

        private void UpdateText(Transform localInstance, SerializableARTrackedImage remoteInstance)
        {
            GameObject go = localInstance.gameObject;
            SerializableXRReferenceImage foundImage = serializedLibrary.FirstOrDefault(image => image.texName == remoteInstance.name);

            var text = go.GetComponentInChildren<Text>();
            text.text = string.Format(
                "{0}\nTracking State: {1}\nReference size: {2} cm\nDetected size: {3} cm",
                remoteInstance.name,
                remoteInstance.trackingState,//get name of state not number
                foundImage.realSize.x * 100f,
                remoteInstance.size.x * 100f);
        }

        private void SetTexture(Transform localInstance, SerializableARTrackedImage remoteInstance)
        {
            GameObject go = localInstance.gameObject;
            SerializableXRReferenceImage foundImage = serializedLibrary.FirstOrDefault(image => image.texName == remoteInstance.name);

            var material = go.GetComponentInChildren<MeshRenderer>().material;
            var tex = new Texture2D((int)foundImage.texSize.x, (int)foundImage.texSize.y, (TextureFormat)Enum.Parse(typeof(TextureFormat), foundImage.texFormat), false);
            tex.LoadRawTextureData(foundImage.texData);
            tex.Apply();
            material.mainTexture = tex;
            texPool.Add(tex);
        }
    }
}
