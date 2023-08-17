//-------------------------------------------------------------------------------------------------------
// <copyright file="ServerReceiver.cs" createdby="cSustrich">
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

namespace XRRemote
{   
    public class XRRemoteImageManager : MonoBehaviour
    {
        [Tooltip("The native AR Tracked Image Manager component attached to AR Session Origin.")]
        [SerializeField] private ARTrackedImageManager manager;
        public ARTrackedImageManager Manager => manager;
        [HideInInspector] public XRReferenceImageLibrary imageLibrary {get; private set;}
        public static XRRemoteImageManager Instance { get; private set; }
        private bool readyToSend = false;
        public List<SerializableTexture2D> serializedLibrary {get; private set;}

        private void Awake()
        {
            //[review] switch to  new instancing method
            if (Instance != null)
            {
                Debug.LogError("CustomNdiReceiver must be only one in the scene.");
            
                return;
            }

            Instance = this;
        }

        public void Start()
        {
            StartCoroutine(UpdateLibrary());
        }

        public void OnDisable()
        {
            StopAllCoroutines();
        }


        private IEnumerator UpdateLibrary()
        {
            while (true)
            {
                Debug.Log("UpdateLibrary");
                if (manager != null)
                {
                    if (manager.referenceLibrary != null)
                    {
                        XRReferenceImageLibrary imageLibrary = manager.referenceLibrary as XRReferenceImageLibrary;
                        if (CheckLibraryValidity(imageLibrary))
                        {
                            SerializeImageLibrary(imageLibrary);
                        }
                    }
                    else
                    {
                        readyToSend = false;
                        // if (DebugFlags.displayXRRemoteImageManagerStats)
                        // {
                            Debug.LogWarning("XRRemoteImageManager: No reference library found on ARTrackedImageManager.");
                        // }
                    }
                } 
                else
                {
                    readyToSend = false;
                // if (DebugFlags.displayXRRemoteImageManagerStats)
                // {
                    Debug.LogWarning("XRRemoteImageManager: ARTrackedImageManager not found.");
                // }
                }
                
                yield return new WaitForSeconds(1f);
            }
        }

        private bool CheckLibraryValidity(XRReferenceImageLibrary imageLibrary)
        {
            readyToSend = true;

            for (int i = 0; i < imageLibrary.count; i++)
            {
                //check for issues with library entry that would cause errors on server
                bool emptyTextureError = imageLibrary[i].textureGuid.Equals(Guid.Empty);
                bool sizeError = imageLibrary[i].specifySize && imageLibrary[i].size.ToString() == "(0.00, 0.00)";
                if (emptyTextureError || sizeError)
                {
                    readyToSend = false;

                    // if (DebugFlags.displayXRRemoteImageManagerStats)
                    // {
                        if (emptyTextureError)
                        {
                            Debug.LogWarning("XRRemoteImageManager: Reference Image with no texture found.");
                        }
                        if (sizeError)
                        {
                            Debug.LogWarning($"XRRemoteImageManager: Reference Image with specified size (0, 0) found at index {i}.");
                        }
                    // }
                }
            }
            return readyToSend;
        }

        private void SerializeImageLibrary(XRReferenceImageLibrary library)
        {
            List<SerializableTexture2D> serializedLibrary = new List<SerializableTexture2D>();
            for (int i = 0; i < library.count; i++)
            {
                Texture2D texture = library[i].texture;
                SerializableTexture2D serializedTexture = new SerializableTexture2D(library[i]);
                if (serializedTexture.texData == null) Debug.Log("null tex data whyyyy");
                if (serializedTexture.texData != null) serializedLibrary.Add(serializedTexture);
            }

            this.serializedLibrary = serializedLibrary;
            StopAllCoroutines();

            if (serializedLibrary.Count != library.count)
            {
                Debug.LogWarning("XRRemoteImageManager: Some images not sent to device. See individual Errors for further details.");
            }
            Debug.Log($"SerializedLibrary.Count: {serializedLibrary.Count}");
        }

    }
}
