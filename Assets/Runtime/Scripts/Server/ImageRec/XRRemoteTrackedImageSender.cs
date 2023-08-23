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
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;
using XRRemote.Serializables;
using Unity.Jobs;

namespace XRRemote
{
    public class XRRemoteTrackedImageSender : MonoBehaviour
    {

        [SerializeField] private ARTrackedImageManager arTrackedImageManager;
        [SerializeField] private Text imageNameText;
        [SerializeField] private Text libraryCountText;
        private bool supportsMutableLibraries => arTrackedImageManager.descriptor.supportsMutableLibrary;

        private void OnEnable()
        {
            ServerReceiver.Instance.OnImageLibraryReceived += XRRemoteTrackedImageSender_OnImageLibraryReceived;
            // arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            ServerReceiver.Instance.OnImageLibraryReceived -= XRRemoteTrackedImageSender_OnImageLibraryReceived;
            // arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void Update() //[delete]
        {
            if (arTrackedImageManager.enabled == true && arTrackedImageManager.referenceLibrary != null)
            {
                libraryCountText.text = $"Library Count: {arTrackedImageManager.referenceLibrary.count}";
            }
        }

        private void XRRemoteTrackedImageSender_OnImageLibraryReceived(object sender, EventArgs e)
        {   
            
            List<SerializableTexture2D> serializedTextures = ServerReceiver.Instance.serializedTextures;

            if (serializedTextures == null || serializedTextures.Count == 0)
            {
                Debug.LogError("No images received from client.");
                return;
            }

            if (supportsMutableLibraries)
            {
                Debug.Log("XRRemoteImageManager: This XRImageTrackingSubsystem supports mutable libraries.");
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

        private Dictionary<Texture2D, XRInfo> ReconstructLibrary(List<SerializableTexture2D> serializedTextures)
        {
            Debug.Log("XRRemoteImageManager: Reconstructing library from received images.");
            Dictionary<Texture2D, XRInfo> reconstructedImages = new Dictionary<Texture2D, XRInfo>();

            foreach (SerializableTexture2D texture in serializedTextures)
            {
                Texture2D tex = texture.ConvertFromSerializableTexture2DToTexture2D(out XRInfo xrInfo);
                reconstructedImages.Add(tex, xrInfo);
            }

            return reconstructedImages;
        }

        private void AddImagesToLibrary(MutableRuntimeReferenceImageLibrary mutableLibrary, Dictionary<Texture2D, XRInfo> reconstructedImages)
        {
            Debug.Log("XRRemoteImageManager: Adding images to library.");
            foreach (KeyValuePair<Texture2D, XRInfo> entry in reconstructedImages)
            {

                //using extension method that accepts texture
                //https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/api/UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.html
                //no memory management necessary with this method
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

                    //[review] is this necessary, or unhelpful/undesirable?
                    JobHandle jobHandle = jobState.jobHandle;
                    jobHandle.Complete();

                    if (jobState.status == AddReferenceImageJobStatus.Success)
                    {
                        Debug.Log($"XRRemoteImageManager: Image {newImageName} added to library successfully.");
                    }
                    else
                    {
                        //should report status "ErrorInvalidImage" if arcore rejects image
                        Debug.LogWarning($"XRRemoteImageManager: Failed to add image {newImageName} to library. {jobState.status}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"XRRemoteImageManager: Failed to add image {entry.Value.name} to library. {e}");
                }
            }
        }

        private void InitializeNativeImageManager(MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            //Do i need to do things with the subsystem here??
            Debug.Log("XRRemoteImageManager: Initializing native image manager."); //[delete]
            arTrackedImageManager.enabled = false;
            arTrackedImageManager.referenceLibrary = mutableLibrary;
            arTrackedImageManager.enabled = true;
            // arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        // private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        // {
        //     Debug.Log("XRRemoteImageManager: Tracked images changed.");
            
        //     foreach (ARTrackedImage trackedImage in eventArgs.added)
        //     {
        //         string imageName = trackedImage.referenceImage.name;
        //         imageNameText.text = $"{imageName} Detected";
        //     }
        //     foreach (ARTrackedImage trackedImage in eventArgs.updated)
        //     {
        //         string imageName = trackedImage.referenceImage.name;
        //         imageNameText.text = $"{imageName} Detected";
        //     }
        // }

    }
}
