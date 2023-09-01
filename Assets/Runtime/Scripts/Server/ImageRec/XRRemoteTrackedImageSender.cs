//-------------------------------------------------------------------------------------------------------
// <copyright file="XRRemoteTrackedImageSender.cs" createdby="cSustrich">
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
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;
using XRRemote.Serializables;
using Unity.Jobs;
using System.Linq;

namespace XRRemote
{
    public class XRRemoteTrackedImageSender : MonoBehaviour
    {
        [SerializeField] private ARTrackedImageManager arTrackedImageManager;
        [SerializeField] private Text libraryCountText;
        private Dictionary<Texture2D, XRInfo> reconstructedImages;
        private bool supportsMutableLibraries => arTrackedImageManager.descriptor.supportsMutableLibrary;

        private void OnEnable()
        {
            ServerReceiver.Instance.OnImageLibraryReceived += XRRemoteTrackedImageSender_OnImageLibraryReceived;
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            ServerReceiver.Instance.OnImageLibraryReceived -= XRRemoteTrackedImageSender_OnImageLibraryReceived;
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            
        }

        private void Update()
        {
            if (arTrackedImageManager.enabled == true && arTrackedImageManager.referenceLibrary != null)
            {
                libraryCountText.text = $"Library Count: {arTrackedImageManager.referenceLibrary.count}";
            }
        }

        private void XRRemoteTrackedImageSender_OnImageLibraryReceived(object sender, EventArgs e)
        {   
            
            List<SerializableXRReferenceImage> serializedTextures = ServerReceiver.Instance.serializedTextures;

            if (serializedTextures == null || serializedTextures.Count == 0)
            {
                Debug.LogError("No images received from client.");
                return;
            }

            if (supportsMutableLibraries)
            {
                var mutableLibrary = arTrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
                // [delete]
                // int supportedFormatCount = mutableLibrary.supportedTextureFormatCount;
                // Debug.Log($"XRRemoteImageManager: Mutable library supports {supportedFormatCount} texture formats.");
                // for (int i = 0; i < supportedFormatCount; i++)
                // {
                //     TextureFormat supportedFormat = mutableLibrary.GetSupportedTextureFormatAt(i);
                //     Debug.Log($"Supported Texture Format {i}: {supportedFormat}");
                // }

                AddImagesToLibrary(mutableLibrary, ReconstructLibrary(serializedTextures));
                InitializeNativeImageManager(mutableLibrary);
            }
            else
            {
                Debug.LogError("This XRImageTrackingSubsystem does not support mutable libraries");
                return;
            }
        }

        private Dictionary<Texture2D, XRInfo> ReconstructLibrary(List<SerializableXRReferenceImage> serializedTextures)
        {
            reconstructedImages = new Dictionary<Texture2D, XRInfo>();

            foreach (SerializableXRReferenceImage texture in serializedTextures)
            {
                Texture2D tex = texture.ConvertFromSerializableXRReferenceImageToTexture2D(out XRInfo xrInfo);
                reconstructedImages.Add(tex, xrInfo);
            }

            return reconstructedImages;
        }

        private void AddImagesToLibrary(MutableRuntimeReferenceImageLibrary mutableLibrary, Dictionary<Texture2D, XRInfo> reconstructedImages)
        {
            foreach (KeyValuePair<Texture2D, XRInfo> entry in reconstructedImages)
            {
                if (mutableLibrary.IsTextureFormatSupported(entry.Key.format))
                {
                    try
                    {
                        Texture2D newImageTexture = entry.Key;
                        string newImageName = entry.Value.name;
                        float? newImageWidthInMeters = entry.Value.specifySize ? entry.Value.size.x : null;

                        AddReferenceImageJobState jobState = mutableLibrary.ScheduleAddImageWithValidationJob(
                            newImageTexture, 
                            newImageName, 
                            newImageWidthInMeters
                        );

                        JobHandle jobHandle = jobState.jobHandle;
                        jobHandle.Complete();

                        if (jobState.status == AddReferenceImageJobStatus.Success)
                        {
                            Debug.Log($"XRRemoteImageManager: Image {newImageName} added to library successfully.");
                        }
                        else
                        {
                            Debug.LogWarning($"XRRemoteImageManager: Failed to add image {newImageName} to library. {jobState.status}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"XRRemoteImageManager: Failed to add image {entry.Value.name} to library. {e}");
                    }
                }
            }
        }

        private void InitializeNativeImageManager(MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            arTrackedImageManager.enabled = false;
            arTrackedImageManager.referenceLibrary = mutableLibrary;
            arTrackedImageManager.enabled = true;
        }

        public bool TryGetTrackables(out List<SerializableARTrackedImage> trackables)
        {
            TrackableCollection<ARTrackedImage> currentlyTracking = arTrackedImageManager.trackables;
            if (currentlyTracking.count == 0)
            {
                trackables = null;
                return false;
            }
            trackables = new List<SerializableARTrackedImage>();
            foreach (ARTrackedImage image in currentlyTracking)
            {
                if (image.trackingState != TrackingState.None) trackables.Add(new SerializableARTrackedImage(image));
            }
            if (trackables.Count == 0) return false;
            
            return true;
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
                SetTexture(trackedImage);
                UpdateInfo(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);
        }

        private void SetTexture(ARTrackedImage trackedImage)
        {
            GameObject go = trackedImage.gameObject;
            var material = go.GetComponentInChildren<MeshRenderer>().material;
            Texture2D tex = reconstructedImages.FirstOrDefault(item => item.Value.name == trackedImage.referenceImage.name).Key;
            material.mainTexture = tex;
        }

        void UpdateInfo(ARTrackedImage trackedImage)
        {
            UpdateText(trackedImage);

            var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
            var planeGo = planeParentGo.transform.GetChild(0).gameObject;


            // Disable the visual plane if it is not being tracked
            if (trackedImage.trackingState != TrackingState.None )
            {
                planeGo.SetActive(true);
                trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);
            }
            else
            {
                planeGo.SetActive(false);
            }
        }

        private void UpdateText(ARTrackedImage trackedImage)
        {
            GameObject go = trackedImage.gameObject;
            var text = go.GetComponentInChildren<Text>();

            text.text = string.Format(
                "{0}\nTracking State: {1}\nReference size: {2} cm\nDetected size: {3} cm",
                trackedImage.referenceImage.name,
                trackedImage.trackingState,
                trackedImage.referenceImage.size * 100f,
                trackedImage.size * 100f);
        }
    }
}
